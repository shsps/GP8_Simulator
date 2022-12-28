using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour, ICatchable
{
    public bool IsCatching { get; set; } = false;

    public void Catch(GameObject tool)
    {
        this.transform.parent = tool.transform;
        //this.transform.localPosition = Vector3.zero;
        this.GetComponent<Rigidbody>().useGravity = false;
        IsCatching = true;
    }

    public void Release()
    {
        this.transform.parent = null;
        this.GetComponent<Rigidbody>().useGravity = true;
        IsCatching = false;
    }
}
