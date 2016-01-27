using UnityEngine;
using System.Collections;

public class View3D : MonoBehaviour {

    // Camera cam;
    AudioSource source;
    GameObject model;
    Object3Ddata data;
	// Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();
        EventManager.Instance.AddListener("On3DShow", OnEvent);
        //EventManager.Instance.AddListener("OnReload", OnEvent);
        gameObject.SetActive(false);
	}
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //if (model != null)
            Destroy(model);
            StopAllCoroutines();
            
            EventManager.Instance.PostNotification("OnEndOfView3D", this, data.id);
            gameObject.SetActive(false);
        }
    }

    public void OnEvent(string eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case "On3DShow":
                {
                    data = (Object3Ddata)param;
                    gameObject.SetActive(true);
                    StartCoroutine(AutoPlayContent(data));
                    break;
                }

            //case "OnReload":
            //    {
            //        EventManager.Instance.AddListener("On3DShow", OnEvent);
            //        Debug.Log("OnReload");
            //        break;
            //    }
            default:
                break;
        }
    }

    IEnumerator AutoRotate(GameObject model)
    {
        Transform trans = model.GetComponent<Transform>();
        while (true)
        {
            trans.Rotate(0, 20 * Time.deltaTime, 0);
            yield return null;
        }
    }

    IEnumerator AutoPlayContent(Object3Ddata data)
    {
        model = Instantiate(data.model) as GameObject;
        model.transform.parent = gameObject.transform;

        model.transform.localPosition = Vector3.zero;
        //model.transform.Translate(transform.position);

        Coroutine rotate = StartCoroutine(AutoRotate(model));
        yield return StartCoroutine(data.PlayAudio(source, false));

        StopCoroutine(rotate);
        Destroy(model);
        EventManager.Instance.PostNotification("OnEndOfView3D", this, data.id);
        gameObject.SetActive(false);
    }
}