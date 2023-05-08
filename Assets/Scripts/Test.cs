using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour
{
    public GameObject g1;
    public GameObject g2;
    public GameObject g3;
    void Start()
    {
        //this.transform.parent = cube.transform;
        g3.transform.parent = g1.transform;
        g3.transform.localPosition = new Vector3(0, 0.0003406125f, 0);
        g3.transform.parent = g2.transform;
        //this.transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
