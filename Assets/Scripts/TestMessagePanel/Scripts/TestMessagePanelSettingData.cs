using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NKShien.Tools
{
    [CreateAssetMenu]
    public class TestMessagePanelSettingData : ScriptableObject
    {
        public TestMessagePanelSettingParameter settingParameter;
    }

    [System.Serializable]
    public struct TestMessagePanelSettingParameter
    {
        [System.Serializable]
        public struct PanelFunctionKey
        {
            public TestPanelButtonFunction func;
            public KeyCode key;
        }

        //-------------------------------------------------

        public int defaultOrder;
        public PanelFunctionKey[] panelFunctionKeySetting;

        //-------------------------------------------------

        public TestMessagePanelSettingParameter SetDefaultOrder(int order)
        {
            defaultOrder = order;
            return this;
        }

        public TestMessagePanelSettingParameter SetPanelFunctionKey(TestPanelButtonFunction func, KeyCode key)
        {
            for (int i = 0; i < panelFunctionKeySetting.Length; i++)
            {
                if (panelFunctionKeySetting[i].func == func)
                {
                    panelFunctionKeySetting[i].key = key;
                    break;
                }
            }

            return this;
        }
    }
}
