using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour
{
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(new Vector3(0, 0.01f, 0));
    }
}
