using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu]
public class Cond_WinType : ConditionData
{
    public string winType;

    public Cond_WinType(string _typeName)
    {
        winType = _typeName;
    }

}
