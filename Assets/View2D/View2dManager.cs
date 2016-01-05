using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class View2dManager : MonoBehaviour {
    //view 2d, canvas
    public GameObject View2dWindow;

    public Slider uiSliderVolume, uiSliderTime; // slider hien thi thoi gian, chinh volume
    
    public Sprite[] listImage; // list anh thanh phan
    public Image imageDisplay; // khung anh hien thi len man hinh

    public Button btnVolume;   //button volume on/off
    public Sprite btnVolumeImageOn;  //sprite volume ON
    public Sprite btnVolumeImageOff; //sprite volume OFF
    
    // audio input
    AudioSource audio; //audio source input
    private float currentVolume;// gia tri volume hien tai

    int currentImage = 0; // chi so anh hien thoi
    int maxImage; //so luong anh trong 1 khung

    float timeStart=0;
    PictureData recvData;

    void Start()
    {
        //play audio
        audio = GetComponent<AudioSource>();
        //audio.Play();

        //gia tri uislider audio volume
        uiSliderVolume.maxValue = 1 ;
        uiSliderVolume.minValue = 0;
       
        //gia tri uislider time
       
        EventManager.Instance.AddListener("OnPictureManualClick", OnEvent);
        View2dWindow.SetActive(false);
    }

    void Update()
    {
        currentVolume = uiSliderVolume.value ; // lay gia tri volume hien tai tren uiSliderVolume

        //display image and time
        DisplayAudioTime();
        //AutoNextImage();
        
    }

    #region xu li button
    // next anh?
    public void OnClick_BtnNext()
    {
        
        if (currentImage == maxImage - 1)
        {
            //audio.Stop();
            Debug.Log("Tang chi so");
            return;
        }
        currentImage++;
        imageDisplay.sprite = listImage[currentImage];

        

    }

    // previous anh?
    public void OnClick_BtnPrevious()
    {
        
        if (currentImage == 0)
        {
            Debug.Log("het anh");
            currentImage = 0;
            return;
        }
        currentImage--;
        imageDisplay.sprite = listImage[currentImage];
    }

    #region auto display image
    public void AutoNextImage()
    {
        int i = 0;
        while (i < recvData.sprites.Count)
        {
            if (timeStart > recvData.imgTime[currentImage])
            {
                currentImage++;
                imageDisplay.sprite = listImage[currentImage];
            }
            timeStart += Time.deltaTime;
            i++;
        }
    }
    #endregion 

    // exit panel
    public void OnClick_BtnExit()
    {
        audio.time = 0;
        audio.Stop();
        uiSliderTime.value = 0f;
        MoveCharator.isRotatable = true;
        View2dWindow.SetActive(false);
        EventManager.Instance.PostNotification("OnEndOfPictureView", this);
    }


    //button pause
    private bool isToggle = false;
    public void OnClick_BtnPause()
    {
        isToggle = !isToggle;
        if (isToggle == true)
        {
            audio.Pause();
        }
        else
        {
            audio.Play();
        }
    }


    //button replay
    public void OnClick_BtnReplay()
    {
        audio.Stop();
        audio.time = 0;
        audio.Play();
    }


    //button volume, an vao se bat tat
    private bool isToggleVolume = false;
    public void OnClick_BtnVolume()
    {
        //mute
        isToggleVolume = !isToggleVolume;
        if (isToggleVolume == true)
        {
            //thay hinh anh hien thi = nut off
            //gan gia tri volume = 0
            btnVolume.image.sprite = btnVolumeImageOff;
            audio.volume = 0;
        }
            //resume volume
        else
        {
            //thay hinh anh hien thi = nut on
            //gan gia tri volume = gia tri hien thoi tren slider
            btnVolume.image.sprite = btnVolumeImageOn;
            audio.volume = currentVolume;
        }
    }



    //button zoom in
    //vector tang giam chi so zoom
    private Vector3 zoomVector = new Vector3(0.1f, 0.1f, 0f); //chi so tang giam khi zoom
    private int zoomCount=0; // so lan bam zoom co the bam
    private int maxZoomCount = 3; // so lan bam co the zoom nhieu nhat
    public void OnClick_BtnZoomIn()
    {
        
        if (zoomCount <=3)
        {
            zoomCount++;
            imageDisplay.rectTransform.localScale += zoomVector;
        }
       
    }


    // button zoom out
    public void OnClick_BtnZoomOut()
    {

        if (zoomCount >= -3)
        {
            zoomCount--;
            imageDisplay.rectTransform.localScale -= zoomVector;

        }
        
    }

    #endregion

   
    #region audio slider control volume, time
    
    //chinh am luong volume = slide
    //done

    //hien thi thoi gian audio tren slider done
    public void DisplayAudioTime()
    {
        //gan gia tri cua audio time = sliderTime value
      
        //Debug.Log(" value " + uiSliderTime.value + " max value " + uiSliderTime.maxValue);
        float x = uiSliderTime.value + 0.001f;
        if ((x < uiSliderTime.maxValue))
        {
            //Debug.Log(audio.time);
            uiSliderTime.value = audio.time;
            //Debug.Log(" value " + (int)uiSliderTime.value + " max value " + (int)uiSliderTime.maxValue);
        }
            
        //khi gia tri dat den max, stop audio
        else// if (uiSliderTime.value >= uiSliderTime.maxValue)
        {
            Debug.Log("Auto shutdown!");
            OnClick_BtnExit();
            //View2dWindow.SetActive(false);
            
        }
    }

    #endregion

    public void OnEvent(string eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case "OnPictureManualClick":
                {
                    recvData = (PictureData)param;
                    if (recvData != null)
                    {
                        listImage = new Sprite[recvData.sprites.Count];
                        for (int i = 0; i < recvData.sprites.Count; i++)
                        {
                            listImage[i] = recvData.sprites[i];
                        }
                        maxImage = recvData.sprites.Count;
                        Debug.Log("max image " + maxImage);
                        View2dWindow.SetActive(true);
                        audio.clip = recvData.detailAudio;

                        uiSliderTime.minValue = 0;
                        uiSliderTime.maxValue = audio.clip.length;
                        //Debug.Log(audio.clip); 
                        audio.Play();
                        imageDisplay.sprite = listImage[0];
                        //StartCoroutine(AutoNextImage());
                    }
                    break;
                }

            default:
                break;
        }
    }

}


