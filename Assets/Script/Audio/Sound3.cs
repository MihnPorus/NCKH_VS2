using UnityEngine;
using System.Collections;

public class Sound3 : MonoBehaviour {

	public static Sound3 instance;
	
	
	public void Start(){
		instance = this;
	}

	public void Play(){
		GetComponent<AudioSource>().Play ();
	}
	
	public void Stop(){
		GetComponent<AudioSource>().Stop();
	}
}
