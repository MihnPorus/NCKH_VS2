using UnityEngine;
using System.Collections;

public class IntroduceSound : MonoBehaviour {

	public static IntroduceSound instance;
	public AudioClip[] audios;

	public void Start(){
		instance = this;
	}
	
	public void play1(){
		GetComponent<AudioSource>().Stop();
		GetComponent<AudioSource>().clip = audios[0];
		GetComponent<AudioSource>().Play();
		//AudioSource.PlayClipAtPoint(audios[0], new Vector3(0,0,0));
	}

	public void play2(){
		GetComponent<AudioSource>().Stop();
		GetComponent<AudioSource>().clip = audios[1];
		GetComponent<AudioSource>().Play();
		//AudioSource.PlayClipAtPoint(audios[1], new Vector3(0,0,0));
	}

	public void play3(){
		GetComponent<AudioSource>().Stop();
		GetComponent<AudioSource>().clip = audios[2];
		GetComponent<AudioSource>().Play();
		//AudioSource.PlayClipAtPoint(audios[2], new Vector3(0,0,0));
	}

	public void play4(){
		GetComponent<AudioSource>().Stop();
		GetComponent<AudioSource>().clip = audios[3];
		GetComponent<AudioSource>().Play();
		//AudioSource.PlayClipAtPoint(audios[3], new Vector3(0,0,0));
	}

	public void play5(){
		GetComponent<AudioSource>().Stop();
		GetComponent<AudioSource>().clip = audios[4];
		GetComponent<AudioSource>().Play();
		//AudioSource.PlayClipAtPoint(audios[4], new Vector3(0,0,0));
	}

	public void stop(){
		GetComponent<AudioSource>().Stop();

	}
	
}
