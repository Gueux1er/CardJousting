using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using TMPro;

namespace LibLabSystem
{
    public class LLPoolManager : MonoBehaviour
    {
        public static LLPoolManager instance;
        
        void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            transform.position = Vector3.zero;
            transform.eulerAngles = Vector3.zero;
        }

        void OnSceneLoaded()
        {
            transform.position = Vector3.zero;
            transform.eulerAngles = Vector3.zero;
        }

        public void DespawnAll()
        {
            PoolManager.Pools["POOL"].DespawnAll();
        }

        public void Despawn(Transform t)
        {
            PoolManager.Pools["POOL"].Despawn(t, transform);
        }
    }
}