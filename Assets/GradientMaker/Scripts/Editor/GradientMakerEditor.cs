using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.IO;
using System;

class GradientMakerEditor : EditorWindow {

	public enum GradType
	{
		Horizontal,
		Vertical,
		Radial
	};
	
	private GradientWrapper Grad;
	private Gradient LastGrad;
	private bool _invertGradient = false;
	private GradType gradType = GradType.Vertical; 
	private float _radialGradientFalloff = 2f;
	private string _fileName = "Unnamed Gradient";
	private bool _overWriteExisting = false;
	private int _gradientSizeX = 256;
	private int _gradientSizeY = 256;
	private bool _fixedAspect = true;
	private Texture2D checker;
	private Texture2D outTexture;
	private string path;
	private Color[] gradientColours;
	private SerializedProperty colorGradient;
	private SerializedObject serializedGradient;
	private UnityEngine.GameObject tempObject;
	
	[MenuItem ("Window/Gradient Maker")]
	
	static void Init () {
		GradientMakerEditor window = (GradientMakerEditor)EditorWindow.GetWindow(typeof (GradientMakerEditor));
		window.title = "Gradient Maker Lite";
		window.minSize = new Vector2(260,410);
		
	}

	private void OnGUI(){
		Grad = GUIGradientField.GradientField("Gradient", Grad);

		// Gradient Options
		_invertGradient = EditorGUILayout.Toggle("Invert Gradient", _invertGradient);
		gradType = (GradType)EditorGUILayout.EnumPopup ("Gradient Type", gradType);
		
		// Gradient Falloff
		if(gradType == GradType.Radial){
				EditorGUILayout.BeginHorizontal();
				string falloff = EditorGUILayout.TextField("Falloff", _radialGradientFalloff.ToString());
				float.TryParse(falloff, out _radialGradientFalloff);
			GUILayout.EndHorizontal();
			_radialGradientFalloff = GUILayout.HorizontalSlider(_radialGradientFalloff, 0f, 100f);
		}
		
		// Output filename
		_fileName = EditorGUILayout.TextField("Output Filename",_fileName);

		// Gradient output size/aspect
		EditorGUILayout.BeginHorizontal();
			if(_fixedAspect){
				EditorGUILayout.PrefixLabel("Gradient Size");
			} else {
				EditorGUILayout.PrefixLabel("Gradient Width");
			}
			string gradSizeX = EditorGUILayout.TextField(_gradientSizeX.ToString());
			int.TryParse(gradSizeX, out _gradientSizeX);
			EditorGUILayout.PrefixLabel("px");
		EditorGUILayout.EndHorizontal();

		if(_fixedAspect){
			_gradientSizeY = _gradientSizeX;
		} else {
			string gradSizeY = EditorGUILayout.TextField("Gradient Height",_gradientSizeY.ToString());
			int.TryParse(gradSizeY, out _gradientSizeY);
		}

		// Set parammeters of gradient preview area
		GUILayoutOption[] guioptions = new GUILayoutOption[2];
		guioptions[0] = GUILayout.Height(Mathf.Clamp(_gradientSizeX,4,256));
		guioptions[1] = GUILayout.Width(Mathf.Clamp(_gradientSizeY,4,256));
		
		GUIStyle previewStyle = new GUIStyle();
		
		previewStyle.alignment = TextAnchor.MiddleCenter;
		
		// Load checker pattern for preview area
	
		string path = null;
		
		if(checker == null || checker.width != _gradientSizeX || checker.height != _gradientSizeY){
			
			if(Application.platform == RuntimePlatform.OSXEditor){
				path = Application.dataPath + "/GradientMaker/";
			} else if (Application.platform == RuntimePlatform.WindowsEditor){
				path = Application.dataPath + "\\GradientMaker\\";		
			} 
			
			byte[] rawChecker = File.ReadAllBytes(path + "Checker.png");
			checker = new Texture2D(_gradientSizeX, _gradientSizeY);
			checker.LoadImage(rawChecker);
		}
		
		// Draw prewiew area
		GUILayout.BeginHorizontal(checker, previewStyle, guioptions);
            GUILayout.Label(outTexture, guioptions);
		GUILayout.EndHorizontal();
		
		_overWriteExisting = EditorGUILayout.ToggleLeft("Overwrite existing", _overWriteExisting);
		
		GUILayout.BeginHorizontal();
			if(GUILayout.Button("Save Gradient")){
				InitProcessGradient(false);
			}
		GUILayout.EndHorizontal();
		
		// Update preview if nessisary
		if(GUI.changed){
			Undo.RecordObject(this, "Gradient Changed");
			InitProcessGradient(true);
			EditorUtility.SetDirty (this);
		}
	}
	
