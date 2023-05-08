using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QRTracking
{
    public class UnityEditorQRcodeTest : MonoBehaviour
    {
        public static UnityEditorQRcodeTest Instance;

        [SerializeField] private GameObject QRcodePrefab;
        private GameObject QRcodeInstance;

        private TextMesh QRID;
        private TextMesh QRNodeID;
        private TextMesh QRText;
        private TextMesh QRVersion;
        private TextMesh QRTimeStamp;
        private TextMesh QRSize;
        private GameObject QRInfo;

        [SerializeField] private GameObject stepManager;
        [SerializeField] private GameObject gP8_3D;

        void Start()
        {
            /*QRcodePrefab = Instantiate(QRcodePrefab);
            QRcodePrefab.GetComponent<QRCode>().UnityEditorTest = true;

            QRInfo = QRcodePrefab.transform.GetChild(0).gameObject;
            QRID = QRInfo.transform.GetChild(0).gameObject.GetComponent<TextMesh>();
            QRNodeID = QRInfo.transform.GetChild(1).gameObject.GetComponent<TextMesh>();
            QRText = QRInfo.transform.GetChild(2).gameObject.GetComponent<TextMesh>();
            QRText.text = "Arm";
            QRVersion = QRInfo.transform.GetChild(3).gameObject.GetComponent<TextMesh>();
            QRSize = QRInfo.transform.GetChild(4).gameObject.GetComponent<TextMesh>();
            QRTimeStamp = QRInfo.transform.GetChild(5).gameObject.GetComponent<TextMesh>();*/

            gP8_3D.SetActive(true);
            stepManager.SetActive(true);
        }
    }
}
