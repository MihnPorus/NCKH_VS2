using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MoveCharator : MonoBehaviour
{
    private View2dManager view2d;
    public static bool isRotatable = true;
    public bool onStart = false;
    float yRotation = 0;
    private int count = 0;
    private bool checkDirection = false;
    private bool checkCount = false;
    private bool checkMove = true;
    private bool checkPause = false;
    private bool checkEsc = false;
    private bool lookCamera = false;
    public Transform cameMain;
    public AudioClip firstClip;

    private AudioSource aSource;

    //Di chuyen tu do
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float MinimumX = -60F;
    public float MaximumX = 60F;
    public bool smooth;
    public float smoothTime = 5f;


    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    void Start()
    {

        SetValueRotateView();
        StartCoroutine(Auto());


        EventManager.Instance.AddListener("OnEndOfView2D", OnEvent);
        EventManager.Instance.AddListener("On3DShow", OnEvent);
        EventManager.Instance.AddListener("OnEndOfView3D", OnEvent);
        EventManager.Instance.AddListener("OnEndOfPictureView", OnEvent);

        aSource = GetComponent<AudioSource>();
        aSource.clip = firstClip;
        aSource.Play();
    }
     public void SetValueRotateView()
    {
        m_CharacterTargetRot = transform.localRotation;
        m_CameraTargetRot = cameMain.localRotation;
    }

    IEnumerator Auto()
    {
        while (true)
        {
            //kiem tra bien checkSitemap, khong cho phep xoay
            if (PlayerPrefs.GetInt("checkSiteMap") != 0)
                RotateView();
            yield return null;
        }
    }
    //}
    /// <summary>
    /// Di chuyển tự động đến object
    /// </summary>
    public void StartMove()
    {

        Debug.Log("MoveCharater-StartMove()");
        StartCoroutine(Move());




    }
    public IEnumerator Pause()
    {

        while (!checkPause)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Reload");
                EventManager.Instance.RemoveEvent("OnMoveToObject");
                EventManager.Instance.RemoveEvent("OnEndOfView3D", OnEvent);
                EventManager.Instance.RemoveEvent("On3DShow", OnEvent);
                EventManager.Instance.PostNotification("OnReload", this);
                Destroy(gameObject);
            }

            yield return null;
        }
        // Duc: 
        EventManager.Instance.PostNotification("OnShowTime", this, count);
        Debug.Log(count);
        Debug.Log("MoveCharater-Pause()");
        //while (!checkEsc)
        //{
        //    if (Input.GetKey(KeyCode.Escape))
        //    {
        //        checkEsc = true;
        //    };
        //    yield return null;
        //}

    }
    public IEnumerator Move()
    {
        Debug.Log("Hoan thanh lo trinh!!! kakaka");
        yield return null;
       
    }
    
    /// <summary>
    /// Dùng chuột phải quay đối tượng
    /// </summary>
    public void RotateView()
    {

        if (Input.GetMouseButton(1) && isRotatable)
        {

            float yRot = Input.GetAxis("Mouse X") * XSensitivity;
            float xRot = Input.GetAxis("Mouse Y") * YSensitivity;

            m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (smooth)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                cameMain.localRotation = Quaternion.Slerp(cameMain.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                transform.localRotation = m_CharacterTargetRot;
                cameMain.localRotation = m_CameraTargetRot;
            }

        }
    }
    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    public void OnEvent(string eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case "OnEndOfView2D":
                {
                    Item.isInteractable = true;
                    AICharacterControl.isEscapable = true;
                    isRotatable = true;
                    checkEsc = true;
                    break;
                }
            case "On3DShow":
                {
                    isRotatable = false;
                    cameMain.gameObject.SetActive(false);
                    break;
                }

            case "OnEndOfView3D":
                {
                    cameMain.gameObject.SetActive(true);
                    isRotatable = true;
                    Item.isInteractable = true;
                    AICharacterControl.isEscapable = true;

                    
                    
                    checkEsc = true;
                    break;
                }

            case "OnEndOfPictureView":
                {

                    AICharacterControl.isEscapable = true;

                    break;
                }

            
            default:
                break;
        }
    }
}
