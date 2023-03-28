using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour
{
    public GameObject cube;
    void Start()
    {
        print(this.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = cube.transform.position + new Vector3(0, 0, 3);
        
    }
}
