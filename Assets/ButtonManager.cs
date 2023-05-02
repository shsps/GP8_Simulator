using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using System.Text;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public PressableButton[] buttonsHoloLens2 = new PressableButton[16];//�оɲ�
    public GameObject teachingBoxButtonGroup;
    public List<GameObject> buttons = new List<GameObject>();
    public List<GameObject> tutorialBtn = new List<GameObject>();
    public List<Color> tutorialBtnMaterials = new List<Color>();
    public List<GameObject> grabBtn = new List<GameObject>();
    public List<Color> grabBtnMaterials = new List<Color>();
    public PressableButton[] buttonsOfSteps = new PressableButton[4];
    public int actionType = 0;
    public GameObject circle, cube;
    int x = 0;
    int showResetFirstBtn = 0;
    [SerializeField] int y = 0;
    public PressableButton test;
    public PressableButton reset;
    public GameObject teachingText;
    public GameObject armForTest,steps;
    public bool isPlaying = false;
    [SerializeField] private Material[] ms;
    [SerializeField] private Color o;
    [SerializeField] private Color c = Color.green;
    [SerializeField] private IKManager3D2 ik;

    // Start is called before the first frame update
    void Start()
    {
        
        for(int i = 0; i<teachingBoxButtonGroup.transform.childCount;i++)//All buttons in teaching box
        {
            GameObject btn = teachingBoxButtonGroup.transform.GetChild(i).gameObject;
            if (btn.tag=="Tutorial"||btn.tag =="Both")
            {
                tutorialBtn.Add(btn);
                tutorialBtnMaterials.Add(btn.GetComponent<MeshRenderer>().materials[0].color);
            }
            if(btn.tag == "Grab"||btn.tag=="Both")
            {
                grabBtn.Add(btn);
                grabBtnMaterials.Add(btn.GetComponent<MeshRenderer>().materials[0].color);
            }
        }
        foreach (PressableButton b in buttonsHoloLens2)
        {
            b.ButtonPressed.AddListener(() =>
            {
                ButtonPressed(b.GetComponent<MeshRenderer>());//Change Color
                print(b.GetComponent<MeshRenderer>());

                //Below are only once
                if(b.name=="��U")//Grab
                {
                    ik.SearchItemCatchable(false);//Hololens button give false
                }
                if(b.name =="��J")//Record current info
                {
                    StepManager.instance.AddStep(ik);
                    ik.IsCatchPressed = false;
                    //Record current position
                }
                if(b.name == "�M��")
                {
                    ik.init_MoveTool();
                    //Back to the first position
                }
            });
            b.ButtonReleased.AddListener(() =>
            {
                ButtonRelease(b.GetComponent<MeshRenderer>());//Change Color
            });
        }

        //�B�J����
        foreach (PressableButton b in buttonsOfSteps)
        {
            b.ButtonPressed.AddListener(() =>
            {
                if (b.name == "PreviousActionType")
                {
                    if (y == 0)
                    {
                        print("reset");
                        StepManager.instance.ResetRobotArm();
                        y++;
                    }
                    actionType--;
                    if (actionType < 0) actionType = 0;
                    ResetBtns();
                    if(actionType==0)
                    {
                        GameObject.Find("StepManager").GetComponent<StepManager>().step = 0;
                        ButtonToPress();
                        GameObject.Find("StepManager").GetComponent<StepManager>().step = 1;
                        for(int i =0; i< 6; i++)
                        {
                            try
                            {
                                teachingText.GetComponent<TutorialBoard>().ChangeTextOrder(TutorialBoard.ChangeOrderDirection.Previous);
                            }
                            catch
                            {

                            }
                        }
                    }
                    StepManager.instance.ChangeStepOrder(-1);
                    ButtonToPress();
                    teachingText.GetComponent<TutorialBoard>().ChangeTextArrayOrder(TutorialBoard.ChangeOrderDirection.Previous);
                    teachingText.GetComponent<TutorialBoard>().ChangeTitle();
                    y = 0;
                }
                if (b.name == "NextActionType")
                {
                    actionType++;
                    ResetBtns();
                    if (actionType > 2/*�ʧ@�ռ�*/) actionType = 2;
                    if (actionType != 2)
                    {
                        StepManager.instance.MoveDirectly(StepManager.changeStepDirection.positive);
                        if (y == 0)
                        {
                            print(ik.catchStatusNow.ToString());
                            StepManager.instance.ResetRobotArm();
                            y++;
                        }
                    }
                    else
                    {
                        if(y==0)
                        {
                            StepManager.instance.ResetRobotArm();
                            y++;
                        }
                    }
                    StepManager.instance.ChangeStepOrder(1);
                    ButtonToPress();
                    teachingText.GetComponent<TutorialBoard>().ChangeTextArrayOrder(TutorialBoard.ChangeOrderDirection.Next);
                    teachingText.GetComponent<TutorialBoard>().ChangeTitle();
                    y = 0;
                }
                if(b.name == "PreviousStep")
                {
                    teachingText.GetComponent<TutorialBoard>().ChangeTextOrder(TutorialBoard.ChangeOrderDirection.Previous);
                    StepManager.instance.MoveDirectly(StepManager.changeStepDirection.negative);
                    StepManager.instance.MoveDirectly(StepManager.changeStepDirection.negative);
                    ButtonToPress();
                }
                if(b.name == "NextStep")
                {
                    teachingText.GetComponent<TutorialBoard>().ChangeTextOrder(TutorialBoard.ChangeOrderDirection.Next);
                    StepManager.instance.MoveDirectly(StepManager.changeStepDirection.positive);
                    ButtonToPress();
                }
            });

            test.ButtonPressed.AddListener(() =>
            {
                armForTest.SetActive(true);
                armForTest.transform.GetChild(1).name = "JointS";
                steps.SetActive(true);
                steps.name = "StepManager";
                circle.SetActive(true);
                cube.SetActive(true);
            });
            reset.ButtonPressed.AddListener(() =>
            {
                StepManager.instance.ResetRobotArm();
                for (int i = 0; i <= 10; i++)
                {
                    try
                    {
                        teachingText.GetComponent<TutorialBoard>().ChangeTextOrder(TutorialBoard.ChangeOrderDirection.Previous);
                    }
                    catch
                    {

                    }
                }
                ResetBtns();
                if (showResetFirstBtn == 0)
                {
                    GameObject.Find("StepManager").GetComponent<StepManager>().step = 0;
                    ButtonToPress();
                    StepManager.instance.MoveDirectly(StepManager.changeStepDirection.negative);
                    showResetFirstBtn++;
                }
                showResetFirstBtn = 0;
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
        if(actionType!=2)
        {
            StepManager.instance.MoveNextSlowly();//�ھڳ]�w��ơA�b�ɶ������ܫ��w��m
        }
        if(x==0)
        {
            GameObject.Find("StepManager").GetComponent<StepManager>().step = 0;
            ButtonToPress();
            x++;
        }//���}�l����ʵe�ɲĤ@�����s�|�G
        foreach (PressableButton b in buttonsHoloLens2)//Continue
        {
            if(b.IsPressing)
            {
                isPlaying = false;
                switch (b.name)
                {
                    case "L.X+":
                        ik.MoveToolX(0.01f);
                        break;
                    case "L.X-":
                        ik.MoveToolX(-0.01f);
                        break;
                    case "L.Y+":
                        ik.MoveToolY(-0.025f);//up
                        break;
                    case "L.Y-":
                        ik.MoveToolY(0.025f);//down
                        break;
                    case "L.Z+":
                        ik.MoveToolZ(0.02f);//forward
                        break;
                    case "L.Z-":
                        ik.MoveToolZ(-0.02f);//back
                        break;
                    case "���չB��":
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

    public void ButtonRelease(MeshRenderer lastMesh)
    {
        ms = lastMesh.materials;
        ms[0].color = o;
    }

    public void ButtonToPress()//Check witch action to show
    {
        switch (actionType)
        {
            case 0:
                TutorialButtonToPress();
                break;
            case 1:
                GrabButtonToPress();
                break;
            case 2:
                FreeMode();
                break;
        }
    }

    public void TutorialButtonToPress()//Tutorial
    {
        int nowStep = GameObject.Find("StepManager").GetComponent<StepManager>().step;
        print("tutorial"+nowStep);
        switch (nowStep)
        {
            case 0:
                ResetBtns();
                ButtonPressed(tutorialBtn[nowStep].GetComponent<MeshRenderer>());
                break;
            case 1:
                ResetBtns();
                ButtonPressed(tutorialBtn[nowStep].GetComponent<MeshRenderer>());
                break;
            case 2:
                ResetBtns();
                ButtonPressed(tutorialBtn[nowStep].GetComponent<MeshRenderer>());
                break;
            case 3:
                ResetBtns();
                ButtonPressed(tutorialBtn[nowStep].GetComponent<MeshRenderer>());
                break;
            case 4:
                ResetBtns();
                ButtonPressed(tutorialBtn[nowStep].GetComponent<MeshRenderer>());
                break;
            case 5:
                ResetBtns();
                ButtonPressed(tutorialBtn[nowStep].GetComponent<MeshRenderer>());
                break;
            case 6:
                ResetBtns();
                ButtonPressed(tutorialBtn[0].GetComponent<MeshRenderer>());
                break;
        }
    }

    public void GrabButtonToPress()//TODO:Insert Real Step
    {
        int nowStep = GameObject.Find("StepManager").GetComponent<StepManager>().step;
        switch (nowStep)
        {
            case 0:
                ResetBtns();
                ButtonPressed(grabBtn[2].GetComponent<MeshRenderer>());
                break;
            case 1:
                ResetBtns();
                ButtonPressed(grabBtn[1].GetComponent<MeshRenderer>());
                break;
            case 2:
                ResetBtns();
                ButtonPressed(grabBtn[4].GetComponent<MeshRenderer>());
                break;
            case 3:
                ResetBtns();
                ButtonPressed(grabBtn[0].GetComponent<MeshRenderer>());
                break;
            case 4:
                ResetBtns();
                ButtonPressed(grabBtn[3].GetComponent<MeshRenderer>());
                break;
            case 5:
                ResetBtns();
                ButtonPressed(grabBtn[1].GetComponent<MeshRenderer>());
                break;
            case 6:
                ResetBtns();
                ButtonPressed(grabBtn[4].GetComponent<MeshRenderer>());
                break;
            case 7:
                ResetBtns();
                ButtonPressed(grabBtn[0].GetComponent<MeshRenderer>());
                break;
        }

    }

    public void FreeMode()
    {
        StepManager.instance.ResetRobotArm();
    }

    public void ResetBtns()
    {
        for (int i = 0;i<tutorialBtnMaterials.Count;i++)
        {
            tutorialBtn[i].GetComponent<MeshRenderer>().materials[0].color = tutorialBtnMaterials[i];
        }
        for (int j = 0; j < grabBtnMaterials.Count; j++)
        {
            grabBtn[j].GetComponent<MeshRenderer>().materials[0].color = grabBtnMaterials[j];
        }
    }
}
