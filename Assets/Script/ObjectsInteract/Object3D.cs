using UnityEngine;
using System.Collections;
using System;

public class Object3D : Item {

    public int order;
    public GameObject modelPrefab;

    // Cache dữ liệu
    Object3Ddata data;
    AudioSource source;
    int clickCount = 0;
	// Use this for initialization
	void Start ()
    {
        #region Download Model at the start
        data = new Object3Ddata();
        data.id = id;
        StartCoroutine(data.GetAudio(1));
        data.model = modelPrefab;
        

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
                    if (order == (int) param)
                        StartCoroutine(AutoPlayContent());
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
            //Debug.Log(data.introAudio + " - " + data.detailAudio);
        }
        model.GetComponent<Renderer>().material.color = Color.red;
        yield return StartCoroutine(data.PlayAudio(source, true));
        //data.model.GetComponent<Renderer>().material.color = Color.white;
        EventManager.Instance.PostNotification("On3DShow", this, data);
    }

    IEnumerator ManualPlayContent()
    {
        if (data.introAudio == null)
        {
            yield return StartCoroutine(DownloadData());
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

        //data.model.GetComponent<Renderer>().material.color = Color.white;

        if (!data.isCancel)
            EventManager.Instance.PostNotification("On3DShow", this, data);

        else if (!data.isCancel && clickCount < 2)
            EventManager.Instance.PostNotification("On3DShow", this, data);
        clickCount = 0;
    }

    public override IEnumerator DownloadData()
    {
        yield return StartCoroutine(data.GetAudio(1));

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
    }
}



public class Object3Ddata
{
    #region Define const to get data from DB

    static string secretKey = "museum";

    static string getModelURL = "http://localhost/GetModel.php?";
    static string getSpriteURL = "http://localhost/GetSprite.php?";
    static string getAudioURL = "http://localhost/GetAudio.php?";
    static string getTextURL = "http://localhost/GetText.php?";

    #endregion

    #region variables to store data from DB
    // 0 la bundle name, 1 la assetname
    public string[] modelBundle;
    // 0 la bundle name, 1 la assetname
    public string[] textBundle;
    // 0 la bundle gioi thieu, 1 la asset gioi thieu, 2 la bundle chi tiet, 3 la asset chi tiet
    public string[] audioBundle;
    // Chan la bundle name, le la asset name
    public string[] spriteBundle;

    #endregion

    public int id;
    public bool isCancel;
    public bool isStop = false;

    public GameObject model;
    public AudioClip introAudio;
    public AudioClip detailAudio;

    public IEnumerator PlayAudio(AudioSource source, bool isIntro)
    {
        isStop = false;
        if (isIntro)
        {
            isCancel = false;
            source.clip = introAudio;
        }
        else
            source.clip = detailAudio;

        source.Play();

        while (source.isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !isStop)
            {
                source.Stop();
                isCancel = true;
                yield break;
            }
            yield return null;
        }
    }

    public void Stop()
    {
        isStop = true;
    }

    public IEnumerator GetModel()
    {
        string hash = Md5Ultility.Md5Sum(id + secretKey);
        string url = getModelURL + "id=" + id + "&hash=" + hash;

        WWW www = new WWW(url);
        yield return www;
        if (www.error != null)
        {
            Debug.Log("There was an error posting the high score: " + www.error);
        }
        else
        {
            //Debug.Log(www.text);
            modelBundle = www.text.Split('/');
            //Debug.Log(modelBundle[0] + " - " + modelBundle[1]);
            for (int i = 0; i < modelBundle.Length; i++)
            {
                modelBundle[i] = modelBundle[i].TrimStart().TrimEnd();
            }
        }
    }

    public IEnumerator GetAudio(int lang)
    {
        string hash = Md5Ultility.Md5Sum(id + secretKey);
        string url = getAudioURL + "id=" + id + "&hash=" + hash + "&lang=" + lang;

        WWW www = new WWW(url);
        yield return www;
        if (www.error != null)
        {
            Debug.Log("There was an error posting the high score: " + www.error);
        }
        else
        {
            //Debug.Log(www.text);
            audioBundle = www.text.Split('/');
            //Debug.Log(audioBundle[0] + " - " + audioBundle[1] + " - " + audioBundle[2] + " - " + audioBundle[3]);
            for (int i = 0; i < audioBundle.Length; i++)
            {
                audioBundle[i] = audioBundle[i].TrimStart().TrimEnd();
            }
        }
    }

    IEnumerator StartCoroutine(IEnumerator x)
    {
        while (x.MoveNext()) ;
        yield return null;
    }

    public IEnumerator Download()
    {
        yield return StartCoroutine(GetAudio(1));

        AssetBundleLoadAssetOperation request = BundleManager.LoadAssetAsync(audioBundle[0], audioBundle[1], typeof(AudioClip));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);
        introAudio = request.GetAsset<AudioClip>();



        request = BundleManager.LoadAssetAsync(audioBundle[2], audioBundle[3], typeof(AudioClip));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);
        detailAudio = request.GetAsset<AudioClip>();

        BundleManager.UnloadBundle(audioBundle[0]);
    }
}