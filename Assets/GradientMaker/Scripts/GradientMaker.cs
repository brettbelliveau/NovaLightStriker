// This is a legacy class from v1.0 of gradient maker and will soon be gone for good //
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

[ExecuteInEditMode]

public enum GradType{
	Horizontal,
	Vertical,
	Radial
};

public class GradientMaker : MonoBehaviour {

	[SerializeField] public Gradient Grad;
	[HideInInspector] public Gradient LastGrad;
	[HideInInspector] public bool _invertGradient = false;
	[HideInInspector] public GradType gradType = GradType.Vertical; 
	[HideInInspector] public float _radialGradientFalloff = 2f;
	[HideInInspector] public string _fileName = "Unnamed Gradient";
	[HideInInspector] public bool _overWriteExisting = false;
	[HideInInspector] public int _gradientSizeX = 256;
	[HideInInspector] public int _gradientSizeY = 256;
	[HideInInspector] public bool _fixedAspect = true;
	[HideInInspector] public Texture2D outTexture;
	private string path;
	private Color[] gradientColours;
	
	void Start (){
		CreateGradTexture ();
	}

	public void CreateGradTexture(){
		// Create gradient object and assign generic starting colours
		Grad = new Gradient();
		LastGrad = new Gradient();
		GradientColorKey[] gck = new GradientColorKey[2];
		gck[0] = new GradientColorKey(Color.black, 0f);
		gck[1] = new GradientColorKey(Color.white, 1f);
		GradientAlphaKey[] gak = new GradientAlphaKey[2];
		gak[0] = new GradientAlphaKey(1f, 0f);
		gak[1] = new GradientAlphaKey(1f, 1f);
		Grad.SetKeys(gck, gak);
		LastGrad.SetKeys(gck, gak);
		
		outTexture = new Texture2D(_gradientSizeX,_gradientSizeY);
		outTexture.hideFlags = HideFlags.HideAndDontSave;
//		Debug.Log (outTexture);
	}

	public void InitProcessGradient(bool preview){
		if (!outTexture)
			CreateGradTexture ();
			
		// Check if sizes are valid
		
		if (_gradientSizeX < 4 || _gradientSizeY < 4) {
//			Debug.LogError ("Gradient must be at least 4px in either dimension");
			return;
		}
			
		outTexture.Resize (_gradientSizeX, _gradientSizeY);
		gradientColours = new Color[_gradientSizeX * _gradientSizeY];

		LastGrad.SetKeys(Grad.colorKeys,Grad.alphaKeys);

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
	
	public bool GradientChanged(){
		
		if (!outTexture)
			CreateGradTexture ();
	
		GradientColorKey[] lastgck = LastGrad.colorKeys;
		GradientColorKey[] newgck = Grad.colorKeys;
		GradientAlphaKey[] lastgak = LastGrad.alphaKeys;
		GradientAlphaKey[] newgak = Grad.alphaKeys;
		
		// First check to see if number of color or alpha keys is different
		if (lastgck.Length != newgck.Length || lastgak.Length != newgak.Length) 
			return true;
		
		// Now check if color or alpha keys are different
		
		for(int i = 0; i < lastgck.Length; i ++){
			if(lastgck[i].color != newgck[i].color || lastgck[i].time != newgck[i].time) 
				return true;
		}
		
		for(int i = 0; i < lastgak.Length; i ++){
			if(lastgak[i].alpha != newgak[i].alpha || lastgak[i].time != newgak[i].time ) 
				return true;
		}
		
		return false;
	}
	
}
#endif