using UnityEngine;
using System.Collections;

public class TamBiet : MonoBehaviour {

	public static TamBiet instance;
	
	
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
