using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LibLabSystem;
using PathologicalGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LibLabGames.NewGame
{
    public class GameManager : IGameManager
    {
        public static GameManager instance;

        public SettingEntities settingEntities;

        public GameObject entityPrefab;

        [System.Serializable]
        public struct PlayerInfo
        {
            public int life;
            public bool canSpawn;
            public Image spawnCooldownFadeImage;
            public Image spawnCooldownImage;
            public List<Transform> spawnTransforms;
            public Entity lastEntityOnGameBoard;
            public List<Entity> entitiesOnGameBoard;
            public Entity entityOnTraining;
        }
        public PlayerInfo[] playerInfos;

        public KeyCode[] debugKeyCodeSpawn;
        public KeyCode[] debugKeyCodeSpawnBis;
        public int debugValue;

        private float cooldownSpawnValue;

        public override void GetSettingGameValues()
        {
            cooldownSpawnValue = settingValues.GetFloatValue("cooldownSpawn");
        }

        private void Awake()
        {
            if (!DOAwake()) return;

            instance = this;
        }

        private void Start()
        {
            base.DOStart();

            GetSettingGameValues();

            for (int i = 0; i < playerInfos.Length; ++i)
            {
                playerInfos[i].canSpawn = true;
                playerInfos[i].spawnCooldownFadeImage.gameObject.SetActive(false);
                playerInfos[i].spawnCooldownImage.gameObject.SetActive(false);
            }
        }

        private int currentPlayer;
        private Entity entitySpwaned;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                currentPlayer = 0;
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                currentPlayer = 1;

            for (int i = 0; i < debugKeyCodeSpawnBis.Length; ++i)
            {
                if (Input.GetKeyDown(debugKeyCodeSpawnBis[i]))
                {
                    debugValue = i;
                }
            }

            if (playerInfos[currentPlayer].canSpawn)
            {
                for (int i = 0; i < debugKeyCodeSpawn.Length; ++i)
                {
                    if (Input.GetKeyDown(debugKeyCodeSpawn[i]))
                    {
                        entitySpwaned = Instantiate(entityPrefab, playerInfos[currentPlayer].spawnTransforms[i]).GetComponent<Entity>();
                        entitySpwaned.DOSpawn(currentPlayer, i);
                        playerInfos[currentPlayer].lastEntityOnGameBoard = entitySpwaned;
                        playerInfos[currentPlayer].entitiesOnGameBoard.Add(entitySpwaned);

                        cooldownEnumCoco[currentPlayer] = CooldownEnum(currentPlayer);
                        StartCoroutine(cooldownEnumCoco[currentPlayer]);
                    }
                }
            }
        }

        IEnumerator[] cooldownEnumCoco = new IEnumerator[2];
        IEnumerator CooldownEnum(int p)
        {
            playerInfos[p].canSpawn = false;

            playerInfos[p].spawnCooldownFadeImage.gameObject.SetActive(true);
            playerInfos[p].spawnCooldownImage.gameObject.SetActive(true);

            playerInfos[p].spawnCooldownFadeImage.DOKill();
            playerInfos[p].spawnCooldownFadeImage.DOColor(new Color(1, 0, 0, 0.5f), 0.3f);

            playerInfos[p].spawnCooldownImage.fillAmount = 1;
            playerInfos[p].spawnCooldownImage.transform.DOScale(0, 0.3f).From();

            float startTime = Time.time;
            while (Time.time - startTime < cooldownSpawnValue)
            {
                float t = (Time.time - startTime) / cooldownSpawnValue;

                playerInfos[p].spawnCooldownImage.fillAmount = 1 - t;

                yield return null;
            }

            playerInfos[p].spawnCooldownFadeImage.DOKill();
            playerInfos[p].spawnCooldownFadeImage.DOColor(new Color(1, 0, 0, 0), 0.2f)
                .OnComplete(() => playerInfos[p].spawnCooldownFadeImage.gameObject.SetActive(false));
                
            playerInfos[p].spawnCooldownImage.gameObject.SetActive(false);
            playerInfos[p].canSpawn = true;
        }

        public void ReadCardNFC_GameBoard(string info)
        {

        }

        public void ReadCardNFC_Training(string info)
        {

        }

        public void WriteCardNFC_Training()
        {

        }
    }
}