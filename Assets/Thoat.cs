using UnityEngine;
using System.Collections;

public class Thoat : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//if(Input.GetKey("a")){
		if (Input.GetKeyDown (KeyCode.Escape) == true) {
			Application.Quit();
		}
	}

	/*void OnGUI(){
		
		if(GUI.Button(new Rect((Screen.width) /2, (Screen.height)/2, 50, 50), "Quit Game")){
			Application.Quit();
		}
	}*/
}
