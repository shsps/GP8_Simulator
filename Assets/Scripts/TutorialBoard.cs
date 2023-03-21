using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBoard : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private Text title;
    private string[][] textArray =
    {
        new string[] {"按下X+可向右移動", "按下X-可向左移動", "按下Y+可向上移動", "按下Y-可向下移動", "按下Z+可向前移動", "按下Z-可向後移動"},
        new string[] {"第一步:移動到方塊正上方", "第二步:向下移動到可夾取範圍，", "第三步:按下夾取按鈕(此處以協助鈕代替)", "第四步:向上移動", "第五步:前進至欲放置之位置上方", "第六步:向下至欲放置之位置", "第七步:按下放開按鈕(此處以協助鈕代替)", "第八步:回至原點" },
        new string[] {"請依照剛剛教學之步驟進行輸入" }
    };
    public int TextArrayOrder = 0;
    public int TextOrder = 0;
    public enum ChangeOrderDirection
    {
        Next,
        Previous
    }

    void Start()
    {
        title.text = "基礎教學";
        text.text = textArray[TextArrayOrder][TextOrder];
    }

    public void ChangeTextOrder(ChangeOrderDirection direction)//Step
    {
        if (direction == ChangeOrderDirection.Next)
        {
            if(TextOrder == textArray[TextArrayOrder].Length - 1)
            {
                throw new System.IndexOutOfRangeException("TextOrder is at the last.");
            }
            TextOrder++;
        }
        else if (direction == ChangeOrderDirection.Previous)
        {
            if(TextOrder == 0)
            {
                throw new System.IndexOutOfRangeException("TextOrder is at the first");
            }
            TextOrder--;
        }

        text.text = textArray[TextArrayOrder][TextOrder];
    }

    public void ChangeTextArrayOrder(ChangeOrderDirection direction)//Action
    {
        if (direction == ChangeOrderDirection.Next)
        {
            if (TextArrayOrder == textArray.Length - 1)
            {
                throw new System.IndexOutOfRangeException("TextArrayOrder is at the last.");
            }
            TextArrayOrder++;
        }
        else if (direction == ChangeOrderDirection.Previous)
        {
            if (TextArrayOrder == 0)
            {
                throw new System.IndexOutOfRangeException("TextArrayOrder is at the first");
            }
            TextArrayOrder--;
        }
        TextOrder = 0;

        text.text = textArray[TextArrayOrder][TextOrder];
    }

    public void ChangeTitle()//Title
    {
        switch (TextArrayOrder)
        {
            case 0:
                title.text = "基礎教學";
                break;
            case 1:
                title.text = "夾取及紀錄教學";
                break;
            case 2:
                title.text = "自由模式";
                break;
        }
    }
}
