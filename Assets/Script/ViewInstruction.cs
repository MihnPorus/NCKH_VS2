using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class ViewInstruction : MonoBehaviour
{
    public Button auto;
    public Button manual;
    public GameObject player;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    void Start()
    {
        EventManager.Instance.AddListener("OnReload", OnEvent);
    }


    public void OnAutoClick() {
        PlayerPrefs.SetInt("IsAutoMode", 1);
        Instantiate(player);
        Cursor.visible = false;
        Hide();

    }

    public void OnManualClick()
    {
        PlayerPrefs.SetInt("IsAutoMode", 0);
        Instantiate(player);
        Cursor.visible = true;
        Hide();
    }

    void Show()
    {
        gameObject.SetActive(true);
        audioSource.UnPause();
        Cursor.visible = true;
    }

    void Hide()
    {
        gameObject.SetActive(false);
        audioSource.Pause();
    }


    public void OnEvent(string eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case "OnReload":
                {
                    Show();
                    break;
                }
        }
    }
}