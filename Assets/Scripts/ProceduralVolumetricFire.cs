using UnityEngine;

using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Mattatz.ProceduralVolumetricFire {

	[RequireComponent (typeof (MeshFilter))]
	[RequireComponent (typeof (MeshRenderer))]
	public class ProceduralVolumetricFire : MonoBehaviour {

		public float sliceSpacing = 0.02f;

		void Start () {
			CreateMesh();
		}
	
		void Update () {
		}

		void CreateMesh () {

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

			GetComponent<MeshFilter>().sharedMesh = mesh;
		}

	}


}

