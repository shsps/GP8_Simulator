using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using System.Text;

public class ButtonManager : MonoBehaviour
{
    public PressableButton[] buttonsHoloLens2 = new PressableButton[16];//教導盒
    public PressableButton[] buttonsOfSteps = new PressableButton[2];
    public PressableButton test;
    public GameObject armForTest,steps;
    public bool isPlaying = false;
    [SerializeField] private Material[] ms;
    [SerializeField] private Color o;
    [SerializeField] private Color c = Color.green;
    [SerializeField] private IKManager3D2 ik;
    [SerializeField] private IKManager3D2.OperationMode currentMode;
    //public Microsoft.MixedReality.QR.QRCode qrCode;

    // Start is called before the first frame update
    void Start()
    {
        //armForTest = gameObject.transform.Find("JointS(Scene)").gameObject;
        //教導盒
        foreach (PressableButton b in buttonsHoloLens2)
        {
            b.ButtonPressed.AddListener(() =>
            {
                ButtonPressed(b.GetComponent<MeshRenderer>());//Change Color
                print(b.GetComponent<MeshRenderer>());

                //Below are only once
                if(b.name=="協助")//Grab
                {
                    ik.SearchItemCatchable(false);//Hololens button give false
                }
                if(b.name =="輸入")//Record current info
                {
                    StepManager.instance.AddStep(ik);
                    ik.IsCatchPressed = false;
                    //Record current position
                }
                if(b.name == "清除")
                {
                    ik.init_MoveTool();
                    //Back to the first position
                }
            });
            b.ButtonReleased.AddListener(() =>
            {
                ButtonRelease();//Change Color
            });
        }

        //步驟切換
        foreach (PressableButton b in buttonsOfSteps)
        {
            b.ButtonPressed.AddListener(() =>
            {
                if(b.name == "PreviousStep")
                {
                    StepManager.instance.MoveDirectly(StepManager.changeStepDirection.negative);
                    StepManager.instance.MoveDirectly(StepManager.changeStepDirection.negative);
                }
                if(b.name == "NextStep")
                {
                    StepManager.instance.MoveDirectly(StepManager.changeStepDirection.positive);
                }
            });

            /*if(test.IsPressing)
            {
                string data = "50872";
                byte[] byteArray = Encoding.ASCII.GetBytes(data);
                qrCode = new Microsoft.MixedReality.QR.QRCode().GetRawData(byteArray);
            }*/
            test.ButtonPressed.AddListener(() =>
            {
                armForTest.SetActive(true);
                armForTest.transform.GetChild(1).name = "JointS";
                steps.SetActive(true);
                steps.name = "StepManager";
                //QRTracking.QRCode.test = true;
            });

            test.ButtonReleased.AddListener(() =>
            {

                //QRTracking.QRCode.test = false;
            });
        }

        isPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (ik != GameObject.Find("JointS").GetComponent<IKManager3D2>())
        {
            ik = GameObject.Find("JointS").GetComponent<IKManager3D2>();
        }
        foreach (PressableButton b in buttonsHoloLens2)//Continue
        {
            if(b.IsPressing)
            {
                isPlaying = false;
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
                    case "測試運轉":
                        StepManager.instance.MoveNextSlowly();
                        break;
                    default:
                        print(b.name);
                        break;
                }
            }
        }

        foreach (PressableButton b in buttonsOfSteps)
        {
            if (b.IsPressing)
            {
                isPlaying = true;
            }
        }

        if (isPlaying)
        {
            StepManager.instance.MoveNextSlowly();
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

    public void ButtonToPress()
    {

    }
}
