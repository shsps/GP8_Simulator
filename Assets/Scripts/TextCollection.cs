using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextCollection : MonoBehaviour
{
    [SerializeField]public static Text Btext;
    // Start is called before the first frame update
    void Start()
    {
        Btext = GameObject.Find("TestBoard").GetComponentInChildren<Text>();
        Btext.text = "Ori";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void BackgroundText(string s)
    {
        Btext.text = s;
    }
}
