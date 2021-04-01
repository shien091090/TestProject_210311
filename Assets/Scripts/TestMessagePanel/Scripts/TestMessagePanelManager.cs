using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKShien.Tools
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Canvas))]
    public class TestMessagePanelManager : MonoBehaviour
    {
        private static TestMessagePanelManager _instance;
        public static TestMessagePanelManager Instance
        {
            get { return _instance; }
        }

        //--------------------------------------------------------------

        [Header("Setting")]
        [SerializeField] private TestMessagePanelSettingData settingData;
        [SerializeField] private TestMessagePanelSettingParameter settingParameter;

        [Header("Prefab")]
        public GameObject testPanelPrefab;

        private Canvas canvas;
        private TestMessagePanel testPanel;

        //--------------------------------------------------------------

        void Start()
        {
            BasicInitialize();
            if (Application.isPlaying) GameInitialize();
        }

        //--------------------------------------------------------------

        private void BasicInitialize()
        {
            if (Application.isPlaying)
            {
                if (_instance == null) _instance = this;
                else Destroy(this.gameObject);
            }

            settingParameter = new TestMessagePanelSettingParameter();
            settingParameter = settingData.settingParameter;

            if (canvas == null) canvas = this.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.sortingOrder = settingParameter.defaultOrder;
        }

        private void GameInitialize()
        {
            GameObject _go = Instantiate(testPanelPrefab, this.transform);
            testPanel = _go.GetComponent<TestMessagePanel>();

            testPanel.SetUI(false);
        }

        public void SaveSettingParameter()
        {
            settingData.settingParameter = settingParameter;
        }

        public void ShowMessage(string content)
        {
            //TODO : 顯示文字
        }
    }

}