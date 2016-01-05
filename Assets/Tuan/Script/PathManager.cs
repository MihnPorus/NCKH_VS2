using UnityEngine;
using System.Collections;

public class PathManager : MonoBehaviour
{

    public Transform[] items;
    public bool freezee = false;
    public Transform _diemnhin;
    public float SpeedPath=5;
    public Vector3[] GetPathPoints()
    {
        //Debug.Log("PathManager - GetPathPoints()");
        Vector3[] PathV3 = new Vector3[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            PathV3[i] = items[i].position;
        }
        return PathV3;
    }
    public Transform[] GetItems()
    {
        Debug.Log("PathManager - GetItems()");
        return items;
    }
}
