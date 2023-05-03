using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QRTracking
{
    [RequireComponent(typeof(QRTracking.SpatialGraphNodeTracker))]
    public class QRCode : MonoBehaviour
    {
        public Microsoft.MixedReality.QR.QRCode qrCode;
        private GameObject qrCodeCube;
        public GameObject Base/*手臂基座*/,Arm/*自定義手臂*/,Default;
        public GameObject JointRotation;
        public GameObject StepManagerController;
        public GameObject Circle, Cube;//欲夾物體
        public int DontActiveTheArm = 0;
        public string CompareStr = "Arm";
        public Text TestText;

        public TextMesh ShowError;
        public float PhysicalSize { get; private set; }
        public string CodeText { get; private set; }

        private TextMesh QRID;
        private TextMesh QRNodeID;
        private TextMesh QRText;
        private TextMesh QRVersion;
        private TextMesh QRTimeStamp;
        private TextMesh QRSize;
        private GameObject QRInfo;
        private bool validURI = false;
        private bool launch = false;
        private System.Uri uriResult;
        private long lastTimeStamp = 0;
        public static int locateCounter = 0;
        int firstTime = 0;
        public static bool test = false;
        int timePassed;
        public GameObject QRCodeGameObject;
        int secondsOfStamp = 0;

        // Use this for initialization
        void Start()
        {
            locateCounter = 0;
            DontActiveTheArm = 0;
            PhysicalSize = 0.1f;
            CodeText = "Dummy";
            if (qrCode == null)
            {
                throw new System.Exception("QR Code Empty");
            }

            PhysicalSize = qrCode.PhysicalSideLength;
            CodeText = qrCode.Data;

            qrCodeCube = gameObject.transform.Find("Cube").gameObject;
            Base = gameObject.transform.Find("GP8_3D").gameObject;
            Arm = Base.transform.Find("JointS").gameObject;//prefab內物件
            Default = Base.transform.Find("DefaultThings").gameObject;

            QRInfo = gameObject.transform.Find("QRInfo").gameObject;
            QRID = QRInfo.transform.Find("QRID").gameObject.GetComponent<TextMesh>();
            QRNodeID = QRInfo.transform.Find("QRNodeID").gameObject.GetComponent<TextMesh>();
            QRText = QRInfo.transform.Find("QRText").gameObject.GetComponent<TextMesh>();
            QRVersion = QRInfo.transform.Find("QRVersion").gameObject.GetComponent<TextMesh>();
            QRTimeStamp = QRInfo.transform.Find("QRTimeStamp").gameObject.GetComponent<TextMesh>();
            QRSize = QRInfo.transform.Find("QRSize").gameObject.GetComponent<TextMesh>();

            ShowError = QRInfo.transform.Find("ShowError").gameObject.GetComponent<TextMesh>();
            QRCodeGameObject = this.gameObject;

            StepManagerController = gameObject.transform.Find("StepManager").gameObject;

            QRID.text = "Id:" + qrCode.Id.ToString();
            QRNodeID.text = "NodeId:" + qrCode.SpatialGraphNodeId.ToString();
            QRText.text = CodeText;

            if (System.Uri.TryCreate(CodeText, System.UriKind.Absolute,out uriResult))
            {
                validURI = true;
                QRText.color = Color.blue;
            }

            QRVersion.text = "Ver: " + qrCode.Version;
            QRSize.text = "Size:" + qrCode.PhysicalSideLength.ToString("F04") + "m";
            QRTimeStamp.text = "Time:" + qrCode.LastDetectedTime.ToString("MM/dd/yyyy HH:mm:ss.fff");
            QRTimeStamp.color = Color.yellow;
            Debug.Log("Id= " + qrCode.Id + "NodeId= " + qrCode.SpatialGraphNodeId + " PhysicalSize = " + PhysicalSize + " TimeStamp = " + qrCode.SystemRelativeLastDetectedTime.Ticks + " QRVersion = " + qrCode.Version + " QRData = " + CodeText);
            
            Base.SetActive(false);
            Arm.SetActive(false);
            Default.SetActive(false);

        }

        void UpdatePropertiesDisplay()
        {
            
            int secondsOfNow = (DateTime.Now.Hour * 60 * 60) + (DateTime.Now.Minute * 60) + (DateTime.Now.Second);

            if (qrCode != null && lastTimeStamp != qrCode.SystemRelativeLastDetectedTime.Ticks)
            {
                secondsOfStamp = (qrCode.LastDetectedTime.Hour * 60 * 60) + (qrCode.LastDetectedTime.Minute * 60) + (qrCode.LastDetectedTime.Second);

                ///Below is fine
                QRSize.text = "Size:" + qrCode.PhysicalSideLength.ToString("F04") + "m";

                QRTimeStamp.text = "Time:" + qrCode.LastDetectedTime.ToString("MM/dd/yyyy HH:mm:ss.fff");
                QRTimeStamp.color = QRTimeStamp.color==Color.yellow? Color.white: Color.yellow;
                PhysicalSize = qrCode.PhysicalSideLength;
                Debug.Log("Id= " + qrCode.Id + "NodeId= " + qrCode.SpatialGraphNodeId/*Same*/ + " PhysicalSize = " + PhysicalSize + " TimeStamp = " + qrCode.SystemRelativeLastDetectedTime.Ticks + " Time = " + qrCode.LastDetectedTime.ToString("MM/dd/yyyy HH:mm:ss.fff"));

                qrCodeCube.transform.localPosition = new Vector3(PhysicalSize / 2.0f, PhysicalSize / 2.0f, 0.0f);
                qrCodeCube.transform.localScale = new Vector3(PhysicalSize, PhysicalSize, 0.005f);
                lastTimeStamp = qrCode.SystemRelativeLastDetectedTime.Ticks;
                QRInfo.transform.localScale = new Vector3(PhysicalSize/0.2f, PhysicalSize / 0.2f, PhysicalSize / 0.2f);
                ///Above is fine

                if (QRText.text == CompareStr)
                {
                    if(firstTime!=0&&(secondsOfNow-secondsOfStamp)<1)
                    {
                        firstTime = 0;
                    }
                    Base.SetActive(true);
                    Default.SetActive(true);
                    Base.transform.localPosition = new Vector3(PhysicalSize / 2.0f, PhysicalSize / 2.0f, 0.0f);//機械手臂基座位置OK
                    Base.transform.localScale = new Vector3(100, 100, 100);//機械手臂基座大小OK
                    Arm.SetActive(true);

                    StepManagerController.SetActive(true);

                    StepManager.instance.ResetRobotArm();
                    Arm.GetComponent<IKManager3D2>().InitPositionRotation();

                    locateCounter = 0;
                }
            }

            Base.transform.rotation = Quaternion.Euler(0, 0, 0);//OK
            Arm.transform.localPosition = new Vector3(0, 0.00212f, 0);//OK

            timePassed = secondsOfNow - secondsOfStamp;
            ShowError.text = timePassed.ToString();

            if ((timePassed<2)&&(Arm.transform.rotation!=Quaternion.Euler(0,0,0)))
            {
                Arm.transform.rotation = Quaternion.Euler(0,0,0);
                locateCounter++;
            }

            if(timePassed>=2)
            {
                
                if(firstTime==0)
                {
                    //lastSetTime = secondsOfStamp;

                    Circle.transform.rotation = Quaternion.Euler(90, 0, 0);
                    Cube.transform.rotation = Quaternion.Euler(90, 0, 0);
                    Circle.transform.localPosition = new Vector3(-0.00083f, 0.004676f, 0.004452f);
                    Cube.transform.localPosition = new Vector3(-0.000270001f,0.004675752f, 0.004481005f);

                    StepManager.instance.ResetCatchableItemOrigin();//紀錄初始點
                    //StepManager.instance.MoveNextSlowly();//不會作用
                    StepManager.instance.MoveDirectly(StepManager.changeStepDirection.negative);
                    //StepManager.instance.ChangeStepOrder(-1);
                    firstTime++;
                }
                Circle.transform.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                Cube.transform.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

            }

        }

        // Update is called once per frame
        void Update()
        {
            UpdatePropertiesDisplay();
            if (launch)
            {
                launch = false;
                LaunchUri();
            }
            
        }

        void LaunchUri()
        {
#if WINDOWS_UWP
            // Launch the URI
            UnityEngine.WSA.Launcher.LaunchUri(uriResult.ToString(), true);
#endif
        }

        public void OnInputClicked()
        {
            if (validURI)
            {
                launch = true;
            }
// eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }
    }
}