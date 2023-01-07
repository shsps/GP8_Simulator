using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class RemoteAction : MonoBehaviour
{
    [SerializeField] GameObject cube;
    [SerializeField] bool tXNegative = false;
    [SerializeField] private PressableButtonHoloLens2 buttonTXN;
    [SerializeField] private IKManager3D2 ik;
    public Text txt;
    // Start is called before the first frame update
    void Start()
    {
        /*buttonTXN.ButtonPressed.AddListener(() =>
            {
                tXNegative = true;
                //ik.MoveToolHorizon(0.001f);
            });
        buttonTXN.ButtonReleased.AddListener(() =>
            {
                tXNegative = false;
            });*/
        txt.text = "X-S- X+S+:�ux�b��V���u����\nY - L - Y + L +:�uy�b��V���u����\nZ - U - Z + U +:�uz�b��V���u����\n��U:��������\n��J:������e�y��\n���չB��:�̾ڿ�J�������ǹB��ܤU�ӰO���I";
    }

    // Update is called once per frame
    void Update()
    {
        /*if(ik == null)
        {
            ik = GameObject.Find("JointS").GetComponent<IKManager3D2>();
        }*/
        /*if (/*tXNegative//buttonTXN.IsPressing)
        {
            ik.MoveToolZ(0.001f);
        }*/
    }

    public void PrintSomething(string s)
    {
        print(s);
    }
}
