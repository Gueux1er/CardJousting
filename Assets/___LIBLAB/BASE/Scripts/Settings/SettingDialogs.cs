using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using LibLabSystem;
using UnityEngine;

namespace LibLabGames.NewGame
{
    [Serializable]
    public class Dialog
    {
        public string tag;
        public string characterName;
        public List<List<string>> dialogs;
        public List<List<string>> choices;
    }

    [Serializable]
    public class ColorWordDictionary : SerializableDictionary<string, Color> { }

    [CreateAssetMenu(fileName = "SettingDialogs", menuName = "LibLab/SettingDialogs")]
    public class SettingDialogs : ScriptableObject
    {
        [Header("CSV Files")]
        [SerializeField] public TextAsset CSVDialogFile;
        [SerializeField] public TextAsset CSVColorWordFile;

        [Header("Infos")]
        public List<Dialog> dialogs;
        public ColorWordDictionary colorWords;

        public Dialog GetDialog(string tagName)
        {
            foreach (Dialog d in dialogs)
            {
                if (d.tag == tagName)
                    return d;
            }
            LLLog.LogE("SettingDialogs", "<color=red>" + tagName + "</color> tag not found for dialog value!");
            return null;
        }

        public Color GetColorValue(string tagName)
        {
            if (colorWords.ContainsKey(tagName))
                return colorWords[tagName];

            LLLog.LogE("SettingDialogs", "<color=red>" + tagName + "</color> tag not found for color value!");
            return Color.white;
        }

        [ContextMenu("Clear Dialog")]
        public void ClearDialog()
        {
            dialogs.Clear();
            colorWords.Clear();
        }

        private Dialog d;
        private Color c;
        private string[] sArray;
        private List<string> sList;
        [ContextMenu("Update Dialogs by CSV")]
        public void UpdateDialogsByCSV(bool log = true)
        {
            ClearDialog();
            string[, ] fileGrid = CSVReader.SplitCsvGrid(CSVDialogFile.text);
            for (int i = 1; i < fileGrid.GetUpperBound(0); ++i)
            {
                d = new Dialog();
                d.tag = fileGrid[i, 0];
                d.characterName = fileGrid[i, 1];

                d.dialogs = new List<List<string>>();
                sArray = fileGrid[i, 2].Split('|');
                sList = new List<string>();
                for (int j = 0; j < sArray.Length; ++j)
                {
                    sList.Add(sArray[j].Remove(0, 5));

                    if ((j + 1 < sArray.Length && int.Parse(sArray[j][1].ToString()) != int.Parse(sArray[j + 1][1].ToString()))
                    || j == sArray.Length - 1)
                    {
                        d.dialogs.Add(new List<string>());

                        for (int k = 0; k < sList.Count; ++k)
                            d.dialogs[d.dialogs.Count - 1].Add(sList[k]);
                        sList = new List<string>();
                    }
                }

                d.choices = new List<List<string>>();
                if (fileGrid[i, 3] != null && fileGrid[i, 3] != "")
                {
                    sArray = fileGrid[i, 3].Split('|');
                    sList = new List<string>();
                    for (int j = 0; j < sArray.Length; ++j)
                    {
                        sList = sArray[j].Split('*').ToList();
                        d.choices.Add(sList);
                    }
                }

                dialogs.Add(d);
            }

            fileGrid = CSVReader.SplitCsvGrid(CSVColorWordFile.text);
            for (int i = 1; i < fileGrid.GetUpperBound(0); ++i)
            {
                if (ColorUtility.TryParseHtmlString("#" + fileGrid[i, 1], out c))
                    colorWords.Add(fileGrid[i, 0], c);
                else
                    colorWords.Add("!ERROR! " + fileGrid[i, 0], Color.clear);
            }

            if (log)
                LLLog.Log("SettingDialogs", "The setting game dialogs was successful updated !");
        }
    }
}