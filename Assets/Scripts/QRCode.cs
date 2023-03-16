using System.Collections;

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
        public GameObject Base/*手臂基座*/,Arm;//自定義手臂
        public GameObject JointRotation;
        public GameObject StepManagerController;
        //public GameObject DefaultThings;//桌子，板子，要夾的物品
        //public GameObject qrPosition;//自定義父物件

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
        Vector3 v = new Vector3(270, 0, 0);
        public static int a = 0;
        public static bool test = false;
        //public Text debug;

        // Use this for initialization
        void Start()
        {
            a = 0;
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
            //qrPosition = this.gameObject;//自定義父物件
            //debug = gameObject.transform.Find("Debug").gameObject.GetComponent<Text>();//Debug
            //JointRotation = gameObject.transform.Find("JointRotation").gameObject;
            QRInfo = gameObject.transform.Find("QRInfo").gameObject;
            QRID = QRInfo.transform.Find("QRID").gameObject.GetComponent<TextMesh>();
            QRNodeID = QRInfo.transform.Find("QRNodeID").gameObject.GetComponent<TextMesh>();
            QRText = QRInfo.transform.Find("QRText").gameObject.GetComponent<TextMesh>();
            QRVersion = QRInfo.transform.Find("QRVersion").gameObject.GetComponent<TextMesh>();
            QRTimeStamp = QRInfo.transform.Find("QRTimeStamp").gameObject.GetComponent<TextMesh>();
            QRSize = QRInfo.transform.Find("QRSize").gameObject.GetComponent<TextMesh>();

            ShowError = QRInfo.transform.Find("ShowError").gameObject.GetComponent<TextMesh>();

            StepManagerController = gameObject.transform.Find("StepManager").gameObject;

            //DefaultThings = gameObject.transform.Find("DefaultThings").gameObject;

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
        }

        void UpdatePropertiesDisplay()
        {
            // Update properties that change
            if (qrCode != null && lastTimeStamp != qrCode.SystemRelativeLastDetectedTime.Ticks)
            {
                QRSize.text = "Size:" + qrCode.PhysicalSideLength.ToString("F04") + "m";

                QRTimeStamp.text = "Time:" + qrCode.LastDetectedTime.ToString("MM/dd/yyyy HH:mm:ss.fff");
                QRTimeStamp.color = QRTimeStamp.color==Color.yellow? Color.white: Color.yellow;
                PhysicalSize = qrCode.PhysicalSideLength;
                Debug.Log("Id= " + qrCode.Id + "NodeId= " + qrCode.SpatialGraphNodeId + " PhysicalSize = " + PhysicalSize + " TimeStamp = " + qrCode.SystemRelativeLastDetectedTime.Ticks + " Time = " + qrCode.LastDetectedTime.ToString("MM/dd/yyyy HH:mm:ss.fff"));

                
                qrCodeCube.transform.localPosition = new Vector3(PhysicalSize / 2.0f, PhysicalSize / 2.0f, 0.0f);
                qrCodeCube.transform.localScale = new Vector3(PhysicalSize, PhysicalSize, 0.005f);
                lastTimeStamp = qrCode.SystemRelativeLastDetectedTime.Ticks;
                QRInfo.transform.localScale = new Vector3(PhysicalSize/0.2f, PhysicalSize / 0.2f, PhysicalSize / 0.2f);


                if (QRText.text == "50872"||test)
                {
                    Base.SetActive(true);
                    Base.transform.localPosition = new Vector3(PhysicalSize / 2.0f, PhysicalSize / 2.0f, 0.0f);//機械手臂基座位置
                    Base.transform.localScale = new Vector3(100, 100, 100);//機械手臂基座大小
                    Arm.transform.localPosition = new Vector3(0, 0.00212f, 0);
                    Arm.GetComponent<IKManager3D2>().Init();
                    a = 0;

                    StepManagerController.SetActive(true);

                    ShowError.text = StepManagerController.activeSelf.ToString();

                    StepManager.instance.MoveNextSlowly();//Maybe this will have bug
                }
                //ShowError.text = Base.activeSelf.ToString();
                a = 0;
            }

            Base.transform.rotation = Quaternion.Euler(0, 0, 0);
            //Arm.transform.rotation = Quaternion.Euler(0,0,0);

            /*DefaultThings.transform.localPosition = new Vector3(0, 0, 0);
            DefaultThings.transform.rotation = Quaternion.Euler(0, 0, 0);*/

            if ((a<4)&&(Arm.transform.rotation!=Quaternion.Euler(0,0,0)))
            {
                Arm.transform.rotation = Quaternion.Euler(0,0,0);
                a++;
            }
            //JointRotation.transform.rotation = Quaternion.Euler(90,0,0);
            /*if(this.transform.rotation != Quaternion.Euler(v))
            {
                this.transform.rotation = Quaternion.Euler(v);
            }*/
            //debug = gameObject.transform.Find("Debug").gameObject.GetComponent<Text>();
            //debug.text = this.transform.rotation.x.ToString() +" "+ this.transform.rotation.y.ToString() +" "+ this.transform.rotation.z.ToString();
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