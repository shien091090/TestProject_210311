using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu]
public class Cond_GameType : ConditionData
{
    public string gameType;

    public Cond_GameType(string _typeName)
    {
        gameType = _typeName;
    }
}
