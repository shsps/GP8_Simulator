using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBoard : MonoBehaviour
{
    [SerializeField] private Text text;
    private string[][] textArray =
    {
        new string[] {"�Ĥ@�B���UX+", "�ĤG�B���UX-", "�ĤT�B���UY+", "�ĥ|�B���UY-"},
        new string[] {"�Ĥ@�B���ʨ������W��", "�ĤG�B�V�U���ʨ�i�����d��", "�ĤT�����U�������s"}
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
        text.text = textArray[TextArrayOrder][TextOrder];
    }

    public void ChangeTextOrder(ChangeOrderDirection direction)
    {
        if (direction == ChangeOrderDirection.Next)
        {
            if(TextOrder == textArray.Length - 1)
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

    public void ChangeTextArrayOrder(ChangeOrderDirection direction)
    {
        if (direction == ChangeOrderDirection.Next)
        {
            if (TextArrayOrder == textArray[TextArrayOrder].Length - 1)
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
}
