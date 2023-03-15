using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour
{
    public GameObject cube;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cube.transform.position = new Vector3(2, 0, 0);
        RaycastHit hit;
        if(Physics.Raycast(this.transform.position, Vector3.right * 3, out hit, 3))
        {
            print(hit.collider.name);
        }
    }
}
