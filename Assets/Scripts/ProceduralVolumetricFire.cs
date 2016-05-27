using UnityEngine;

using System.Linq;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (MeshFilter))]
[RequireComponent (typeof (MeshRenderer))]
public class ProceduralVolumetricFire : MonoBehaviour {

	[SerializeField] float 
        sliceSpacing = 0.02f,
        fineness = 0.1f;

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

    Mesh CreateMesh() {
        Mesh mesh = new Mesh();

        int depth = (int)(1f / sliceSpacing);
        depth = Mathf.Max(depth, 1);

        int rows = (int)(1f / fineness);
        rows = Mathf.Max(rows, 2);

        int step = rows * 2;
        int tstep = (rows - 1) * 6;
        float inv = 1f / (rows - 1);

        Vector3[] vertices = new Vector3[depth * step];
        Vector3[] normals = new Vector3[depth * step];
        int[] triangles = new int[depth * tstep];

        for (int i = 0; i < depth; i++) {
            int offset = i * step;
            int toffset = i * tstep;

            float rate = (float)i / depth;
            float normalizedRate = (float)i / (depth - 1);

            for (int j = 0; j < rows; j++) {
                float rrate = j * inv;
                vertices[offset + j * 2] = new Vector3(-0.5f, rrate, (1f - rate) - 0.5f);
                vertices[offset + j * 2 + 1] = new Vector3(0.5f, rrate, (1f - rate) - 0.5f);
                normals[offset + j * 2] = new Vector3(0f, rrate, normalizedRate);
                normals[offset + j * 2 + 1] = new Vector3(1f, rrate, normalizedRate);
                if (j != rows - 1) {
                    var k = j * 6;
                    var v0 = offset + j * 2;
                    var v1 = offset + j * 2 + 1;
                    var v2 = offset + j * 2 + 2;
                    var v3 = offset + j * 2 + 3;
                    triangles[toffset + k + 0] = v0;
                    triangles[toffset + k + 1] = v2;
                    triangles[toffset + k + 2] = v1;
                    triangles[toffset + k + 3] = v1;
                    triangles[toffset + k + 4] = v3;
                    triangles[toffset + k + 5] = v2;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();

        return mesh;
    }

}

