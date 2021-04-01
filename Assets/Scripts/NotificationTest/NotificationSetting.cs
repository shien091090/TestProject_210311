using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu]
public class NotificationSetting : ScriptableObject
{
    public List<ConditionData> conditions;
}
