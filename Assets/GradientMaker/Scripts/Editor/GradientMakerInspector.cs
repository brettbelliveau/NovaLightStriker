// This is a legacy class from v1.0 of gradient maker and will soon be gone for good //
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using UnityEditor;

[CustomEditor(typeof(GradientMaker))]

class GradientMakerInspector : Editor {

	public static GameObject gradMaker = null;
	
	private GradientMaker gradEdit;
	
	[MenuItem ("GameObject/Gradient Maker")]
	
	static void CreateGradientMaker(){
		gradMaker = GameObject.Find("GradientMaker");
		if (!gradMaker) {
			gradMaker = new GameObject ("GradientMaker");
			gradMaker.AddComponent<GradientMaker> ();
		}
		GameObject[] sel = new GameObject[1];
		sel[0] = gradMaker;
		Selection.objects = sel;
	}
	
	public override void OnInspectorGUI(){
		DrawDefaultInspector();
		gradEdit = (GradientMaker)target;
		
		EditorGUIUtility.LookLikeControls();
		//// Gradient GUI element is being provided by GradientEditor class as it's an internal UI element
		
		// Gradient Options
		gradEdit._invertGradient = EditorGUILayout.Toggle("Invert Gradient", gradEdit._invertGradient);
		gradEdit.gradType = (GradType)EditorGUILayout.EnumPopup ("Gradient Type", gradEdit.gradType);
		
		// Gradient Falloff
		if(gradEdit.gradType == GradType.Radial){
			GUILayout.BeginHorizontal();
				string falloff = EditorGUILayout.TextField("Radial Gradient Falloff", gradEdit._radialGradientFalloff.ToString());
				float.TryParse(falloff, out gradEdit._radialGradientFalloff);
				gradEdit._radialGradientFalloff = GUILayout.HorizontalSlider(gradEdit._radialGradientFalloff, 0f, 100f);
			GUILayout.EndHorizontal();
		}
		
		// Output filename
		gradEdit._fileName = EditorGUILayout.TextField("Output Filename",gradEdit._fileName);
		gradEdit._overWriteExisting = EditorGUILayout.Toggle("Overwrite existing", gradEdit._overWriteExisting);

		// Gradient output size/aspect
		EditorGUILayout.BeginHorizontal();
			if(gradEdit._fixedAspect){
				EditorGUILayout.PrefixLabel("Gradient Size");
			} else {
				EditorGUILayout.PrefixLabel("Gradient Width");
			}
			string gradSizeX = EditorGUILayout.TextField(gradEdit._gradientSizeX.ToString());
			int.TryParse(gradSizeX, out gradEdit._gradientSizeX);
			EditorGUILayout.PrefixLabel("px");
		EditorGUILayout.EndHorizontal();

		if(gradEdit._fixedAspect){
			gradEdit._gradientSizeY = gradEdit._gradientSizeX;
		} else {
			string gradSizeY = EditorGUILayout.TextField("Gradient Height",gradEdit._gradientSizeY.ToString());
			int.TryParse(gradSizeY, out gradEdit._gradientSizeY);
		}

		// Set parammeters of gradient preview area
		GUILayoutOption[] guioptions = new GUILayoutOption[2];
		guioptions[0] = GUILayout.Height(Mathf.Clamp(gradEdit._gradientSizeX,gradEdit._gradientSizeX, 256));
        guioptions[1] = GUILayout.Width(Mathf.Clamp(gradEdit._gradientSizeX,gradEdit._gradientSizeX, 256));
		
		GUIStyle previewStyle = new GUIStyle();
		
		previewStyle.alignment = TextAnchor.MiddleCenter;
		
		// Load checker pattern for preview area
	
		string path = null;
		
		if(Application.platform == RuntimePlatform.OSXEditor){
			path = Application.dataPath + "/GradientMaker/";
		} else if (Application.platform == RuntimePlatform.WindowsEditor){
			path = Application.dataPath + "\\GradientMaker\\";		
		} 
		
		byte[] rawChecker = File.ReadAllBytes(path + "Checker.png");
		Texture2D checker = new Texture2D(gradEdit._gradientSizeX, gradEdit._gradientSizeY);
		checker.LoadImage(rawChecker);
		
		// Draw prewiew area
		GUILayout.BeginHorizontal(checker, previewStyle, guioptions);
            GUILayout.Label(gradEdit.outTexture, guioptions);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
			if(GUILayout.Button("Save Gradient")){
				gradEdit.InitProcessGradient(false);
			}
		GUILayout.EndHorizontal();
		
		// Update preview if nessisary
		if(GUI.changed || gradEdit.GradientChanged()){
			Undo.RecordObject(this, "Gradient Changed");
			gradEdit.InitProcessGradient(true);
			EditorUtility.SetDirty (target);
		}
	}

}