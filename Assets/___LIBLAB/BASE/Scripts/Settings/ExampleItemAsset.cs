using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LibLabGames.NewGame
{
    // Example of a GameObject list on a ScriptableObject with Get function

    [CreateAssetMenu(fileName = "ItemAsset", menuName = "LibLab/ItemAsset")]
    public class ExampleItemAsset : ScriptableObject
    {
        public List<GameObject> items;

        public GameObject GetItem(int index)
        {
            return items[index];
        }
    }
}