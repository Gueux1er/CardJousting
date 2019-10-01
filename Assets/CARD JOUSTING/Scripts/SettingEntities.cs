using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LibLabGames.NewGame
{
    [CreateAssetMenu(fileName = "SettingEntities", menuName = "CardJousting/SettingEntities")]
    public class SettingEntities : ScriptableObject
    {
        public List<EntityInfo> entityInfos;
    }
}