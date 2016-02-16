using UnityEngine;
using System.Collections;

public class ObjectRotator : MonoBehaviour {

    Transform trans;

    void Start()
    {
        trans = transform;
    }
	
	// Update is called once per frame
	void Update () {
        if (!gameObject.activeInHierarchy)
            return;
        trans.Rotate(0, 20 * Time.deltaTime, 0);
    }
}