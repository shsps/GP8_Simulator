using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointLimitException : System.Exception
{
    public JointLimitException()
    {

    }

    public JointLimitException(string message) : base(message)
    {

    }
}
