using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour
{
    public GameObject cube;
    void Start()
    {
        this.GetComponent<Text>().text = File.Exists($"{Application.dataPath}/SampleQRCodes_Data/Resources/StepInfoPrefabs.txt").ToString();
        //File.Exists($"{Application.dataPath}/Resources/StepInfoPrefabs.txt").ToString()
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
