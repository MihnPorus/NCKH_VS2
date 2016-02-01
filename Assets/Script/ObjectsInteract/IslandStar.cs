using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class IslandStar : MonoBehaviour {

    public bool isReady = false;

    public int id;
    public int order;
    public GameObject textObject;
    public List<float> time;
    //public List<Sprite> sprites;
    //public AudioClip intro;
    //public AudioClip detail;

    PictureData data;

    Image starImg;

    void Start()
    {
        starImg = GetComponent<Image>();
        EventManager.Instance.AddListener("OnRequestIslandData", OnEvent);
        EventManager.Instance.AddListener("OnDownloadIsland", OnEvent);
        EventManager.Instance.AddListener("OnIslandPlay", OnEvent);
        if (order == 0)
            EventManager.Instance.PostNotification("OnDownloadIsland", this, 0);
    }

    public void OnMouseDown()
    {
        OnMouseEnter();
        Debug.Log("OnMouseDown");
        StartCoroutine(OnResponseIslandData());
    }

    void OnMouseEnter()
    {
        starImg.color = Color.red;
    }

    void OnMouseExit()
    {
        starImg.color = Color.white;
    }

    public IEnumerator DownLoadContent()
    {
        if (data == null)
        {
            data = new PictureData();
            data.id = id;

            #region Download Island Content
            yield return StartCoroutine(data.GetAudio(1));
            AssetBundleLoadAssetOperation request =
                BundleManager.LoadAssetAsync(data.audioBundle[0], data.audioBundle[1], typeof(AudioClip));
            if (request == null)
                yield break;
            yield return StartCoroutine(request);
            data.introAudio = request.GetAsset<AudioClip>();

            request = BundleManager.LoadAssetAsync(data.audioBundle[2], data.audioBundle[3], typeof(AudioClip));
            Debug.Log(request);
            if (request == null)
                yield break;
            yield return StartCoroutine(request);
            data.detailAudio = request.GetAsset<AudioClip>();
            // =====================================

            //// Download text================
            //bundleName = "";
            //assetName = "";
            //request = null;
            //DBManager.Instance.GetText(id, ref bundleName, ref assetName);
            //request = BundleManager.LoadAssetAsync(bundleName, assetName, typeof(TextAsset));
            //if (request == null)
            //    yield break;
            //yield return StartCoroutine(request);
            ////Debug.Log(request.GetAsset<TextAsset>());
            //data.text = request.GetAsset<TextAsset>();
            //// =====================================

            // Download sprites================
            yield return StartCoroutine(data.GetSprites());
            int size = data.spriteBundle.Length - 1;
            for (int i = 0; i < size; i += 2)
            {
                request = BundleManager.LoadAssetAsync(data.spriteBundle[i], data.spriteBundle[i + 1], typeof(Sprite));
                if (request == null)
                    yield break;
                yield return StartCoroutine(request);
                data.sprites.Add(request.GetAsset<Sprite>());
            }
            // =====================================

            data.imgTime = time;
            #endregion

        }

        isReady = true;

        EventManager.Instance.PostNotification("OnDownloadIsland", this, order + 1);
    }

    IEnumerator WaitForReady()
    {
        while (!isReady)
        {
            yield return null;
        }
    }

    IEnumerator OnResponseIslandData(int temp=-1)
    {
        if (order != temp && temp != -1)
        {
            OnMouseExit();
            // Tat ten dao di
            textObject.SetActive(false);
        }
        else if (order == temp || temp == -1)
        {
            // Hien thi ten dao
            textObject.SetActive(true);
            yield return StartCoroutine(WaitForReady());
            
            Debug.Log("order = temp = " + order);
            OnMouseEnter();
            EventManager.Instance.PostNotification("OnIslandPlay", this, data);
        }
    }

    public void OnEvent(string eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case "OnRequestIslandData":
                {
                    int temp = (int)param;
                    //Debug.Log("OnRequestIslandData: " + temp + order);
                    StartCoroutine(OnResponseIslandData(temp));
                    break;
                }

            case "OnDownloadIsland":
                {
                    int temp = (int)param;

                    if (order == temp)
                    {
                        Debug.Log("Download Island: " + order);
                        StartCoroutine(DownLoadContent());
                    }

                    break;
                }

            case "OnIslandPlay":
                {
                    PictureData temp = (PictureData)param;
                    if (data == null || temp.id != data.id)
                    {
                        OnMouseExit();
                        textObject.SetActive(false);
                    }
                    break;
                }

            default:
                break;
        }
    }
}