	private static GradientWrapper CreateGradTexture(){
		// Create gradient object and assign generic starting colours	
		GradientWrapper.ColorKey[] gck = new GradientWrapper.ColorKey[2];
		gck[0] = new GradientWrapper.ColorKey(Color.black, 0f);
		gck[1] = new GradientWrapper.ColorKey(Color.white, 1f);
		GradientWrapper.AlphaKey[] gak = new GradientWrapper.AlphaKey[2];
		gak[0] = new GradientWrapper.AlphaKey(1f, 0f);
		gak[1] = new GradientWrapper.AlphaKey(1f, 1f);
		GradientWrapper gw = new GradientWrapper();
		gw = GUIGradientField.GradientField("Gradient", gw);
		gw.SetKeys(gck, gak);
		return gw;
	}
	
	private void InitProcessGradient(bool preview){
		if (!outTexture){
			outTexture = new Texture2D(_gradientSizeX,_gradientSizeY);
			outTexture.hideFlags = HideFlags.HideAndDontSave;
		}
		
		outTexture.Resize (_gradientSizeX, _gradientSizeY);
		gradientColours = new Color[_gradientSizeX * _gradientSizeY];
		
		switch (gradType) {
		case GradType.Horizontal:
			HorizontalGradient();
			break;
		case GradType.Vertical:
			VerticalGradient();
			break;
		case GradType.Radial:
			RadialGradient();
			break;
		}
		
		ProcessGradient (preview);
	}
	
	private void HorizontalGradient(){
		for (int i = 1; i < ((float)_gradientSizeX*_gradientSizeY); i++) {
			if (_invertGradient) {
				gradientColours [i - 1] = Grad.Evaluate (1 - (1f / _gradientSizeY * Mathf.FloorToInt (i % _gradientSizeX)));
			} else {
				gradientColours [i] = Grad.Evaluate (1f / _gradientSizeY * Mathf.FloorToInt (i % _gradientSizeX));
			}
		}
	}
	
	private void VerticalGradient(){
		for (int i = 1; i < ((float)_gradientSizeX*_gradientSizeY); i++) {
			if (_invertGradient) {
				gradientColours [i - 1] = Grad.Evaluate (1 - (1f / _gradientSizeY * Mathf.FloorToInt (i / _gradientSizeX)));
			} else {
				gradientColours [i - 1] = Grad.Evaluate (1f / _gradientSizeY * Mathf.FloorToInt (i / _gradientSizeX));
			}
		}
	}
	
	private void RadialGradient(){
		Vector2 centerPoint = new Vector2 (_gradientSizeX * 0.5f, _gradientSizeY * 0.5f);
		Vector2 curPoint = Vector2.zero;
		for (int i = 1; i < ((float)_gradientSizeX*(float)_gradientSizeY); i++) {
			curPoint = new Vector2 (i % _gradientSizeX, Mathf.FloorToInt (i / _gradientSizeX));
			float dist = 1 / (centerPoint - curPoint).magnitude;
			if (_invertGradient) {
				gradientColours [i - 1] = Grad.Evaluate (1 - (dist * _radialGradientFalloff));
			} else {
				gradientColours [i - 1] = Grad.Evaluate (dist * _radialGradientFalloff);
			}
		}
	}
	
	private void ProcessGradient(bool preview){
		outTexture.SetPixels (gradientColours);
		outTexture.Apply ();
		
		if(!preview) {
			byte[] bytes = outTexture.EncodeToPNG ();
			OutputGradient (bytes);
		}
	}
	
	private void OutputGradient(byte[] bytes){
		// Platform specific file-out location
		if(Application.platform == RuntimePlatform.OSXEditor){
			path = Application.dataPath + "/GradientMaker/Gradients/";
		} else if (Application.platform == RuntimePlatform.WindowsEditor){
			path = Application.dataPath + "\\GradientMaker\\Gradients\\";
		}
		
		if(!Directory.Exists(path)){
			AssetDatabase.CreateFolder("Assets/GradientMaker", "Gradients");
			Debug.Log("No 'Gradients' folder found, creating it...");
		}
		
		// Output the file
		bool option = false;
		bool alreadyExists = File.Exists(path + _fileName + ".png");
		if(alreadyExists && !_overWriteExisting){
			option = EditorUtility.DisplayDialog("Gradient Already Exists",
			                                     "A gradient of this name already exists, do you want to overwrite it?",
			                                     "Overwrite",
			                                     "Cancel");
		} else {
			option = true;
		}
		if(option){
			File.WriteAllBytes((path + _fileName + ".png"), bytes);
			AssetDatabase.Refresh();
			var outputFile = AssetDatabase.LoadAssetAtPath<Texture>("Assets/GradientMaker/Gradients/" + _fileName + ".png");
			string logString = alreadyExists ? "Gradient Overwritten: " : "Gradient saved: ";
			Debug.Log(logString + outputFile, outputFile);
			EditorGUIUtility.PingObject(outputFile);
		}
	}
}