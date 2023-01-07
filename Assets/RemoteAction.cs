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
        txt.text = "X-S- X+S+:沿x軸方向直線移動\nY - L - Y + L +:沿y軸方向直線移動\nZ - U - Z + U +:沿z軸方向直線移動\n協助:夾取物件\n輸入:紀錄當前座標\n測試運轉:依據輸入紀錄順序運行至下個記錄點";
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
