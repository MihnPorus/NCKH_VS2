using UnityEngine;
using System.Collections;

public class ObjectRotator : MonoBehaviour {
	
	public float xRotation = 0.0f;
	public float yRotation = 0.0f;
	public float zRotation = 0.0f;

	
	
	// Update is called once per frame
	void Update () {
	
		  // Slowly rotate the object around its X axis at 1 degree/second.
        transform.Rotate(xRotation, yRotation, zRotation);
        
	}
}
