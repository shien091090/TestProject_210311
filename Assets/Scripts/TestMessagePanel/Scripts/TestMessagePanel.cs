using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKShien.Tools
{
    public class TestMessagePanel : MonoBehaviour
    {
        [Header("參考物件")]
        [SerializeField] private Text txt;

        [Header("遊戲進行狀態")]
        [SerializeField] private bool isActive;
        public bool GetActive { get { return isActive; } }

        public void SetUI(bool onOff)
        {
            this.gameObject.SetActive(onOff);
            isActive = onOff;
        }
    }

}