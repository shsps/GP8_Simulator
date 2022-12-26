using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        print(this.name + " be clidked");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        print(this.name + " up");
    }
}
