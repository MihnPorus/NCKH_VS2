using UnityEngine;
using System.Collections;

public class Sound1 : MonoBehaviour {

	public static Sound1 instance;

	
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

	public bool isPlaying(){
		return GetComponent<AudioSource>().isPlaying;
	}
}
