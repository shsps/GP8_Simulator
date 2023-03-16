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
        var textFile = Resources.Load<TextAsset>("StepInfoPrefabs");
        this.GetComponent<Text>().text = textFile.text;
        print(textFile);
        //this.GetComponent<Text>().text = Resources.Load("StepInfoPrefabs.txt").ToString();
        //File.Exists($"{Application.dataPath}/Resources/StepInfoPrefabs.txt").ToString()
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
