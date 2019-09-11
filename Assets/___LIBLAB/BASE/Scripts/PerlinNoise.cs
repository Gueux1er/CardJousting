using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
	public float powerOne;
    public float powerTwo;
    float influenceOne;
	float influenceTwo;

	void Start () 
	{
        influenceOne = Random.Range (0.00f, 99.99f);
        influenceTwo = Random.Range (0.00f, 99.99f);
		Noise ();
	}

	void Update ()
    {
        influenceOne += Time.deltaTime;
        influenceTwo += Time.deltaTime;

		Noise();
	}

	void Noise() {
		MeshFilter mf = GetComponent<MeshFilter>();
		Vector3[] vertices = mf.mesh.vertices;
		for (int i = 0; i < vertices.Length; i++) 
		{    
			float xCoord = influenceOne + vertices[i].x;
			float zCoord = influenceTwo + vertices[i].z;
			vertices[i].z = (Mathf.PerlinNoise (xCoord, zCoord) - powerOne) * powerTwo;
		}
		mf.mesh.vertices = vertices;
		mf.mesh.RecalculateBounds();
		mf.mesh.RecalculateNormals();
	}
}