using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class SaBan : Item {

    public List<PictureData> data;

    public List<int> ids;
    public List<float> mTime;
    

    public int order;
    public int numberOfData = 3;

    AudioSource aSource;
    int clickCount = 0;
    
	IEnumerator Start () {
        data = new List<PictureData>();
        for (int i = 0; i < numberOfData; i++)
        {
            PictureData temp = new PictureData();
            temp.id = ids[i];
            data.Add(temp);
        }
        yield return StartCoroutine(data[0].GetAudio(1));
        yield return StartCoroutine(data[0].GetText(1));
        yield return StartCoroutine(data[0].GetSprites());

        StartCoroutine(DataStorage.Instance.DownloadSaban(this, true));

        aSource = GetComponent<AudioSource>();

        EventManager.Instance.AddListener("OnShowTime", OnEvent);
        EventManager.Instance.AddListener("OnSabanNextData", OnEvent);
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

    // Download du lieu thu number
    IEnumerator DownloadData(int number)
    {
        if (number < numberOfData)
        {
            #region DownloadContent

            // Download audio================
            yield return StartCoroutine(data[number].GetAudio(1));
            AssetBundleLoadAssetOperation request =
                BundleManager.LoadAssetAsync(data[number].audioBundle[0], data[number].audioBundle[1], typeof(AudioClip));
            if (request == null)
                yield break;
            yield return StartCoroutine(request);
            data[number].introAudio = request.GetAsset<AudioClip>();

            Debug.Log(data[number].audioBundle);

            //request = null;
            request = BundleManager.LoadAssetAsync(data[number].audioBundle[2], data[number].audioBundle[3], typeof(AudioClip));
            //Debug.Log(request);
            if (request == null)
                yield break;
            yield return StartCoroutine(request);
            data[number].detailAudio = request.GetAsset<AudioClip>();

            //Debug.Log(data.detailAudio);
            //=======================

            // Download text================
            //bundleName = "";
            //assetName = "";
            //request = null;
            //DBManager.Instance.GetText(ids[number], ref bundleName, ref assetName);
            //request = BundleManager.LoadAssetAsync(bundleName, assetName, typeof(TextAsset));
            //if (request == null)
            //    yield break;
            //yield return StartCoroutine(request);
            ////Debug.Log(request.GetAsset<TextAsset>());
            //temp.text = request.GetAsset<TextAsset>();

            //Debug.Log(temp.text);
            // ==============================

            // Download sprites================
            yield return StartCoroutine(data[number].GetSprites());
            int size = data[number].spriteBundle.Length - 1;
            for (int i = 0; i < size; i += 2)
            {
                request = BundleManager.LoadAssetAsync(data[number].spriteBundle[i], data[number].spriteBundle[i + 1], typeof(Sprite));
                if (request == null)
                    yield break;
                yield return StartCoroutine(request);
                data[number].sprites.Add(request.GetAsset<Sprite>());
            }
            BundleManager.UnloadBundle(data[number].audioBundle[0]);
            #endregion
        }

    }

    // Tu dong download lan luot noi dung
    // va send noi dung toi view sa ban de play noi dung 
    IEnumerator AutoPlayContent()
    {
        // Nếu chưa có dữ liệu thì download về
        if (data[0].introAudio == null)
        {

            // yield return download du lieu dau tien o day
            #region First Data

            yield return StartCoroutine(DataStorage.Instance.DownloadSaban(this, false));


            List<float> tempTime = new List<float>();
            for (int i = 0; i < 7; i++)
            {
                tempTime.Add(mTime[i]);
            }
            data[0].imgTime = tempTime;

            #endregion
        }

        model.GetComponent<Renderer>().material.color = Color.red;
        yield return StartCoroutine(data[0].PlayAudio(aSource, true));

        if (data[0].isCancel)
        {
            EventManager.Instance.PostNotification("OnEndOfView2D", this, id);
            yield return null;
        }

        else
        {

            EventManager.Instance.PostNotification("OnSabanFirstTime", this, data[0]);

            yield return StartCoroutine(DownloadData(1));

            for (int i = 7; i < 11; i++)
            {
                data[1].imgTime.Add(mTime[i]);
            }

            yield return StartCoroutine(DownloadData(2));

            for (int i = 11; i < 18; i++)
            {
                data[2].imgTime.Add(mTime[i]);
            }
        }
    }

    IEnumerator ManualPlayContent()
    {
        // Nếu chưa có dữ liệu thì download về
        if (data[0].introAudio == null)
        {
            // yield return download du lieu dau tien o day
            #region First Data

            yield return StartCoroutine(DataStorage.Instance.DownloadSaban(this, false));


            List<float> tempTime = new List<float>();
            for (int i = 0; i < 7; i++)
            {
                tempTime.Add(mTime[i]);
            }
            data[0].imgTime = tempTime;

            #endregion
        }

        model.GetComponent<Renderer>().material.color = Color.red;

        clickCount = 1;
        IEnumerator routine = data[0].PlayAudio(aSource, true);
        StartCoroutine(routine);
        while (clickCount < 2 && !data[0].isCancel)
        {
            if (!routine.MoveNext())
                break;

            yield return null;
        }
        aSource.Stop();
        Debug.Log(clickCount);
        Debug.Log(data[0].isCancel);
        
        if (data[0].isCancel || clickCount < 2)
        {

            EventManager.Instance.PostNotification("OnEndOfView2D", this, id);
            Debug.Log("abc");
            yield return null;
        }

        if (!data[0].isCancel)
        {
           // MoveCharator.isRotatable = false;
            EventManager.Instance.PostNotification("OnSabanFirstTime", this, data[0]);

            for (int i = 7; i < 11; i++)
            {
                data[1].imgTime.Add(mTime[i]);
            }

            for (int i = 11; i < 18; i++)
            {
                data[2].imgTime.Add(mTime[i]);
            }
        }
        else if (!data[0].isCancel && clickCount < 2)
        {
            //MoveCharator.isRotatable = false;
            EventManager.Instance.PostNotification("OnSabanFirstTime", this, data[0]);

            for (int i = 7; i < 11; i++)
            {
                data[1].imgTime.Add(mTime[i]);
            }


            for (int i = 11; i < 18; i++)
            {
                data[2].imgTime.Add(mTime[i]);
            }
        }
        else
            EventManager.Instance.PostNotification("OnEndOfView2D", this, id);
        clickCount = 0;
    }

    public void OnEvent(string eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case "OnShowTime":
                {
                    int temp = (int)param;
                    if (temp == order)
                        StartCoroutine(AutoPlayContent());
                    break;
                }

            case "OnSabanNextData":
                {
                    int t = (int)param;
                    if (t == numberOfData - 1)
                    {
                        EventManager.Instance.PostNotification("OnSabanLastTime", this, data[numberOfData - 1]);
                        
                    }
                    else
                    {
                        EventManager.Instance.PostNotification("OnSabanShowTime", this, data[t]);
                    }
                    break;
                }

            case "OnFinishMoveToObject":
                {
                    Vector3 temp = (Vector3)param;

                    if (temp.x == pointOfView.x && temp.z == pointOfView.z)
                    {
                        Debug.Log("OnFinishMoveToObject: " + temp);
                        StartCoroutine(ManualPlayContent());
                    }
                    break;
                }

            case "OnEndOfView2D":
                {
                    if (id == (int)param)
                        OnMouseExit();
                    break;
                }

            case "OnMoveToObject":
                {
                    //Debug.Log("OnMoveToObject");
                    if (aSource.isPlaying)
                    {
                        data[0].isCancel = true;
                        data[0].Stop();
                    }

                    break;
                }

            default:
                break;
        }
    }

    public override IEnumerator DownloadData()
    {
        for (int i = 0; i < 3; i++)
            yield return StartCoroutine(DownloadData(i));
    }
}