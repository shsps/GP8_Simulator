using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepInfoGenerator : MonoBehaviour
{
    //private List<StepInfo> stepInfoList = new List<StepInfo>();

    private static Regex regex1 = new Regex(@"{[^{^}]+}", RegexOptions.Multiline);
    private static Regex regex2 = new Regex(@"((\d+\.?\d*,)|[A-Za-z]+)+", RegexOptions.Multiline);

    public static List<List<StepInfo>> StepInfoReader()
    {
        List<List<StepInfo>> result = new List<List<StepInfo>>();
        string readText = File.ReadAllText($"{Application.dataPath}/Resources/StepInfoPrefabs.txt");
        MatchCollection matchCollection1 = regex1.Matches(readText);
       // Debug.Log(readText);
        foreach (Match match1 in matchCollection1)
        {
            List<string> group1 = new List<string>();
            MatchCollection matchCollection2 = regex2.Matches(match1.Value);
            List<StepInfo> tmpStepInfo = new List<StepInfo>();

            //Debug.Log(match1.Value);
            foreach (Match match2 in matchCollection2)
            {
                group1.Add(match2.Value);
            }

            GameObject robotArm = GameObject.Find(group1[0]);
            robotArm.TryGetComponent<IKManager3D2>(out IKManager3D2 _ik);
            for (int i = 1; i < group1.Count; i++)
            {
                string[] sp = group1[i].Split(','); //rotate angle and isCatch
                tmpStepInfo.Add(new StepInfo(_ik,
                    float.Parse(sp[0]),
                    float.Parse(sp[1]),
                    float.Parse(sp[2]),
                    bool.Parse(sp[3])));
            }
            result.Add(tmpStepInfo);
        }
        return result;
    }

    public static void StepInfoWriter()
    {

    }
}
