using UnityEngine;

using System.Linq;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (MeshFilter))]
[RequireComponent (typeof (MeshRenderer))]
public class ProceduralVolumetricFire : MonoBehaviour {

	[SerializeField] 
		float sliceSpacing = 0.02f;

	public Vector4 scale = new Vector4(1f, 3f, 1f, 0.5f);
	public float
		lacunarity = 2.0f,
		gain = 0.5f,
		magnitude = 1.3f,
		attenuation = 0.2f;

	Material material;

	void Start () {
		GetComponent<MeshFilter>().sharedMesh = CreateMesh();
		material = GetComponent<MeshRenderer>().material;
	}

	void Update () {
		material.SetFloat("_Atten", attenuation);
		material.SetFloat("_Lacunarity", lacunarity);
		material.SetFloat("_Gain", gain);
		material.SetFloat("_Magnitude", magnitude);
	}

	Mesh CreateMesh () {
		Mesh mesh = new Mesh();

		int count = (int)(1f / sliceSpacing);

		Vector3[] vertices = new Vector3[count * 4];
		Vector3[] normals = new Vector3[count * 4];
		int[] triangles = new int[count * 6];

		for(int i = 0; i < count; i++) {

			int offset = i * 4;

			float rate = (float)i / count;

			// backward to front.
			vertices[offset + 0] = new Vector3(- 0.5f, - 0.5f, (1f - rate) - 0.5f);
			vertices[offset + 1] = new Vector3(  0.5f, - 0.5f, (1f - rate) - 0.5f);
			vertices[offset + 2] = new Vector3(  0.5f,   0.5f, (1f - rate) - 0.5f);
			vertices[offset + 3] = new Vector3(- 0.5f,   0.5f, (1f - rate) - 0.5f);

			float normalizedRate = (float)i / (count - 1);

			// use normal for tex position.
			normals[offset + 0] = new Vector3(0f, 0f, normalizedRate);
			normals[offset + 1] = new Vector3(1f, 0f, normalizedRate);
			normals[offset + 2] = new Vector3(1f, 1f, normalizedRate);
			normals[offset + 3] = new Vector3(0f, 1f, normalizedRate);

			int toffset = i * 6;
			triangles[toffset + 0] = offset;  
			triangles[toffset + 1] = offset + 3;  
			triangles[toffset + 2] = offset + 2;  

			triangles[toffset + 3] = offset + 1;  
			triangles[toffset + 4] = offset;  
			triangles[toffset + 5] = offset + 2;  
		}

		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.triangles = triangles;

		mesh.RecalculateBounds();

		return mesh;
	}


}

