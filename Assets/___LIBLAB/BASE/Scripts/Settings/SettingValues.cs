using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using LibLabSystem;
using UnityEngine;
using UnityEditor;

namespace LibLabGames.NewGame
{
    [Serializable]
    public class BoolValueDictionary : SerializableDictionary<string, bool> { }

    [Serializable]
    public class FloatValueDictionary : SerializableDictionary<string, float> { }

    [CreateAssetMenu(fileName = "SettingValues", menuName = "LibLab/SettingValues")]
    public class SettingValues : ScriptableObject
    {
        [Header("CSV Files")]
        [SerializeField] public TextAsset CSVFile;

        [Header("Infos")]
        public BoolValueDictionary boolValues;
        public FloatValueDictionary floatValues;

        public bool GetBoolValue(string valueName)
        {
            if (!boolValues.ContainsKey(valueName))
            {
                LLLog.LogE("SettingValues", "<color=red>" + valueName + "</color> value not found !");
                return false;
            }
            return boolValues[valueName];
        }

        public float GetFloatValue(string valueName)
        {
            if (!floatValues.ContainsKey(valueName))
            {
                LLLog.LogE("SettingValues", "<color=red>" + valueName + "</color> value not found !");
                return 0;
            }
            return floatValues[valueName];
        }

        [ContextMenu("Clear All Values")]
        public void ClearAllValues()
        {
            boolValues.Clear();
            floatValues.Clear();
        }

        #if UNITY_EDITOR
        [ContextMenu("Update Values by CSV")]
        public void UpdateValuesByCSV()
        {
            ClearAllValues();
            string[, ] fileGrid = CSVReader.SplitCsvGrid(CSVFile.text);
            for (int i = 1; i < fileGrid.GetUpperBound(0); ++i)
            {
                if (fileGrid[i, 0] == null || fileGrid[i, 0] == "")
                    return;

                if (fileGrid[i, 1] == "TRUE" || fileGrid[i, 1] == "FALSE")
                {
                    boolValues.Add(fileGrid[i, 0], fileGrid[i, 1] == "TRUE" ? true : false);
                }
                else
                {
                    floatValues.Add(fileGrid[i, 0], float.Parse(fileGrid[i, 1]));
                }
            }

            EditorUtility.SetDirty(this);

            LLLog.Log("SettingValues", "The setting game values was successful updated !");
        }
        #endif
    }
}