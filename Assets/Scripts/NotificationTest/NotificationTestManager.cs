using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;

public class NotificationTestManager : MonoBehaviour
{


    public void BTN_Test()
    {
        NotificationData.Init();

        NotificationCondition _noti = new NotificationCondition();
        _noti
            .SetCondition(new Cond_GameType("PrizePoker Game"))
            .SetCondition(new Cond_HallType(3))
            .SetCondition(new Cond_WinType("123"))
            .ConditionFilter(() =>
            {

            });
    }



}

//-----------------------------------------------------------------------------------------------------------------------------

public class NotificationCondition
{
    public List<ConditionData> conditionDatas = new List<ConditionData>();

    public NotificationCondition SetCondition(ConditionData _cond)
    {
        conditionDatas.Add(_cond);

        return this;
    }

    public void ConditionFilter(System.Action callback)
    {
        //TODO : 與條件庫比對
        NotificationData.ConditionCompare(conditionDatas);

        callback.Invoke();
    }
}

//-----------------------------------------------------------------------------------------------------------------------------

public class NotificationData
{
    private const string TAG_CONDITION = "condition";
    private const string IGNORE_CHAR = "-";

    private static Dictionary<System.Type, HashSet<int>> conditionClassIndexCollection;
    private static Dictionary<System.Type, List<int>> conditionClassColumns = new Dictionary<System.Type, List<int>>();
    private static List<string[]> notificationUnitStorage;

    public static void Init()
    {
        LoadText();
    }

    private static void LoadText()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("notification_condition");

        LoadFromBytes(textAsset.bytes);
    }

    private static void LoadFromBytes(byte[] bytes)
    {
        using (StreamReader sr = new StreamReader(new MemoryStream(bytes)))
        {
            string line = sr.ReadLine(); //讀第一列
            string[] paramTypeFields = line.Split(',');

            line = sr.ReadLine(); //讀第二列
            string[] classTypeFields = line.Split(',');

            for (int i = 0; i < classTypeFields.Length; i++)
            {
                if (!IsValidString(classTypeFields[i])) continue;

                if (paramTypeFields[i] == TAG_CONDITION)
                {
                    System.Type _type = System.Type.GetType(classTypeFields[i]);

                    if (_type == null) continue;

                    if (conditionClassColumns.ContainsKey(_type))
                        conditionClassColumns[_type].Add(i);
                    else
                        conditionClassColumns[_type] = new List<int>() { i };
                }
            }

            if (conditionClassColumns.Count <= 0) return;

            notificationUnitStorage = new List<string[]>();
            conditionClassIndexCollection = new Dictionary<System.Type, HashSet<int>>();

            int _index = 0;
            do //讀以下所有列
            {
                line = sr.ReadLine();
                notificationUnitStorage.Add(line.Split(','));

                foreach (KeyValuePair<System.Type, List<int>> _pair in conditionClassColumns)
                {
                    for (int i = 0; i < _pair.Value.Count; i++)
                    {
                        if (!IsValidString(notificationUnitStorage[_index][_pair.Value[i]])) continue;

                        if (conditionClassIndexCollection.ContainsKey(_pair.Key))
                            conditionClassIndexCollection[_pair.Key].Add(_index);
                        else
                            conditionClassIndexCollection[_pair.Key] = new HashSet<int>() { _index };
                    }
                }

                _index++;

            } while (!sr.EndOfStream);


        }
    }

    public static bool ConditionCompare(List<ConditionData> conditions)
    {
        bool _result = false;
        int[] indexArray = new int[0];
        List<HashSet<int>> _hashSetGroup = new List<HashSet<int>>();

        for (int i = 0; i < conditions.Count; i++)
        {
            System.Type _type = conditions[i].GetType();
            if (!conditionClassIndexCollection.ContainsKey(_type)) continue;

            _hashSetGroup.Add(new HashSet<int>(conditionClassIndexCollection[_type]));
        }

        if (_hashSetGroup.Count <= 0) return _result;

        HashSet<int> _intersection = new HashSet<int>(_hashSetGroup[0]);
        for (int i = 1; i < _hashSetGroup.Count; i++)
        {
            _intersection.IntersectWith(_hashSetGroup[i]);
        }

        if (_intersection.Count >= 1)
        {
            indexArray = new int[_intersection.Count];
            _intersection.CopyTo(indexArray);
            _result = true;
        }

        return _result;
    }

    private static bool IsValidString(string _str)
    {
        return
            !string.IsNullOrEmpty(_str) &&
            _str != IGNORE_CHAR;
    }
}