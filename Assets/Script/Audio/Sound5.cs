﻿using UnityEngine;
using System.Collections;

public class Sound5 : MonoBehaviour {

	public static Sound5 instance;
	
	
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
