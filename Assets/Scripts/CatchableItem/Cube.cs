using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : Catchable
{
    //public bool IsCatching { get; set; } = false;

    public override void Catch(GameObject tool)
    {
        this.transform.parent = tool.transform;
        this.GetComponent<Rigidbody>().useGravity = false;
        IsCatching = true;
    }

    public override void Release()
    {
        if(ParentGameObject != null)
        {
            this.transform.parent = ParentGameObject.transform;
        }
        else
        {
            this.transform.parent = null;
        }
        this.GetComponent<Rigidbody>().useGravity = true;
        IsCatching = false;
    }
}
