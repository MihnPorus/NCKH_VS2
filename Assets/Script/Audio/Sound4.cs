using UnityEngine;
using System.Collections;

public class Sound4 : MonoBehaviour {

	public static Sound4 instance;
	
	
	public void Start(){
		instance = this;
	}

	public void Play(){
		GetComponent<AudioSource>().Play ();
	}

	public void Stop(){
		GetComponent<AudioSource>().Stop();
	}

	public float getLength(){
		return GetComponent<AudioSource>().clip.length;
	}

}
