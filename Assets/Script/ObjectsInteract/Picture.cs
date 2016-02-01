using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Picture : Item {

    // Cache dữ liệu đã download
    public PictureData data = null;

    AudioSource audioSource;
    // Thứ tự lộ trình
    public int order;
    // Những mốc thời gian khi phát
    public List<float> time;

    int clickCount = 0;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    IEnumerator Start()
    {
        data = new PictureData();

        data.id = id;
        //yield return StartCoroutine(data.GetModel());

        yield return StartCoroutine(data.GetAudio(1));
        yield return StartCoroutine(data.GetText(1));
        yield return StartCoroutine(data.GetSprites());
        StartCoroutine(DataStorage.Instance.DownloadPicture(this, true));

        // Nhan su kien
        EventManager.Instance.AddListener("OnShowTime", OnEvent);
        EventManager.Instance.AddListener("OnFinishMoveToObject", OnEvent);
        EventManager.Instance.AddListener("OnEndOfView2D", OnEvent);

        EventManager.Instance.AddListener("OnEndOfPictureView", OnEvent);

        EventManager.Instance.AddListener("OnMoveToObject", OnEvent);

        pointOfView = transform.TransformPoint(pointOfView);
        //pointToLook = transform.TransformPoint(pointToLook);
        //Debug.Log(pointOfView);

    }

    public override IEnumerator OnClick()
    {
        if (PlayerPrefs.GetInt("IsAutoMode")==1 || clickCount > 2)
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

    // Download dữ liệu dựa theo id
    public override IEnumerator DownloadData()
    {
        

        // Download am thanh, text, 
        #region DownloadContent

        AssetBundleLoadAssetOperation request = 
            BundleManager.LoadAssetAsync(data.audioBundle[0], data.audioBundle[1], typeof(AudioClip));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);
        data.introAudio = request.GetAsset<AudioClip>();

        //Debug.Log(data.introAudio);

        //request = null;
        request = BundleManager.LoadAssetAsync(data.audioBundle[2], data.audioBundle[3], typeof(AudioClip));
        //Debug.Log(request);
        if (request == null)
            yield break;
        yield return StartCoroutine(request);
        data.detailAudio = request.GetAsset<AudioClip>();

        //Debug.Log(data.detailAudio);
        //=======================

        // Download text================
        Debug.Log(data.textBundle[0]);
        if (!data.textBundle[0].Trim().Equals(""))
        {
            request = BundleManager.LoadAssetAsync(data.textBundle[0], data.textBundle[1], typeof(TextAsset));
            if (request == null)
                yield break;
            yield return StartCoroutine(request);
            //Debug.Log(request.GetAsset<TextAsset>());
            data.text = request.GetAsset<TextAsset>();

            Debug.Log(data.text);
        }
        // ==============================

        // Download sprites================
        
        int size = data.spriteBundle.Length-1;
        for (int i = 0; i < size; i += 2)
        {
            request = BundleManager.LoadAssetAsync(data.spriteBundle[i], data.spriteBundle[i+1], typeof(Sprite));
            if (request == null)
                yield break;
            yield return StartCoroutine(request);
            data.sprites.Add(request.GetAsset<Sprite>());
        }
        // =====================================

        BundleManager.UnloadBundle(data.audioBundle[0]);

        data.imgTime = time;

        #endregion
    }

    // Tự động play 
    IEnumerator AutoPlayContent()
    {
        // Hiệu ứng chờ download thì play ở đây
        if (data.introAudio == null)
        {
            // Hiệu ứng khi chờ download đặt ở đây
            Loader.waitingScreen.SetActive(true);

            yield return StartCoroutine(DataStorage.Instance.DownloadPicture(this, false));

            Loader.waitingScreen.SetActive(false);
        }
        model.GetComponent<Renderer>().material.color = Color.red;
        // Play intro audio
        if (id != 41)
        {
            yield return StartCoroutine(data.PlayAudio(audioSource, true));
        }


        // Nếu không ấn Escape thì tiếp tục show màn hình 2D, còn không thì đi tiếp
        if (!data.isCancel)
            EventManager.Instance.PostNotification("OnPictureClick", this, data);
        else
        {
            EventManager.Instance.PostNotification("OnEndOfView2D", this, id);
        }
    }

    IEnumerator ManualPlayContent()
    {
        // Đổi màu hiện vật
        model.GetComponent<Renderer>().material.color = Color.red;
        if (data.introAudio == null)
        {
            // Hiệu ứng khi chờ download đặt ở đây
            Loader.waitingScreen.SetActive(true);

            yield return StartCoroutine(DataStorage.Instance.DownloadPicture(this, false));

            Loader.waitingScreen.SetActive(false);
        }
        
        clickCount=1;
        IEnumerator routine = data.PlayAudio(audioSource, true);
        StartCoroutine(routine);
        while (clickCount < 2 && !data.isCancel)
        {
            if (!routine.MoveNext())
                break;
            yield return null;
        }
        audioSource.Stop();

        // Sua o day
        if (!data.isCancel) 
        {
            data.Stop();
            Item.isInteractable = false;
           // MoveCharator.isRotatable = false;
            EventManager.Instance.PostNotification("OnPictureManualClick", this, data);
        }
        else if (!data.isCancel && clickCount < 2)
        {
            data.Stop();
            Item.isInteractable = false;
            //MoveCharator.isRotatable = false;
            EventManager.Instance.PostNotification("OnPictureManualClick", this, data);
        }
        clickCount = 0;
    }

    

    public void OnEvent(string eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case "OnShowTime":
                {
                    int temp = (int) param;
                    if (temp == order)
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

            case "OnEndOfView2D":
                {
                    int t = (int)param;
                    //Debug.Log("End view 2D - " + t);
                    if (id == t)
                        model.GetComponent<Renderer>().material.color = Color.white;

                    break;
                }

            case "OnEndOfPictureView":
                {

                    Item.isInteractable = true;
                    break;
                }

            case "OnMoveToObject":
                {
                    //Debug.Log("OnMoveToObject");
                    if (audioSource.isPlaying)
                    {
                        data.isCancel = true;
                        data.Stop();
                        //isMoveToOther = true;
                    }

                    break;
                }

            default:
                break;
        }
    }
}

