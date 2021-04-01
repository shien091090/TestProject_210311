using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringFormatTest : MonoBehaviour
{
    public void BTN_StringFormatTest()
    {
        int _num = 12345678;

        Debug.Log(_num.ToString("N0"));
    }
}
