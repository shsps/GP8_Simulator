using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;

public class RemoteAction : MonoBehaviour
{
    [SerializeField] GameObject cube;
    [SerializeField] bool tXNegative = false;
    [SerializeField] private Interactable buttonA;
    public Text txt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //System.Diagnostics.Debug.WriteLine(tXNegative);
        //Debug.Log(tXNegative);
        
        if (tXNegative)
        {
            GameObject.Find("JointS").SendMessage("MoveToolHorizon", 0.001f);
        }
    }

    public void PressingTXN()
    {
        tXNegative = true;
        txt = GameObject.Find("Text").GetComponent<Text>();
        txt.text = "Start";
        print("開始");
    }

    public void ReleaseTXN()
    {
        tXNegative = false;
        txt = GameObject.Find("Text").GetComponent<Text>();
        txt.text = "Stop";
        print("放開");
    }

    public void TranslatePositiveX()
    {
        cube.transform.Translate(.0001f, 0, 0);
    }

    public void TranslateNegativeX()
    {
        cube.transform.Translate(-.0001f, 0, 0);
    }

    public void RotatePositiveX()
    {
        cube.transform.Rotate(1, 0, 0);
    }

    public void RotateNegativeX()
    {
        cube.transform.Rotate(-1, 0, 0);
    }

    public void PrintSomething(string s)
    {
        print(s);
    }
}
