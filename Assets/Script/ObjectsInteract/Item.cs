using UnityEngine;
using System.Collections;

public abstract class Item : MonoBehaviour {

    public int id;
    public Vector3 pointOfView;
    public Vector3 pointToLook;
    //public AudioClip clip;

    public static bool isInteractable = true;

    public GameObject model;

    abstract public IEnumerator OnClick();

    abstract public IEnumerator DownloadData();

    public void OnMouseDown()
    {
        if (isInteractable)
            StartCoroutine(OnClick());
    }

    public void OnMouseEnter()
    {
        // Neu dang o che do auto
        if (PlayerPrefs.GetInt("IsAutoMode") == 1)
            return;
        model.GetComponent<Renderer>().material.color = Color.red;
    }

    public void OnMouseExit()
    {
        if (PlayerPrefs.GetInt("IsAutoMode") == 1)
            return;
        model.GetComponent<Renderer>().material.color = Color.white;
    }
}
