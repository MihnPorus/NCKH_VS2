using UnityEngine;
using System.Collections;

public class Begin : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
      public void EnglishChoose()
      {
		Application.LoadLevel("EnglishScene");
      }
      public void VietNamChoose()
      {
		Application.LoadLevel("Bao tang");
      }
      public void ChinaChoose()
      {
		Application.LoadLevel("ChinaScene");
      }


}
