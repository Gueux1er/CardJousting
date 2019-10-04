using System;
using UnityEngine;

namespace LibLabGames.NewGame
{
    [CreateAssetMenu(fileName = "SettingEntities", menuName = "CardJousting/SettingEntities")]
    public class SettingEntities : ScriptableObject
    {
        [Serializable]
        public struct Entity
        {
            public string tag;
            public float evolveTime;
            public GameObject[] entityPrefabs;
            public Sprite[] entitySprites;
        }

        public Entity[] entities;
    }
}