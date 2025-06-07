using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guess
{
    public bool sure = false;//已经确定
    public int sureChar = 0;
    public bool[] guess=new bool[27];//0:可能 1:不可能
}
