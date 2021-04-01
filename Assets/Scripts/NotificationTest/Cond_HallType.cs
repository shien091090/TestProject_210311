using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu]
public class Cond_HallType : ConditionData
{
    public int hallId;

    public Cond_HallType(int id)
    {
        hallId = id;
    }
}