using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorCollect : MonoBehaviour
{
    public void ThrowError(string message)
    {
        GlobalVariable.instance.error.text= message;
    }
}
