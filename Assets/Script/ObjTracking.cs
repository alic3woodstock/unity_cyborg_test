using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjTracking : MonoBehaviour
{

    public GameObject parentObj;
    public GameObject childObj;
    public Vector3 offset;
    public Vector3 debug;
    public Vector3 debug2;
    //0.16 -1.7 -0.04


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float posY = (Mathf.Round(parentObj.transform.position.y * 10000) / 10000);
        Vector3 position =  new Vector3(
            parentObj.transform.position.x, posY,
            parentObj.transform.position.z);
        parentObj.transform.position = position;
        childObj.transform.position = position - offset;
        debug = parentObj.transform.position;
        debug2 = childObj.transform.position;

    }
}
