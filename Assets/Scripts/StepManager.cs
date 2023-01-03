using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    public static StepManager instance;
    public List<StepInfo> StepInfos = new List<StepInfo>();

    void Start()
    {
        instance = this;
    }

    
}
