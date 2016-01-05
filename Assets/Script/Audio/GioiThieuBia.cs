using UnityEngine;
using System.Collections;

public class GioiThieuBia : MonoBehaviour {

	public static GioiThieuBia instance;
	
	
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
