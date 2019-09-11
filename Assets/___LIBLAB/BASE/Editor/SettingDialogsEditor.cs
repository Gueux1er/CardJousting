using System.Collections;
using UnityEditor;
using UnityEngine;

namespace LibLabGames.NewGame
{
    [CustomEditor(typeof(SettingDialogs))]
    public class SettingDialogsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var script = (SettingDialogs) target;
    
            if (GUILayout.Button("Update Dialogs by CSV"))
            {
                script.UpdateDialogsByCSV();
            }
        }
    }
}