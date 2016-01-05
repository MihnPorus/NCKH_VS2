using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class View2D1 : MonoBehaviour {

    AudioSource audioSource;
    public Image image;
    public Text textObject;

    

    PictureData recvData;

	// Use this for initialization
	void Start () {
        EventManager.Instance.AddListener("OnPictureClick", OnEvent);
        audioSource = GetComponent<AudioSource>();
        gameObject.SetActive(false);
	}
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopAllCoroutines();
            EventManager.Instance.PostNotification("OnEndOfView2D", this, recvData.id);
            gameObject.SetActive(false);
        }
    }

    public void OnEvent(string eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case "OnPictureClick":
                {
                    gameObject.SetActive(true);
                    recvData = (PictureData)param;
                    if (recvData != null)
                    {
                        Item.isInteractable = false;
                        StartCoroutine(StartPlay());
                    }
                    break;
                }
            default:
                break;
        }
    }

    IEnumerator StartPlay()
    {
        if (recvData.text != null)
        {
            textObject.text = recvData.text.text;
        }
        StartCoroutine(recvData.PlayImage(image));
        yield return StartCoroutine(recvData.PlayAudio(audioSource, false));
        EventManager.Instance.PostNotification("OnEndOfView2D", this, recvData.id);
        recvData = null;
        gameObject.SetActive(false);
    }
    
}
