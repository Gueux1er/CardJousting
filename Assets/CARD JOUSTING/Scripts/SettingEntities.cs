using System;
using System.Collections.Generic;
using UnityEngine;

namespace LibLabGames.NewGame
{
    [CreateAssetMenu(fileName = "SettingEntities", menuName = "CardJousting/SettingEntities")]
    public class SettingEntities : ScriptableObject
    {
        [Serializable]
        public struct Entity
        {
            public string nameEntity;
            public List<string> tagList;
            public List<float> evolveTimes;
            public GameObject[] entityPrefabs;
            public Sprite[] entitySprites;
        }

        public Entity[] entities;
    }
}