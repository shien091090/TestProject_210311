using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class StopWatchTest : MonoBehaviour
{
    public int ExcuteTimes;

    public void BTN_StopWatchTest()
    {
        Stopwatch _stopwatch = Stopwatch.StartNew();

        FuntionA();

        UnityEngine.Debug.Log("耗時 = " + string.Format("{0:N}", _stopwatch.ElapsedMilliseconds.ToString()));
    }

    private void FuntionA()
    {
        int count = 0;

        for (int i = 0; i < ExcuteTimes; i++)
        {
            string _s = string.Empty;
            int _r = Random.Range(1, 50);

            for (int j = 0; j < _r; j++)
            {
                _s += "a";
            }

            count += _s.Length;
        }

        UnityEngine.Debug.Log("count = " + count);
    }
}
