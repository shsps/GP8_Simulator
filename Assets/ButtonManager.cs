using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class ButtonManager : MonoBehaviour
{
    public PressableButton[] buttonsHoloLens2 = new PressableButton[15];
    [SerializeField] private Material[] ms;
    [SerializeField] private Color o;
    [SerializeField] private Color c = Color.green;
    [SerializeField] private IKManager3D2 ik;
    [SerializeField] private IKManager3D2.OperationMode currentMode;

    // Start is called before the first frame update
    void Start()
    {
        if (ik != GameObject.Find("JointS").GetComponent<IKManager3D2>())
        {
            ik = GameObject.Find("JointS").GetComponent<IKManager3D2>();
        }
        foreach (PressableButton b in buttonsHoloLens2)
        {
            b.ButtonPressed.AddListener(() =>
            {
                ButtonPressed(b.GetComponent<MeshRenderer>());//Change Color
                print(b.GetComponent<MeshRenderer>());
                if(b.name=="協助")//Grab
                {
                    ik.SearchItemCatchable();
                }
                if(b.name =="輸入")//Record current info
                {
                    currentMode = ik.mode;//Record current mode
                    print(currentMode);
                    //Record current position
                }
            });
            b.ButtonReleased.AddListener(() =>
            {
                ButtonRelease();//Change Color
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (PressableButton b in buttonsHoloLens2)
        {
            if(b.IsPressing)
            {
                switch (b.name)
                {
                    case "L.X+":
                        ik.MoveToolX(0.0005f);
                        break;
                    case "L.X-":
                        ik.MoveToolX(-0.0005f);
                        break;
                    case "L.Y+":
                        ik.MoveToolY(-0.002f);//up
                        break;
                    case "L.Y-":
                        ik.MoveToolY(0.002f);//down
                        break;
                    case "L.Z+":
                        ik.MoveToolZ(0.002f);//forward
                        break;
                    case "L.Z-":
                        ik.MoveToolZ(-0.002f);//back
                        break;
                    /*case "R.X+":

                        break;
                    case "R.X-":
                        //ik
                        break;
                    case "R.Y+":

                        break;
                    case "R.Y-":

                        break;
                    case "R.Z+":

                        break;
                    case "R.Z-":

                        break;*/
                    /*case "協助":
                        //ik.SearchItemCatchable();//Grab
                        break;*/
                    case "輸入":
                        //Remember
                        break;
                    default:
                        print(b.name);
                        break;
                }
            }
        }
    }

    public void ButtonPressed(MeshRenderer mesh)
    {
        ms = mesh.materials;
        o = ms[0].color;
        ms[0].color = c;
    }

    public void ButtonRelease(/*MeshRenderer mesh*/)
    {
        ms[0].color = o;
    }

    public void ButtonPressedOnce()
    {

    }
}
