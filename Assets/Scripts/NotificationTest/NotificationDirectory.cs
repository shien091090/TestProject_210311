using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Reflection;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

public class NotificationDirectory
{
    private enum LogicalOperator
    {
        And,
        Or
    }

    private class SubCompareResult
    {
        public LogicalOperator logicTag;
        public bool result;
    }

    //------------------------------------------------------------------

    public const string CLASS_TAG_CONDITION = "condition";
    public const string CLASS_TAG_RESULT = "result";
    public const string CHAR_TAG_IGNORE = "-";
    public const string COMPARE_TAG_RANGE = "_range";
    public const string COMPARE_TAG_GREATERTHEN = "_greaterThan";
    public const string COMPARE_TAG_SMALLERTHAN = "_smallerThan";
    public const string COMPARE_TAG_CONTAIN = "_contain";
    public const string LOGICAL_TAG_AND = "_and";
    public const string LOGICAL_TAG_OR = "_or";
    public const char SPLIT_TAG_RANGE = '~';
    public const string SPLIT_REGEX_PATTERN = ",(?! )";

    private static Dictionary<Type, HashSet<int>> conditionClassIndexCollection;
    private static Dictionary<Type, List<int>> conditionClassColumns;
    private static List<int> resultClassColumns;
    private static List<string[]> notificationUnitStorage;

    private delegate bool del_tryParse<T>(string str, out T data);

    //------------------------------------------------------------------

    public static void Init()
    {
        LoadText();
    }

    public static BroadcastType ConditionCompare(List<ConditionData> conditions)
    {
        int[] _indexList = TypeFilter(conditions); //第一層判斷 : 是否存在指定複合類型的條件

        if (_indexList.Length <= 0) return BroadcastType.None;

        List<NotificationCompareResult> _resultGroup = new List<NotificationCompareResult>();
        _resultGroup = CongruentFilter(_indexList, conditions); //第二層判斷 : 條件是否完全吻合

        if (_resultGroup.Count <= 0) return BroadcastType.None;

        return BroadcastIntegration(_resultGroup);
    }

    private static void LoadText()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("notification_condition");

