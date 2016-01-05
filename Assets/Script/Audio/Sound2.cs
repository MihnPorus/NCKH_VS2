﻿using UnityEngine;
using System.Collections;

public class Sound2 : MonoBehaviour {

	public static Sound2 instance;
	
	
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
