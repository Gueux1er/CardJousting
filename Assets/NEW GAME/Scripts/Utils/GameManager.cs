using System.Collections;
using System.Collections.Generic;
using LibLabSystem;
using UnityEngine;
using PathologicalGames;
using TMPro;
using DG.Tweening;

namespace LibLabGames.NewGame
{
    public class GameManager : IGameManager
    {
        public static GameManager instance;

        public override void GetSettingGameValues()
        {
            // Example :
            // int value = settingValues.GetIntValue("exampleThree");
        }

        private void Awake()
        {
            if (!DOAwake()) return;

            instance = this;
        }

        private void Start()
        {
            // DOStart can be place as you wish
            base.DOStart();
        }
    }
}