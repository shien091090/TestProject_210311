using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NotificationSetting))]
public class NotificationTestEditor : Editor
{
    private NotificationSetting m_target;
    private string title = "NA";

    public override void OnInspectorGUI()
    {
        m_target = (NotificationSetting)target;

        GUILayout.Label(title, EditorStyles.boldLabel);

        if (GUILayout.Button("BTN1", GUILayout.Height(30)))
        {
            title = "1";
        }

        if (GUILayout.Button("BTN2", GUILayout.Height(30)))
        {
            title = "2";
        }

        base.OnInspectorGUI();
    }

}
