using UnityEngine;
using System.Collections;
using DG.Tweening;

public class AICharacterControl : MonoBehaviour
{

    public GameObject targetPicker;

    public static NavMeshAgent agent { get; private set; } // the navmesh agent required for the path finding
    public MoveCharator character { get; private set; } // the character we are controlling
    private Transform target; // target to aim for
    private int count;
    public static bool isEscapable = true;

    int mode;

    // Use this for initialization
    private void Start()
    {
        mode = PlayerPrefs.GetInt("IsAutoMode");
        //Debug.Log(mode);
        Debug.Log("AICharacterControl - Start()");
        // get the components on the object we need ( should not be null due to require component so no need to check )
        agent = GetComponent<NavMeshAgent>();
        character = GetComponent<MoveCharator>();

        agent.updateRotation = false;
        agent.updatePosition = true;
        if (mode != 1) 
        {
            targetPicker.SetActive(true);
        }
        EventManager.Instance.AddListener("OnMoveToObject", OnEvent);

    }
    public void MoveNavMesh(Transform target)
    {
        Debug.Log("AICharacterControl - MoveNavMesh()");
        if (target != null)
        {
            isEscapable = true;
            if (target.tag == "Ground")
            {   
                agent.SetDestination(target.position);// navmesh di chuyen
            }
            
        }
    }

    public void SetTarget(Transform target)
    {
        Debug.Log("AICharacterControl - SetTarget()");
        this.target = target;
        
    }

    IEnumerator MoveToObject(Vector3 pos)
    {
        agent.SetDestination(pos);
        isEscapable = true;
        while (transform.position.x != pos.x || transform.position.z != pos.z)
        {
            //Debug.Log(pos);

            

            yield return null;
        }
        isEscapable = false;
        Debug.Log("Play Event here");
        EventManager.Instance.PostNotification("OnFinishMoveToObject", this, pos);
    }

    public void OnEvent(string eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case "OnMoveToObject":
                {
                    Vector3 pos = (Vector3)param;
                    //Debug.Log("on move to object");
                    //Debug.Log(transform);
                    pos.y = transform.position.y;
                    //Debug.Log(pos.y + " - " + transform.position.y);
                    StartCoroutine(MoveToObject(pos));
                    
                    break;
                }
        }
    }

    void Update()
    {
        if (mode != 1 && isEscapable && Input.GetKeyDown(KeyCode.Escape)) 
        {
            //Debug.Log("Reload");
            EventManager.Instance.RemoveEvent("OnMoveToObject");
            EventManager.Instance.RemoveEvent("OnEndOfView3D", GetComponent<MoveCharator>().OnEvent);
            EventManager.Instance.RemoveEvent("On3DShow", GetComponent<MoveCharator>().OnEvent);
            EventManager.Instance.PostNotification("OnReload", this);
            // Remove su kien cua targetPicker cu
            //Debug.Log(x);
            targetPicker.SetActive(false);
            Destroy(gameObject);
        }
    }
}