using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
using SWS;

public class ViewInstruction : MonoBehaviour
{
    public Button auto;
    public Button manual;
    public GameObject player;
    public GameObject player2;
    public GameObject came;
    public static GameObject playerClone;
    AudioSource audioSource;
    GameObject sphere;

    GameObject cam;

    private Vector3 temp = Vector3.up;
    void Awake()
    {
       
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    void Start()
    {
        EventManager.Instance.AddListener("OnReload", OnEvent);
        
    }

    void OnEnable()
    {
        if (cam != null)
        {
            cam.transform.position = temp;
        }
    }
    public void OnAutoClick() {
        PlayerPrefs.SetInt("IsAutoMode", 1);
        Debug.Log("Dang la auto mode!");
        PlayerPrefs.Save();
        //ep kieu object cho playerClone thi moi su dung sitemap duoc 
        playerClone = (GameObject)Instantiate(player);
        cam = (GameObject)Instantiate(came);
        sphere = GameObject.FindGameObjectWithTag("sphere");
        //Debug.Log(sphere);

        
        cam.GetComponent<SmoothFollow>().target = sphere.transform;
        playerClone.GetComponent<bezierMove>().CameraMain = cam.gameObject.GetComponent<Camera>();
        Cursor.visible = false;
        Hide();

    }

    public void OnManualClick()
    {
        PlayerPrefs.SetInt("IsAutoMode", 0);
        Debug.Log("Dang la manual mode!");
        PlayerPrefs.Save();
        playerClone = (GameObject)Instantiate(player2);
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