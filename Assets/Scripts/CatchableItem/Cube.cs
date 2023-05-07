using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// If you want to let robot arm catch a GameObject, you can add this script on a GameObject.
/// If you want to create a catch function personally, you can create a script inherit to Catchable.
/// </summary>
public class Cube : Catchable
{
    //public bool IsCatching { get; set; } = false;

    public override void Catch(GameObject tool)
    {
        this.transform.parent = tool.transform;
        this.GetComponent<Rigidbody>().useGravity = false;
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        IsCatching = true;
    }

    public override void Release()
    {
        this.GetComponent<Rigidbody>().useGravity = true;
        if (ParentGameObject != null)
        {
            this.transform.parent = ParentGameObject.transform;
        }
        else
        {
            this.transform.parent = null;
        }
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        IsCatching = false;
    }

}
