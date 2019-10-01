using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LibLabGames.NewGame
{
    [System.Serializable]
    public struct EntityInfo
    {
        [System.Serializable]
        public struct EntityEvolutionStat
        {
            public EntityItem[] entityItems;
        }
        [System.Serializable]
        public struct EntityItem
        {
            public eItemType itemType;
            public eWayType wayType;
            public int level;
        }
        public enum eItemType { attack, defence, grab }
        public enum eWayType { right, forward, left }

        public string ID;
        public float unitCasting;
        public float unitSpeed;
        public EntityEvolutionStat[] entityEvolutionStats;
    }
}