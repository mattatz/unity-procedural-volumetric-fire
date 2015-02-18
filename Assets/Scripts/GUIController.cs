using UnityEngine;
using System.Collections;

using Mattatz.ProceduralVolumetricFire;

public class GUIController : MonoBehaviour {

	public ProceduralVolumetricFire fire;

	void Start () {
	}
	
	void Update () {
	}

	void OnGUI () {
		GUI.color = Color.white;

		GUIStyle titleStyle = new GUIStyle();
		titleStyle.fontStyle = FontStyle.Bold;
		GUIStyleState titleState = new GUIStyleState();
		titleState.textColor = GUI.color;
		titleStyle.normal = titleState;
		
		GUIStyle labelStyle = new GUIStyle();
		GUIStyleState labelState = new GUIStyleState();
		labelState.textColor = GUI.color;
		labelStyle.normal = labelState;
		labelStyle.fontSize = 13;

		Material mat = fire.renderer.material;

		GUILayout.BeginVertical("box");

		GUILayout.Label("Lacunarity");
		float lacunarity = GUILayout.HorizontalSlider(mat.GetFloat("_Lacunarity"), 0f, 3.0f);
		mat.SetFloat("_Lacunarity", lacunarity);

		GUILayout.Space(8);

		GUILayout.Label("Gain");
		float gain = GUILayout.HorizontalSlider(mat.GetFloat("_Gain"), 0f, 3.0f);
		mat.SetFloat("_Gain", gain);

		GUILayout.Space(8);

		GUILayout.Label("Magnitude");
		float mag = GUILayout.HorizontalSlider(mat.GetFloat("_Magnitude"), 0f, 3.0f);
		mat.SetFloat("_Magnitude", mag);

		GUILayout.Space(5);

		GUILayout.EndVertical();
	}

}
