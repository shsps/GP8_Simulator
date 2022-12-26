using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GP8Controller : MonoBehaviour
{
    public Animation animationController;
    public Animator animator;
    public GameObject T;
    private readonly Vector3 targetOrigin = new Vector3(2.218f, 1.093f, 0f);
    public GameObject catchObject;
    public GameObject target;
    //public Ray ray;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            animator.Play("Motion1");
            print("press");
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            target.transform.parent = null;
            target.transform.position = targetOrigin;
            target.transform.rotation = Quaternion.Euler(Vector3.zero);
            target.GetComponent<Rigidbody>().useGravity = true;
        }
    }

    public void GrabObject()
    {
        Ray ray = new Ray(T.transform.position, T.transform.right);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "DontCatch") return;
            catchObject = hit.collider.gameObject;
            catchObject.transform.parent = T.transform;
            catchObject.GetComponent<Rigidbody>().useGravity = false;
            print("catch object : " + hit.collider.name);
        }
        
    }

    public void IsCatchObject()
    {
        //StartCoroutine(_IsCatchObject());
        if(catchObject != null)
        {
            animator.Play("Motion2");
        }
    }

    public void PutObject()
    {
        catchObject.transform.parent = null;
        catchObject.GetComponent<Rigidbody>().useGravity = true;
        catchObject = null;
    }
}