        LoadFromBytes(textAsset.bytes);
    }

    private static void LoadFromBytes(byte[] bytes)
    {
        conditionClassIndexCollection = new Dictionary<Type, HashSet<int>>();
        conditionClassColumns = new Dictionary<Type, List<int>>();
        resultClassColumns = new List<int>();
        notificationUnitStorage = new List<string[]>();

        using (StreamReader sr = new StreamReader(new MemoryStream(bytes)))
        {
            string line = sr.ReadLine(); //讀第一列
            string[] paramTypeFields = line.Split(',');

            line = sr.ReadLine(); //讀第二列
            string[] classTypeFields = line.Split(',');

            for (int i = 0; i < classTypeFields.Length; i++)
            {
                if (!IsValidString(classTypeFields[i])) continue;

                if (paramTypeFields[i] == CLASS_TAG_CONDITION)
                {
                    Type _type = Type.GetType(classTypeFields[i]);

                    if (_type == null) continue;

                    if (conditionClassColumns.ContainsKey(_type))
                        conditionClassColumns[_type].Add(i);
                    else
                        conditionClassColumns[_type] = new List<int>() { i };
                }
                else if (paramTypeFields[i] == CLASS_TAG_RESULT)
                {
                    Type _type = Type.GetType(classTypeFields[i]);

                    if (_type == null) continue;

                    resultClassColumns.Add(i);
                }
            }

            if (conditionClassColumns.Count <= 0) return;

            int _index = 0;
            do //讀以下所有列
            {
                line = sr.ReadLine();
                Regex reg = new Regex(SPLIT_REGEX_PATTERN);
                string[] datas = reg.Split(line);
                notificationUnitStorage.Add(datas);

                foreach (KeyValuePair<Type, List<int>> _pair in conditionClassColumns)
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

    private static int[] TypeFilter(List<ConditionData> conditions)
    {
        int[] _resultIndex = new int[0];
        List<HashSet<int>> _hashSetGroup = new List<HashSet<int>>();

        for (int i = 0; i < conditions.Count; i++)
        {
            Type _type = conditions[i].GetType();
            if (!conditionClassIndexCollection.ContainsKey(_type)) continue;

            _hashSetGroup.Add(new HashSet<int>(conditionClassIndexCollection[_type]));
        }

        if (_hashSetGroup.Count <= 0) return _resultIndex;

        HashSet<int> _intersection = new HashSet<int>(_hashSetGroup[0]);
        for (int i = 1; i < _hashSetGroup.Count; i++)
        {
            _intersection.IntersectWith(_hashSetGroup[i]);
        }

        if (_intersection.Count >= 1)
        {
            _resultIndex = new int[_intersection.Count];
            _intersection.CopyTo(_resultIndex);
        }

        return _resultIndex;
    }

    private static List<NotificationCompareResult> CongruentFilter(int[] indexArray, List<ConditionData> conditions)
    {
        List<NotificationCompareResult> _result = new List<NotificationCompareResult>();

        for (int i = 0; i < indexArray.Length; i++)
        {
            bool _isMatch = true;

            for (int j = 0; j < conditions.Count; j++)
            {
                ConditionData _conditionUnit = conditions[j];
                int _fieldCount = conditionClassColumns[_conditionUnit.GetType()].Count;
                string[] _paramField = new string[_fieldCount];

                for (int k = 0; k < _fieldCount; k++)
                {
                    int _columnIndex = conditionClassColumns[_conditionUnit.GetType()][k];
                    _paramField[k] = notificationUnitStorage[indexArray[i]][_columnIndex];
                }

                _isMatch = PropertyCompare(_paramField, _conditionUnit);
                if (!_isMatch) break;
            }

            if (_isMatch)
            {
                NotificationCompareResult _compareResult = new NotificationCompareResult();
                FieldInfo[] _fieldInfos = typeof(NotificationCompareResult).GetFields();

                for (int j = 0; j < resultClassColumns.Count; j++)
                {
                    int _columnIndex = resultClassColumns[j];
                    string _param = notificationUnitStorage[indexArray[i]][_columnIndex];

                    _fieldInfos[j].SetValue(_compareResult, ConvertData(_param, _fieldInfos[j].FieldType));
                }

                _result.Add(_compareResult);
            }
        }

        return _result;
    }



    private static bool PropertyCompare(string[] conditionTable, ConditionData compareObj)
    {
        Type _type = compareObj.GetType();
        FieldInfo[] _fieldInfoArr = _type.GetFields();

        List<bool> orList = new List<bool>();
        List<bool> andList = new List<bool>();

        for (int i = 0; i < conditionTable.Length; i++)
        {
            if (conditionTable[i] == CHAR_TAG_IGNORE) continue;

            object _compareParam = _fieldInfoArr[i].GetValue(compareObj);
            SubCompareResult _compare = SpecificCompare(conditionTable[i], _compareParam, _fieldInfoArr[i]);

            switch (_compare.logicTag)
            {
                case LogicalOperator.And:
                    andList.Add(_compare.result);
                    break;

                case LogicalOperator.Or:
                    orList.Add(_compare.result);
                    break;
            }
        }

        if ((orList == null || orList.Count <= 0) && (andList == null || andList.Count <= 0))
            return false;

        for (int i = 0; i < andList.Count; i++)
        {
            if (!andList[i]) return false;
        }

        bool _or = false;
        for (int i = 0; i < orList.Count; i++)
        {
            if (orList[i]) _or = true;
        }

        return _or;
    }

    private static SubCompareResult SpecificCompare(string tableParam, object compareParam, FieldInfo fieldInfo)
    {
        SubCompareResult _resultInfo = new SubCompareResult();
        string paramName = fieldInfo.Name;
        bool _result = false;

        if (paramName.Contains(COMPARE_TAG_RANGE)) //在數值範圍內
        {
            string[] _range = tableParam.Split(SPLIT_TAG_RANGE);
            int _min = (int)ConvertData(_range[0], typeof(int));
            int _max = (int)ConvertData(_range[1], typeof(int));
            int _value = (int)compareParam;

            _result = (_min <= _value) && (_value <= _max);
        }
        else if (paramName.Contains(COMPARE_TAG_GREATERTHEN)) //大於指定數值
        {
            int _param = (int)ConvertData(tableParam, typeof(int));
            int _value = (int)compareParam;

            _result = (_value >= _param);
        }
        else if (paramName.Contains(COMPARE_TAG_SMALLERTHAN)) //小於指定數值
        {
            int _param = (int)ConvertData(tableParam, typeof(int));
            int _value = (int)compareParam;

            _result = (_value <= _param);
        }
        else if (paramName.Contains(COMPARE_TAG_CONTAIN)) //包含指定值
        {
            IList _param = (IList)ConvertData(tableParam, fieldInfo.FieldType, true);

            _result = _param.Contains(compareParam);
        }
        else //未指定的狀況, 檢查是否相等
        {
            object _param = ConvertData(tableParam, fieldInfo.FieldType);

            _result = _param.Equals(compareParam);
        }

        _resultInfo.result = _result;

        if (paramName.Contains(LOGICAL_TAG_AND)) //加入"AND"邏輯判斷清單
            _resultInfo.logicTag = LogicalOperator.And;
        else //加入"OR"邏輯判斷清單
            _resultInfo.logicTag = LogicalOperator.Or;

        return _resultInfo;
    }

    private static BroadcastType BroadcastIntegration(List<NotificationCompareResult> resultGroup)
    {
        BroadcastType _type = resultGroup[0].broadcastType;

        for (int i = 1; i < resultGroup.Count; i++)
        {
            _type |= resultGroup[i].broadcastType;
        }

        return _type;
    }

    private static object ConvertData(string data, Type type, bool toList = false)
    {
        object value = null;
        bool isIgnore = (data == CHAR_TAG_IGNORE);

        if (type.IsEnum && !toList)
            value = Enum.Parse(type, data);

        else if (type == typeof(int))
            value = ConvertOuput<int>(toList, isIgnore, data, typeof(List<int>), TryParseData<int>(data, int.TryParse));

        else if (type == typeof(long))
            value = ConvertOuput<long>(toList, isIgnore, data, typeof(List<long>), TryParseData<long>(data, long.TryParse));

        else if (type == typeof(float))
            value = ConvertOuput<float>(toList, isIgnore, data, typeof(List<float>), TryParseData<float>(data, float.TryParse));

        else if (type == typeof(string))
            value = ConvertOuput<string>(toList, isIgnore, data, typeof(List<string>), data);

        else if (!isIgnore)
        {
            if (data.StartsWith("[") || data.StartsWith("{"))
                value = JsonConvert.DeserializeObject(data, type);
        }

        return value;
    }

    private static object ConvertOuput<T>(bool toList, bool isIgnore, string data, Type type, object tryParseObj)
    {
        if (toList)
            return JsonConvert.DeserializeObject(data, type);
        else
            return isIgnore ? default(T) : tryParseObj;
    }

    private static bool IsValidString(string _str)
    {
        return
            !string.IsNullOrEmpty(_str) &&
            _str != CHAR_TAG_IGNORE;
    }

    private static object TryParseData<T>(string data, del_tryParse<T> func)
    {
        object _result = null;
        T _obj = Activator.CreateInstance<T>();

        if (!func(data, out _obj))
            _result = default(T);
        else
            _result = _obj;

        return _result;
    }
}