// 1 gói dữ liệu để phát nội dung
public class PictureData
{
    #region Define const to get data from DB

    static string secretKey = "museum";

    static string getModelURL = "http://" + Loader.url + "GetModel.php?";
    static string getSpriteURL = "http://" + Loader.url + "GetSprite.php?";
    static string getAudioURL = "http://" + Loader.url +  "GetAudio.php?";
    static string getTextURL = "http://" + Loader.url +  "GetText.php?";

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

    // Dùng để kiểm tra xem có ấn escape hay không
    public bool isCancel;
    public bool isStop = false;

    //public bool isDoubleClick;

    #region Variables Storing Data

    public List<Sprite> sprites;
    public AudioClip introAudio;
    public AudioClip detailAudio;
    public TextAsset text;
    public List<float> imgTime;

    #endregion

    public PictureData()
    {
        sprites = new List<Sprite>();
        imgTime = new List<float>();
    }

    public IEnumerator PlayAudio(AudioSource source, bool isIntro)
    {
        isStop = false;
        //isDoubleClick = false;
        if (isIntro)
        {
            isCancel = false;
            source.clip = introAudio;
        }
        else
            source.clip = detailAudio;

        source.Play();

        while (source.isPlaying && !isStop)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                source.Stop();
                isCancel = true;
                yield break;
            }

            yield return null;
        }
    }

    public IEnumerator PlayImage(Image image)
    {
        isStop = false;
        //Debug.Log(sprites.Count);
        float timer = 0;
        int i = 0;
        image.sprite = sprites[0];
        while (i < imgTime.Count && !isStop)
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isCancel = true;
                yield break;
            }
            timer += Time.deltaTime;

            if (timer >= imgTime[i])
            {
                //Debug.Log(sprites[i]);
                //Debug.Log(timer);
                //Debug.Log(imgTime[i]);
                image.sprite = sprites[i];
                i++;
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

    public IEnumerator GetText(int lang)
    {
        string hash = Md5Ultility.Md5Sum(id + secretKey);
        string url = getTextURL + "id=" + id + "&hash=" + hash + "&lang=" + lang;

        WWW www = new WWW(url);
        yield return www;
        if (www.error != null)
        {
            Debug.Log("There was an error posting the high score: " + www.error);
        }
        else
        {
            //Debug.Log(www.text);
            textBundle = www.text.Split('/');
            //Debug.Log(textBundle[0] + " - " + textBundle[1]);
            for (int i = 0; i < textBundle.Length; i++)
            {
                textBundle[i] = textBundle[i].TrimStart().TrimEnd();
            }
        }
    }

    public IEnumerator GetSprites()
    {
        string hash = Md5Ultility.Md5Sum(id + secretKey);
        string url = getSpriteURL + "id=" + id + "&hash=" + hash;

        WWW www = new WWW(url);
        yield return www;
        if (www.error != null)
        {
            Debug.Log("There was an error posting the high score: " + www.error);
        }
        else
        {
            //Debug.Log(www.text);
            spriteBundle = www.text.Split('/');
            for (int i = 0; i < spriteBundle.Length; i++)
            {
                spriteBundle[i] = spriteBundle[i].TrimStart().TrimEnd();
            }
        }
    }
}