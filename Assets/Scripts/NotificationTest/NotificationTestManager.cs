using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class NotificationTestManager : MonoBehaviour
{
    public void BTN_Test()
    {
        NotificationDirectory.Init();

        NotificationTicket _noti = new NotificationTicket();
        _noti
            .SetCondition(new Cond_GameType() { gameType = "PrizePoker Game" })
            .SetCondition(new Cond_HallType() { hallId_contain = 1 })
            .ConditionFilter((BroadcastType t) =>
            {
                if ((t & BroadcastType.Marquee) == BroadcastType.Marquee)
                    Debug.Log("Marquee");

                if ((t & BroadcastType.ChatMsg) == BroadcastType.ChatMsg)
                    Debug.Log("ChatMsg");

                if ((t & BroadcastType.Pushs) == BroadcastType.Pushs)
                    Debug.Log("Pushs");
            });
    }
}

//-----------------------------------------------------------------------------------------------------------------------------

[Flags]
public enum BroadcastType
{
    None = 0,
    Marquee = 1,
    ChatMsg = 2,
    Pushs = 4,
    All = Marquee | ChatMsg | Pushs
}

//-----------------------------------------------------------------------------------------------------------------------------

public class NotificationTicket
{
    public List<ConditionData> conditionDatas = new List<ConditionData>();

    public NotificationTicket SetCondition(ConditionData _cond)
    {
        conditionDatas.Add(_cond);

        return this;
    }

    public void ConditionFilter(Action<BroadcastType> callback)
    {
        BroadcastType _outputType = BroadcastType.None;
        _outputType = NotificationDirectory.ConditionCompare(conditionDatas);

        callback.Invoke(_outputType);
    }
}