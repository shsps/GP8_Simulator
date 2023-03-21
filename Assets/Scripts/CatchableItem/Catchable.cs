using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Catchable : MonoBehaviour
{
    public bool IsCatching = false;

    public Vector3 OriginPosition;

    public Quaternion OriginRotation;

    public GameObject ParentGameObject;

    public virtual void SetOrigin()
    {
        OriginPosition = this.transform.position;
        OriginRotation = this.transform.rotation;
        GetComponent<Rigidbody>().freezeRotation = true;
    }

    public virtual void Init()
    {
        this.transform.position = OriginPosition;
        this.transform.rotation = OriginRotation;
    }

    public abstract void Catch(GameObject tool);

    public abstract void Release();
}
