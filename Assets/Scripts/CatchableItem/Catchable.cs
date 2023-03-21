using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Catchable : MonoBehaviour
{
    public bool IsCatching = false;

    public Vector3 OriginPosition;

    public Quaternion OriginRotation;

    public virtual void SetOrigin()
    {
        OriginPosition = this.transform.localPosition;
        OriginRotation = this.transform.localRotation;
        GetComponent<Rigidbody>().freezeRotation = true;
    }

    public virtual void Init()
    {
        this.transform.localPosition = OriginPosition;
        this.transform.localRotation = OriginRotation;
    }

    public abstract void Catch(GameObject tool);

    public abstract void Release();
}
