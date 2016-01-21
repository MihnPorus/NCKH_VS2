using System;
using UnityEngine;


public class PlaceTargetWithMouse : MonoBehaviour
{
    public float surfaceOffset = 0.1f;
    public GameObject setTargetOn;
    //public View2dManager view2d;
    // Update is called once per frame


    private void Update()
    {
        //kiem tra bien checkSitemap
        if (PlayerPrefs.GetInt("checkSiteMap") ==0) return;
        if (Input.GetMouseButtonDown(0)&&Input.GetKey(KeyCode.LeftShift))
        {
            AICharacterControl.agent.enabled = true;
            Debug.Log("PlaceTargetWithMouse - GetMouseButtonDown()");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Ground")
                {
                    transform.position = hit.point + hit.normal * surfaceOffset;
                    if (setTargetOn != null)
                    {
                        transform.tag = "Ground";
                        setTargetOn.SendMessage("MoveNavMesh", transform);
                    }
                }

            }
        }


    }
}