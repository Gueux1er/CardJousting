using LibLabSystem;
using UnityEngine;

namespace LibLabGames.NewGame
{
    public class IGameManager : MonoBehaviour
    {
        [Tooltip("At Start load scene \"LibLab Init\" to init LLSystem.")]
        public bool DEBUGApplyLLSystem = true;

        public bool gameHasStarted;

        public SettingValues settingValues;
        public SettingDialogs settingDialogs;

        public GameObject[] toActiveAtStart;
        public GameObject[] toDisableAtStart;

        [ContextMenu("Get setting game values")]
        public void SettingGameValues()
        {
            GetSettingGameValues();
        }

        public virtual void GetSettingGameValues()
        {
            // Example :
            // int value = settingValues.GetIntValue("exampleThree");
        }

        public bool DOAwake()
        {
            if (LLSystem.isInit == false)
            {
                LLUtils.LoadScene(0);
                return false;
            }

            foreach (var v in toActiveAtStart)
            {
                if (v != null) v.SetActive(true);
            }

            foreach (var v in toDisableAtStart)
            {
                if (v != null) v.SetActive(false);
            }

            return true;
        }

        public virtual void ReloadLevel()
        {
            LLUtils.ReloadCurrentScene();
        }

        public virtual void DOStart()
        {
            gameHasStarted = true;
        }
    }
}