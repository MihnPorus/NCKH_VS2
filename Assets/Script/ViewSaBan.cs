using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ViewSaBan : MonoBehaviour {

    // SaBanInfo
    PictureData sabanData;

    AudioSource audioSource;
    int count;
    int countIsland = 0;

    public Image bgImage;
    public GameObject islandView;          // background cac dao
    public Image mainImage;         // Hien thi noi dung cac dao
    public int numberOfIsland;
	// Use this for initialization
    void Start()
    {
        // Nhan su kien nhan data dau tien tu SaBan
        EventManager.Instance.AddListener("OnSabanFirstTime", OnEvent);

        // Nhan su kien nhan data thu 2 tro di
        EventManager.Instance.AddListener("OnSabanShowTime", OnEvent);
        
        // Nhan su kien nhan data cuoi
        EventManager.Instance.AddListener("OnSabanLastTime", OnEvent);

        // Nhan su kien play noi dung dao
        EventManager.Instance.AddListener("OnIslandPlay", OnEvent);

        audioSource = GetComponent<AudioSource>();

        // Man hinh hien thi anh noi dung dao
        mainImage.gameObject.SetActive(false);
        
        // Man hinh hien thi danh sach dao
        islandView.SetActive(false);

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopAllCoroutines();
            Item.isInteractable = true;
            EventManager.Instance.PostNotification("OnEndOfView2D", this, 15);
            // Man hinh hien thi anh noi dung dao
            mainImage.gameObject.SetActive(false);

            // Man hinh hien thi danh sach dao
            islandView.SetActive(false);

            bgImage.gameObject.SetActive(true);

            countIsland = 0;

            gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            StopAllCoroutines();
            if (audioSource.isPlaying)
                sabanData.Stop();
            if (countIsland == 0)
            {
                countIsland = 1;
                mainImage.gameObject.SetActive(true);
                islandView.SetActive(true);
                bgImage.gameObject.SetActive(false);

                EventManager.Instance.PostNotification("OnRequestIslandData", this, countIsland);
                Debug.Log(countIsland);
            }
            else if (countIsland < numberOfIsland - 1)
            {
                mainImage.gameObject.SetActive(true);
                islandView.SetActive(true);
                countIsland++;
                EventManager.Instance.PostNotification("OnRequestIslandData", this, countIsland);
            }
            else
            {
                EventManager.Instance.PostNotification("OnEndOfView2D", this, 15);
                bgImage.gameObject.SetActive(true);
                mainImage.gameObject.SetActive(false);
                countIsland = 0;
                gameObject.SetActive(false);
            }
        }
    }

    public void OnEvent(string eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case "OnSabanFirstTime":
                {
                    gameObject.SetActive(true);
                    count = 0;
                    sabanData = (PictureData)param;

                    Debug.Log("OnFirstTime");
                    
                    // Khong cho tuong tac voi cac vat the khi dang trong man hinh 2D
                    Item.isInteractable = false;

                    // Play detail audio
                    StartCoroutine(PlayContent(true, false));

                    break;
                }

            case "OnSabanShowTime":
                {
                    sabanData = (PictureData)param;

                    StartCoroutine(PlayContent(false, false));
                    Debug.Log("OnShowTime");

                    break;
                }

            case "OnSabanLastTime":
                {
                    
                    sabanData = (PictureData)param;

                    StartCoroutine(PlayContent(false, true));

                    // Mo các đảo để download
                    islandView.SetActive(true);

                    Debug.Log("OnLastTime");

                    break;
                }

            case "OnIslandPlay":
                {
                    sabanData = (PictureData)param;
                    //Debug.Log(param);

                    StartCoroutine(AutoPlayIsland());
                    
                    break;
                }

            default:
                break;
        }
    }

    // Tu dong play tung phan noi dung sa ban
    IEnumerator PlayContent(bool isFirst, bool isLast)
    {
        StartCoroutine(sabanData.PlayImage(bgImage));
        //Debug.Log(sabanData.detailAudio);
        if (!isFirst)
            yield return StartCoroutine(sabanData.PlayAudio(audioSource, true));
        yield return StartCoroutine(sabanData.PlayAudio(audioSource, false));

        count++;
        if (!isLast)
            EventManager.Instance.PostNotification("OnSabanNextData", this, count);
        else
        {
            bgImage.gameObject.SetActive(false);
            countIsland = 1;
            EventManager.Instance.PostNotification("OnRequestIslandData", this, countIsland);
            Debug.Log(countIsland);
        }

    }

    // Play noi dung cac dao lan luot
    IEnumerator AutoPlayIsland()
    {
        mainImage.gameObject.SetActive(true);

        StartCoroutine(sabanData.PlayImage(mainImage));
        yield return StartCoroutine(sabanData.PlayAudio(audioSource, true));
        yield return StartCoroutine(sabanData.PlayAudio(audioSource, false));
        
        mainImage.gameObject.SetActive(false);

        countIsland++;
        if (countIsland < numberOfIsland)
            EventManager.Instance.PostNotification("OnRequestIslandData", this, countIsland);
        else
        {
            EventManager.Instance.PostNotification("OnEndOfView2D", this, 15);
            islandView.SetActive(false);
            gameObject.SetActive(false);
        }
        //Debug.Log(countIsland);
    }
}