using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MoveCharator : MonoBehaviour
{
    private View2dManager view2d;
    public static bool isRotatable = true;
    public bool onStart = false;
    /// <summary>
    /// sử dụng biến kiểu WaypointManager truy suất các value, function
    /// </summary>
    public WaypointManager waypointManager;
    private PathManager[] pathContainer;
    /// <summary>
    /// speed value
    /// </summary>
    public float _speedmove = 10;
    public float _speedrotation=5;
    /// <summary>
    /// tốc độ ban đầu
    /// </summary>
    private float originSpeed;
    /// <summary>
    /// điểm xoay về hướng di chuyển
    /// </summary>
    private Vector3 _diemxoay;
    /// <summary>
    /// path type: thẳng or cong.
    /// <summary>
    public DG.Tweening.PathType pathType = DG.Tweening.PathType.CatmullRom;
    /// <summary>
    /// định hướng trục của player. default: full3D
    /// <summary>
    public DG.Tweening.PathMode pathMode = DG.Tweening.PathMode.Full3D;

    float yRotation = 0;
    private int count = 0;
    private bool checkDirection = false;
    private bool checkCount = false;
    private bool checkMove = true;
    private bool checkPause = false;
    private bool checkEsc = false;
    private bool lookCamera = false;
    public Transform cameMain;
    public GameObject cubecamera;
    public AudioClip firstClip;
    public AudioClip lastClip;
    public AudioClip thuyenToBiaClip;

    private AudioSource aSource;

    //Di chuyen tu do
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public bool smooth;
    public float smoothTime = 5f;


    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    void Start()
    {
        m_CharacterTargetRot = transform.localRotation;
        m_CameraTargetRot = cubecamera.transform.localRotation;
        if (PlayerPrefs.GetInt("IsAutoMode") == 1)
        {
            Invoke("StartMove", 1);
        }
        else
            StartCoroutine(Auto());

        
        EventManager.Instance.AddListener("OnEndOfView2D", OnEvent);
        EventManager.Instance.AddListener("On3DShow", OnEvent);
        EventManager.Instance.AddListener("OnEndOfView3D", OnEvent);
        EventManager.Instance.AddListener("OnEndOfPictureView", OnEvent);

        aSource = GetComponent<AudioSource>();
        aSource.clip = firstClip;
        aSource.Play();
    }

    IEnumerator Auto()
    {
        while (true)
        {
            //kiem tra bien checkSitemap, khong cho phep xoay
            if(PlayerPrefs.GetInt("checkSiteMap") !=0)
            RotateView();
            yield return null;
        }
    }

    //void Update()
    //{
    //    if (PlayerPrefs.GetInt("IsAutoMode") == 0)
    //    {
    //        RotateView();
    //    }


    //}
    /// <summary>
    /// Di chuyển tự động đến object
    /// </summary>
    public void StartMove()
    {

        Debug.Log("MoveCharater-StartMove()");
        if (pathContainer == null) SetPathContainer(waypointManager.PathWp);
        StartCoroutine(Move());




    }
    public IEnumerator Pause()
    {

        while (!checkPause)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) 
            {
                Debug.Log("Reload");
                EventManager.Instance.PostNotification("OnReload", this);
                EventManager.Instance.RemoveEvent("OnMoveToObject");
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
        Debug.Log("MoveCharater-Move()");
        while (count < pathContainer.Length-2)
        {

            while (checkMove)
            {
                checkEsc = false;
                checkMove = false;
                if (pathContainer[count].freezee)
                {
                    _diemxoay = waypointManager.PathWaypoint(count);
                    StartCoroutine(RotateDirection(pathContainer[count]._diemnhin.position, _diemxoay,pathContainer[count].freezee));
                }
                else
                {
                    
                    if (pathContainer[count].items.Length == 2)
                    {
                        _diemxoay = waypointManager.PathWaypoint(count);
                        StartCoroutine(MovePathWaypoint(pathContainer[count]._diemnhin.position, count));
                    }
                    else
                    {
                        _diemxoay = waypointManager.PathWaypoint(count);
                        StartCoroutine(RotateDirection(_diemxoay));
                        StartCoroutine(MovePathWaypoint(count));
                    }
                }
                StartCoroutine(Pause());

            }
            if (checkEsc)
            {
                count++;
                checkMove = true;
                checkDirection = false;
                checkPause = false;
            }
            yield return null;
        }
        while (count < pathContainer.Length )
        {

            while (checkMove)
            {
                checkEsc = false;
                checkMove = false;
                
                _diemxoay = waypointManager.PathWaypoint(count);
                StartCoroutine(MovePathWaypoint(pathContainer[count]._diemnhin.position, count));
                

            }
            if (checkPause)
            {
                checkEsc = true;
            }
            if (checkEsc)
            {
                count++;
                checkMove = true;
                checkDirection = false;
                checkPause = false;
            }
            yield return null;
        }
        Debug.Log("Hoan thanh lo trinh!!! kakaka");
        aSource.clip = lastClip;
        aSource.Play();

        while (aSource.isPlaying)
            yield return null;

        EventManager.Instance.PostNotification("OnReload", this);
        Destroy(gameObject);
    }
    public IEnumerator MovePathWaypoint(Vector3 _diemnhin, int count)
    {

        Debug.Log("MoveCharater-MovePathWaypoint(Vector3,int)");
        Transform _endpoint = pathContainer[count].items[pathContainer[count].items.Length - 1];

        transform.DOPath(pathContainer[count].GetPathPoints(), pathContainer[count].SpeedPath, pathType, pathMode, 5, new Color(1, 0, 0, 0.5f));

        Vector3 temp = Vector3.up;
        while (_endpoint.position != transform.position )
        {
            Vector3 Direction = _diemnhin - cubecamera.transform.position;
            Quaternion rotation = Quaternion.LookRotation(Direction);
            temp = cubecamera.transform.forward;
            cubecamera.transform.rotation = Quaternion.Slerp(cubecamera.transform.rotation, rotation, Time.deltaTime * _speedrotation);
            yield return null;
        }
        checkPause = true;
    }
    public IEnumerator MovePathWaypoint(int count)
    {

        while (!checkDirection)
        {
            yield return null;
        }
        Debug.Log("MoveCharater-MovePathWaypoint(int)");
        Transform _endpoint = pathContainer[count].items[pathContainer[count].items.Length - 1];
        transform.DOPath(pathContainer[count].GetPathPoints(), 10, pathType, pathMode, 5, new Color(1, 0, 0, 0.5f)).SetLookAt(0); // vua di vua nhin duong thong qua setlooat(0);
        while (_endpoint.position != transform.position)
        {
            yield return null;
        }
        if (count < pathContainer.Length - 2)
            checkPause = true;
        else checkEsc = true;
    }


    public IEnumerator RotateDirection(Vector3 _diemnhin, Vector3 _diemxoay)
    {
        Debug.Log("MoveCharater-RotateDirection(Vector3,Vector3)");

        Vector3 temp = Vector3.up; //biến xác định điều kiện chạy của function
        Vector3  cbeDirection; // biến xác định hướng của this và cubecamera;
        Quaternion  cbeRotation; //biến xác định góc quay của this và cubecamra;

        cbeDirection = _diemnhin - cubecamera.transform.position;

        cbeRotation = Quaternion.LookRotation(cbeDirection);

        while (cubecamera.transform.forward != temp)
        {
            temp = cubecamera.transform.forward;
            cubecamera.transform.rotation = Quaternion.Slerp(cubecamera.transform.rotation, cbeRotation, Time.deltaTime * _speedrotation);// xoay nhin vao doi tuong _ xoay cube(camera)
            yield return null;
        }
        checkDirection = true;
    }
    public IEnumerator RotateDirection(Vector3 _diemxoay)
    {
        Debug.Log("MoveCharater-RotateDirection(Vector3)");

        Vector3 temp = Vector3.up;
        Vector3 temp2 = Vector3.up;//biến xác định điều kiện chạy của function
        Vector3 trfDirection; // biến xác định hướng của this và cubecamera;
        Quaternion trfRotation; //biến xác định góc quay của this và cubecamra;

        trfDirection = _diemxoay - this.transform.position;
        //cbeDirection = (_diemxoay + new Vector3(0,6.63f,0)) - cubecamera.transform.position;

        trfRotation = Quaternion.LookRotation(trfDirection);
        //cbeRotation = Quaternion.LookRotation(cbeDirection);

        while (cubecamera.transform.forward != temp2)
        {
            temp = transform.forward;
            temp2 = cubecamera.transform.forward;
            transform.rotation = Quaternion.Slerp(transform.rotation, trfRotation, Time.deltaTime * 1);
            cubecamera.transform.rotation = Quaternion.Slerp(cubecamera.transform.rotation, transform.rotation, Time.deltaTime * _speedrotation);
            yield return null;
        }
        checkDirection = true;
    }
    public IEnumerator RotateDirection(Vector3 _diemnhin, Vector3 _diemxoay, bool freezee)
    {
        Debug.Log("MoveCharater-RotateDirection(Vector3,Vector3)");

        Vector3 temp = Vector3.up; //biến xác định điều kiện chạy của function
        Vector3 trfDirection, cbeDirection; // biến xác định hướng của this và cubecamera;
        Quaternion trfRotation, cbeRotation; //biến xác định góc quay của this và cubecamra;

        trfDirection = _diemxoay - this.transform.position;
        cbeDirection = _diemnhin - cubecamera.transform.position;

        trfRotation = Quaternion.LookRotation(trfDirection);
        cbeRotation = Quaternion.LookRotation(cbeDirection);

        while (cubecamera.transform.forward != temp)
        {
            temp = cubecamera.transform.forward;
            //transform.rotation = Quaternion.Slerp(transform.rotation, trfRotation, Time.deltaTime * 1);
            cubecamera.transform.rotation = Quaternion.Slerp(cubecamera.transform.rotation, cbeRotation, Time.deltaTime * _speedrotation);
            yield return null;
        }
        //checkDirection = true;
        checkPause = true;
    }
    public IEnumerator RotateDirectionEnd(Vector3 _diemnhin, Vector3 _diemxoay, bool freezee)
    {
        Debug.Log("MoveCharater-RotateDirection(Vector3,Vector3)");

        Vector3 temp = Vector3.up; //biến xác định điều kiện chạy của function
        Vector3 trfDirection, cbeDirection; // biến xác định hướng của this và cubecamera;
        Quaternion trfRotation, cbeRotation; //biến xác định góc quay của this và cubecamra;

        trfDirection = _diemxoay - this.transform.position;
        cbeDirection = _diemnhin - cubecamera.transform.position;

        trfRotation = Quaternion.LookRotation(trfDirection);
        cbeRotation = Quaternion.LookRotation(cbeDirection);

        while (cubecamera.transform.forward != temp)
        {
            temp = cubecamera.transform.forward;
            //transform.rotation = Quaternion.Slerp(transform.rotation, trfRotation, Time.deltaTime * 1);
            cubecamera.transform.rotation = Quaternion.Slerp(cubecamera.transform.rotation, cbeRotation, Time.deltaTime * _speedrotation);
            yield return null;
        }
        checkEsc = true;
    }
    public void SetPathContainer(PathManager[] pathContainer)
    {
        Debug.Log("MoveCharator - SetPathContainer()");
        this.pathContainer = pathContainer;
    }

    public void AtmMove(Vector3[] wpPos)
    {
        originSpeed = _speedmove;
        transform.DOPath(wpPos, originSpeed, pathType, pathMode, 5, new Color(1, 0, 0, 0.5f)).SetLookAt(0);
    }
    /// <summary>
    /// Dùng chuột phải quay đối tượng
    /// </summary>
    public void RotateView()
    {

        if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftShift) && isRotatable)
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
                cubecamera.transform.localRotation = Quaternion.Slerp(cubecamera.transform.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                transform.localRotation = m_CharacterTargetRot;
                cubecamera.transform.localRotation = m_CameraTargetRot;
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
                    
                    int t = (int)param;
                    if (t == 39 && PlayerPrefs.GetInt("IsAutoMode") == 1)
                    {
                        aSource.clip = thuyenToBiaClip;
                        aSource.Play();
                    }
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
