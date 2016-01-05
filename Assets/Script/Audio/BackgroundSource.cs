using UnityEngine;
using System.Collections;

public class BackgroundSource : MonoBehaviour {
	public static BackgroundSource instance;

	public AudioSource backgroundSource;

	void Awake()
	{
		instance = this;
		backgroundSource = GetComponent<AudioSource> ();
	}
	// Use this for initialization
	void Start () {
		
	}
}
