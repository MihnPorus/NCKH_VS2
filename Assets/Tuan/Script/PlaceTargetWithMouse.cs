using System;
using UnityEngine;


public class PlaceTargetWithMouse : MonoBehaviour
{
    public float surfaceOffset = 0.1f;
    public GameObject setTargetOn;

    // Update is called once per frame
    

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)&&Input.GetKey(KeyCode.LeftShift))
        {
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