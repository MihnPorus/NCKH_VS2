using UnityEngine;
using System.Collections;
using System;

public class BiaChuQuyen : Item {

    public int order;

    // Cache dữ liệu
    Object3Ddata data;
    AudioSource source;
    int clickCount = 0;
    // Use this for initialization
    void Start()
    {
        
        #region Download Model at the start
        data = new Object3Ddata();
        data.id = id;
        StartCoroutine(data.GetAudio(1));
        //yield return StartCoroutine(data.GetModel());

        //while (!Loader.isManifestOK)
        //    yield return null;

        //AssetBundleLoadAssetOperation request = 
        //    BundleManager.LoadAssetAsync(data.modelBundle[0], data.modelBundle[1], typeof(GameObject));
        //if (request == null)
        //    yield break;
        ////Debug.Log(request);
        //yield return StartCoroutine(request);

        //data.model = request.GetAsset<GameObject>();

        //BundleManager.UnloadBundle(data.modelBundle[0]);

        //model = Instantiate(data.model) as GameObject;
        //model.transform.parent = gameObject.transform;
        //model.transform.localPosition = Vector3.zero;
        //model.transform.localScale = Vector3.one;


        #endregion

        source = GetComponent<AudioSource>();

        EventManager.Instance.AddListener("OnShowTime", OnEvent);
        EventManager.Instance.AddListener("OnFinishMoveToObject", OnEvent);
        EventManager.Instance.AddListener("OnMoveToObject", OnEvent);

        pointOfView = transform.TransformPoint(pointOfView);
    }

    public override IEnumerator OnClick()
    {
        if (PlayerPrefs.GetInt("IsAutoMode") == 1 || clickCount > 2)
            yield return null;
        else if (clickCount > 0)
        {
            clickCount++;
            //Debug.Log("Click count: " + clickCount);
            yield return null;
        }
        else
        {
            //Debug.Log("picture: " + pointOfView);
            EventManager.Instance.PostNotification("OnMoveToObject", this, base.pointOfView);
        }
    }

    public void OnEvent(string eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case "OnShowTime":
                {
                    if (order == (int)param)
                        StartCoroutine(AutoPlayContent());
                    break;
                }
            case "OnFinishMoveToObject":
                {

                    Vector3 temp = (Vector3)param;

                    if (temp.x == pointOfView.x && temp.z == pointOfView.z)
                    {
                        //Debug.Log("OnFinishMoveToObject: " + temp);
                        StartCoroutine(ManualPlayContent());
                    }
                    break;
                }
            case "OnMoveToObject":
                {
                    //Debug.Log("OnMoveToObject");
                    if (source.isPlaying)
                    {
                        data.isCancel = true;
                        data.Stop();
                    }

                    break;
                }

            default:
                break;
        }
    }

    public IEnumerator AutoPlayContent()
    {
        if (data.introAudio == null)
        {
            yield return StartCoroutine(DownloadData());
        }
        model.GetComponent<Renderer>().material.color = Color.red;
        //yield return StartCoroutine(data.PlayAudio(source, true));
        if (!data.isCancel)
        {
            yield return StartCoroutine(data.PlayAudio(source, false));
        }
        EventManager.Instance.PostNotification("OnEndOfView3D", this, id);
    }

    IEnumerator ManualPlayContent()
    {
        if (data.introAudio == null)
        {
            yield return StartCoroutine(DownloadData());

            //Debug.Log(data.introAudio + " - " + data.detailAudio);
        }
        model.GetComponent<Renderer>().material.color = Color.red;

        clickCount = 1;
        IEnumerator routine = data.PlayAudio(source, true);
        StartCoroutine(routine);
        while (clickCount < 2 && !data.isCancel)
        {
            if (!routine.MoveNext())
                break;
            yield return null;
        }
        source.Stop();
        clickCount = 0;

        if (!data.isCancel)
            yield return StartCoroutine(data.PlayAudio(source, false));

        model.GetComponent<Renderer>().material.color = Color.white;
    }

    public override IEnumerator DownloadData()
    {
        yield return StartCoroutine(data.GetAudio(1));
        Debug.Log(data.audioBundle);
        AssetBundleLoadAssetOperation request = BundleManager.LoadAssetAsync(data.audioBundle[0], data.audioBundle[1], typeof(AudioClip));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);
        data.introAudio = request.GetAsset<AudioClip>();



        request = BundleManager.LoadAssetAsync(data.audioBundle[2], data.audioBundle[3], typeof(AudioClip));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);
        data.detailAudio = request.GetAsset<AudioClip>();

        BundleManager.UnloadBundle(data.audioBundle[0]);

        //Debug.Log(data.introAudio + " - " + data.detailAudio);
    }
}
