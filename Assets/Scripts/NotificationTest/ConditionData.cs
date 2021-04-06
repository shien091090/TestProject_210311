using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using Newtonsoft.Json;

[System.Serializable]
public class ConditionData
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

    //--------------------------------------------------------

    private delegate bool del_tryParse<T>(string str, out T data);

    //--------------------------------------------------------

    public bool PropertyCompare(string[] conditionTable)
    {
        Type _type = this.GetType();
        FieldInfo[] _fieldInfoArr = _type.GetFields();

        List<bool> orList = new List<bool>();
        List<bool> andList = new List<bool>();

        for (int i = 0; i < _fieldInfoArr.Length; i++)
        {
            object _compareParam = _fieldInfoArr[i].GetValue(this);
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

    private SubCompareResult SpecificCompare(string tableParam, object compareParam, FieldInfo fieldInfo)
    {
        SubCompareResult _resultInfo = new SubCompareResult();
        string paramName = fieldInfo.Name;
        bool _result = false;

        if (paramName.Contains("_range")) //在數值範圍內
        {
            string[] _range = tableParam.Split('~');
            int _min = (int)ConvertData(_range[0], typeof(int));
            int _max = (int)ConvertData(_range[1], typeof(int));
            int _value = (int)compareParam;

            _result = (_min <= _value) && (_value <= _max);
        }
        else if (paramName.Contains("_greaterThan")) //大於指定數值
        {
            int _param = (int)ConvertData(tableParam, typeof(int));
            int _value = (int)compareParam;

            _result = (_value >= _param);
        }
        else if (paramName.Contains("_smallerThan")) //小於指定數值
        {
            int _param = (int)ConvertData(tableParam, typeof(int));
            int _value = (int)compareParam;

            _result = (_value <= _param);
        }
        else //未指定的狀況, 檢查是否相等
        {
            object _param = ConvertData(tableParam, fieldInfo.FieldType);

            _result = (_param == compareParam);
        }

        _resultInfo.result = _result;

        if (paramName.Contains("_and")) //加入"AND"邏輯判斷清單
            _resultInfo.logicTag = LogicalOperator.And;
        else //加入"OR"邏輯判斷清單
            _resultInfo.logicTag = LogicalOperator.Or;

        return _resultInfo;
    }

    private static object ConvertData(string data, Type type)
    {
        object value = null;

        if (type == typeof(int))
            value = TryParseData<int>(data, int.TryParse);

        else if (type == typeof(long))
            value = TryParseData<long>(data, long.TryParse);

        else if (type == typeof(float))
            value = TryParseData<float>(data, float.TryParse);

        else if (type == typeof(string))
            value = data;

        else
        {
            if (data.StartsWith("[") || data.StartsWith("{"))
                value = JsonConvert.DeserializeObject(data, type);
        }

        return value;
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
