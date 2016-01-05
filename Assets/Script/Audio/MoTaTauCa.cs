using UnityEngine;
using System.Collections;

public class MoTaTauCa : MonoBehaviour {

	public static MoTaTauCa instance;
	
	
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
