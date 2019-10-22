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

        public bool gameIsReady;

        public SettingEntities settingEntities;

        public GameObject entityPrefab;

        [System.Serializable]
        public struct PlayerInfo
        {
            public int maxLife;
            public int currentLife;
            public Slider lifeSlider;
            public List<bool> canSpawnOnWay;
            public List<Image> spawnCooldownFadeImage;
            public List<Transform> spawnTransforms;
            public Entity[] lastEntitiesOnGameBoard;
            public List<Entity> entitiesOnGameBoard;
            public TextMeshProUGUI gamePhaseTimerText;
            public Image evolutionBackgroundCurrentImage;
            public Image evolutionBackgroundNextImage;
            public Image evolutionProgressionImage;
            public Image evolutionCurrentStatImage;
            public Image evolutionNextStatImage;
            public string entityOnTraining;
            public List<string> entitiesLevelTwo; // tag des cartes NFC
            public List<string> entitiesLevelThree;

            public KeyCode[] DEBUG_selectionEntityKeys;
            public KeyCode[] DEBUG_summonKeys;
            public KeyCode DEBUG_evolveKey;
            public string[] DEBUG_cardOnHand;
            public string DEBUG_entityOn;
        }
        public PlayerInfo[] playerInfos;

        public bool isDrawPhase;
        public CanvasGroup drawPhaseDisplay;
        public TextMeshProUGUI[] drawPhaseTimerTexts;

        public CanvasGroup gameOverDisplay;
        public TextMeshProUGUI[] playerGameOverTexts;

        public KeyCode[] debugKeyCodeSpawn;
        public KeyCode[] debugKeyCodeSpawnBis;
        public KeyCode[] debugKeyCodeEvolution;
        public int debugValue;

        private float cooldownSpawnValue;
        private int gamePhaseTimer;
        private int drawPhaseTimer;

        public override void GetSettingGameValues()
        {
            cooldownSpawnValue = settingValues.GetFloatValue("cooldownSpawn");
            //gamePhaseTimer = (int) settingValues.GetFloatValue("gamePhaseTimer");
            gamePhaseTimer = 60;
            drawPhaseTimer = 7;
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
                DisableEvolveUI(i);
                playerInfos[i].entityOnTraining = string.Empty;

                playerInfos[i].lastEntitiesOnGameBoard = new Entity[3];

                for (int j = 0; j < playerInfos[i].spawnCooldownFadeImage.Count; ++j)
                {
                    playerInfos[i].currentLife = playerInfos[i].maxLife;
                    playerInfos[i].canSpawnOnWay.Add(true);
                    playerInfos[i].spawnCooldownFadeImage[j].gameObject.SetActive(false);
                }
            }

            evolutionCoco = new IEnumerator[2];

            drawPhaseDisplay.alpha = 0;
            gameOverDisplay.alpha = 0;

            // TODO écran d'attente des joueurs (check deck)

            DoStartGame();
        }

        public void DoStartGame()
        {
            gameIsReady = true;

            SoundManager.instance.StartGame();

            EnableGamePhase();
        }

        int phaseTimer;
        private void EnableGamePhase()
        {
            isDrawPhase = false;

            phaseTimer = gamePhaseTimer;
            DOTween.To(() => phaseTimer, x => phaseTimer = x, 0, gamePhaseTimer).SetEase(Ease.Linear)
                .OnComplete(() => EnableDrawPhase());
        }

        private void EnableDrawPhase()
        {
            SoundManager.instance.StartDrawPhase();

            isDrawPhase = true;

            drawPhaseDisplay.DOFade(1f, 1f);

            phaseTimer = drawPhaseTimer;

            DOTween.To(() => phaseTimer, x => phaseTimer = x, 0, drawPhaseTimer).SetEase(Ease.Linear)
                .OnUpdate(() =>
                {
                    for (int i = 0; i < 2; ++i)
                    {
                        drawPhaseTimerTexts[i].text = phaseTimer.ToString("00");
                    };
                })
                .OnComplete(() =>
                {
                    SoundManager.instance.EndDrawPhase();
                    drawPhaseDisplay.DOFade(0f, 0.2f);
                    EnableGamePhase();
                });
        }

        private int currentPlayer;
        private Entity entitySpwaned;
        private GameObject entityGo;
        private void Update()
        {
            if (isDrawPhase || !gameIsReady)
                return;

            for (int i = 0; i < playerInfos.Length; ++i)
            {
                playerInfos[i].gamePhaseTimerText.text = phaseTimer.ToString("00");
                playerInfos[i].lifeSlider.value = (float) playerInfos[i].currentLife / (float) playerInfos[i].maxLife;

                // DEBUG Sélection de carte
                for (int j = 0; j < playerInfos[i].DEBUG_cardOnHand.Length; ++j)
                {
                    if (Input.GetKeyDown(playerInfos[i].DEBUG_selectionEntityKeys[j]))
                    {
                        playerInfos[i].DEBUG_entityOn = playerInfos[i].DEBUG_cardOnHand[j];
                    }
                }

                // DEBUG invocation de la carte
                for (int j = 0; j < playerInfos[i].DEBUG_summonKeys.Length; ++j)
                {
                    if (playerInfos[i].DEBUG_entityOn != string.Empty && Input.GetKeyDown(playerInfos[i].DEBUG_summonKeys[j]))
                    {
                        NFC_Activate(i);

                        int entityLevel = 0;

                        // Chercher si la carte a été évolué au niveau 2
                        if (playerInfos[i].entitiesLevelTwo.Find(x => x.Contains(playerInfos[i].DEBUG_entityOn)) != null)
                        {
                            entityLevel = 1;
                            for (int k = 0; k < playerInfos[i].entitiesLevelTwo.Count; ++k)
                            {
                                if (playerInfos[i].entitiesLevelTwo[k] == playerInfos[i].DEBUG_entityOn)
                                {
                                    playerInfos[i].entitiesLevelTwo[k] = string.Empty;
                                }
                            }
                        }
                        // Chercher si la carte a été évolué au niveau 3
                        if (playerInfos[i].entitiesLevelThree.Find(x => x.Contains(playerInfos[i].DEBUG_entityOn)) != null)
                        {
                            entityLevel = 2;
                            for (int k = 0; k < playerInfos[i].entitiesLevelThree.Count; ++k)
                            {
                                if (playerInfos[i].entitiesLevelThree[k] == playerInfos[i].DEBUG_entityOn)
                                {
                                    playerInfos[i].entitiesLevelThree[k] = string.Empty;
                                }
                            }
                        }

                        // Chercher si la carte invoquée est dans la chambre d'évolution
                        if (playerInfos[i].entityOnTraining == playerInfos[i].DEBUG_entityOn)
                        {
                            playerInfos[i].entityOnTraining = string.Empty;

                            if (evolutionCoco[i] != null)
                                StopCoroutine(evolutionCoco[i]);
                            DisableEvolution(i);
                        }

                        // Cherche le gameObject à invoquer
                        foreach (SettingEntities.Entity entity in settingEntities.entities)
                        {
                            if (entity.tag == playerInfos[i].DEBUG_entityOn)
                            {
                                entityGo = entity.entityPrefabs[entityLevel];
                            }
                        }
                        if (entityGo == null)
                        {
                            LLLog.LogE("Spawn Entity", "No found gameObject to spawn!");
                        }

                        SpawnEntity(entityGo, i, j);

                        entityGo = null;

                        // DEBUG Enlève la carte de la main
                        for (int k = 0; k < playerInfos[i].DEBUG_cardOnHand.Length; ++k)
                        {
                            if (playerInfos[i].DEBUG_cardOnHand[k] == playerInfos[i].DEBUG_entityOn)
                            {
                                playerInfos[i].DEBUG_cardOnHand[k] = string.Empty;
                            }
                        }

                        playerInfos[i].DEBUG_entityOn = string.Empty;
                    }
                }
                // DEBUG évolution d'une carte
                if (Input.GetKeyDown(playerInfos[i].DEBUG_evolveKey))
                {
                    if (playerInfos[i].entityOnTraining == string.Empty && playerInfos[i].DEBUG_entityOn != string.Empty)
                    {
                        playerInfos[i].entityOnTraining = playerInfos[i].DEBUG_entityOn;
                        playerInfos[i].DEBUG_entityOn = string.Empty;

                        NFC_Activate(i);
                        ActiveEvolution(i);

                        if (evolutionCoco[i] != null)
                            StopCoroutine(evolutionCoco[i]);

                        evolutionCoco[i] = EvolutionEnum(i, playerInfos[i].entityOnTraining);
                        StartCoroutine(evolutionCoco[i]);
                    }

                    else if (playerInfos[i].entityOnTraining != string.Empty)
                    {
                        playerInfos[i].entityOnTraining = string.Empty;

                        if (evolutionCoco[i] != null)
                            StopCoroutine(evolutionCoco[i]);

                        DisableEvolution(i);
                    }
                }
            };
        }

        private void NFC_Activate(int player)
        {
            if (player == 0)
                SoundManager.instance.NFCDetectLeft();

            else
                SoundManager.instance.NFCDetectRight();
        }

        public void SpawnEntity(GameObject go, int player, int way, bool isSecondEntity = false)
        {
            print(go.name);
            entitySpwaned = Instantiate(go, playerInfos[player].spawnTransforms[way]).GetComponent<Entity>();

            playerInfos[player].entitiesOnGameBoard.Add(entitySpwaned);

            CooldownSpawn(player, way);

            if (playerInfos[player].lastEntitiesOnGameBoard[way] != null)
            {
                playerInfos[player].lastEntitiesOnGameBoard[way].behindEntity = entitySpwaned;
                entitySpwaned.nextEntity = playerInfos[player].lastEntitiesOnGameBoard[way];
            }

            playerInfos[player].lastEntitiesOnGameBoard[way] = entitySpwaned;

            entitySpwaned.DOSpawn(player, way, isSecondEntity);
        }

        private IEnumerator[] evolutionCoco;
        private IEnumerator EvolutionEnum(int player, string cardID)
        {
            while (playerInfos[player].entityOnTraining != null)
            {
                LLLog.Log("GameManager", string.Format("Entity {0} try to evolve.", cardID));

                bool canEvolve = true;
                int currentLevel = 0;
                float duration = 0;
                Sprite[] statSprites = new Sprite[3];
                foreach (SettingEntities.Entity entity in settingEntities.entities)
                {
                    if (entity.tag == cardID)
                    {
                        if (playerInfos[player].entitiesLevelTwo.Find(x => x.Contains(cardID)) != null)
                            currentLevel = 1;
                        else if (playerInfos[player].entitiesLevelThree.Find(x => x.Contains(cardID)) != null)
                            currentLevel = 2;

                        statSprites = entity.entitySprites;
                        playerInfos[player].evolutionCurrentStatImage.sprite = statSprites[currentLevel];
                        if (currentLevel + 1 != statSprites.Length)
                            playerInfos[player].evolutionNextStatImage.sprite = statSprites[currentLevel + 1];

                        canEvolve = entity.evolveTime > 0;

                        if (!canEvolve)
                            break;

                        if (entity.entityPrefabs.Length == 2 && playerInfos[player].entitiesLevelTwo.Find(x => x.Contains(cardID)) != null)
                            canEvolve = false;
                        else if (entity.entityPrefabs.Length == 3 && playerInfos[player].entitiesLevelThree.Find(x => x.Contains(cardID)) != null)
                            canEvolve = false;

                        if (canEvolve)
                            duration = entity.evolveTime;
                    }
                }

                SoundManager.instance.Evolve((SoundManager.Player) player, duration);

                // Si créature ne peut pas évoluer (déjà au niveau max)
                if (!canEvolve)
                {
                    playerInfos[player].evolutionNextStatImage.sprite = statSprites[statSprites.Length - 1];
                    CannotMoreEvolve(player);

                    while (true)
                    {
                        yield return null;
                    }
                }
                // Si créature peut évoluer au niveau 2 ou 3
                else
                {
                    float startTime = Time.time;
                    float t = 0;

                    while (Time.time - startTime <= duration)
                    {
                        t += Time.deltaTime / duration;
                        playerInfos[player].evolutionProgressionImage.fillAmount = Mathf.Lerp(0, 1, t);

                        yield return null;
                    }

                    if (playerInfos[player].entitiesLevelTwo.Find(x => x.Contains(cardID)) == null)
                        playerInfos[player].entitiesLevelTwo.Add(cardID);
                    else if (playerInfos[player].entitiesLevelThree.Find(x => x.Contains(cardID)) == null)
                        playerInfos[player].entitiesLevelThree.Add(cardID);
                    else
                        LLLog.LogE("GameManager", string.Format("Entity {0} try to evolve higher than level 3.", cardID));

                    playerInfos[player].evolutionProgressionImage.fillAmount = 0;

                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        private void CooldownSpawn(int player, int way)
        {
            playerInfos[player].canSpawnOnWay[way] = false;

            playerInfos[player].spawnCooldownFadeImage[way].gameObject.SetActive(true);

            playerInfos[player].spawnCooldownFadeImage[way].DOKill();
            playerInfos[player].spawnCooldownFadeImage[way].color = new Color(1f, 0, 0, 1f);
            playerInfos[player].spawnCooldownFadeImage[way].DOColor(new Color(1f, 0, 0, 0.5f), 1f)
                .OnComplete(() =>
                {
                    playerInfos[player].canSpawnOnWay[way] = true;
                    playerInfos[player].spawnCooldownFadeImage[way].color = new Color(1f, 0.7f, 0, 0.5f);
                    playerInfos[player].spawnCooldownFadeImage[way].DOColor(new Color(1f, 0.7f, 0, 0f), 0.3f)
                        .SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                        {
                            playerInfos[player].spawnCooldownFadeImage[way].gameObject.SetActive(false);
                        });
                });
        }

        private void ActiveEvolution(int player)
        {
            playerInfos[player].evolutionCurrentStatImage.gameObject.SetActive(true);
            playerInfos[player].evolutionNextStatImage.gameObject.SetActive(true);
            playerInfos[player].evolutionProgressionImage.fillAmount = 0;

            playerInfos[player].evolutionBackgroundCurrentImage.color = Color.white;
            playerInfos[player].evolutionBackgroundNextImage.color = Color.white;
        }

        private void DisableEvolution(int player)
        {
            SoundManager.instance.CancelEvolve((SoundManager.Player) player);

            DisableEvolveUI(player);
        }

        private void DisableEvolveUI(int player)
        {
            playerInfos[player].evolutionCurrentStatImage.gameObject.SetActive(false);
            playerInfos[player].evolutionNextStatImage.gameObject.SetActive(false);
            playerInfos[player].evolutionProgressionImage.fillAmount = 0;

            playerInfos[player].evolutionBackgroundCurrentImage.color = Color.grey;
            playerInfos[player].evolutionBackgroundNextImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        private void CannotMoreEvolve(int player)
        {
            playerInfos[player].evolutionCurrentStatImage.gameObject.SetActive(true);
            playerInfos[player].evolutionNextStatImage.gameObject.SetActive(false);
            playerInfos[player].evolutionProgressionImage.fillAmount = 0;

            playerInfos[player].evolutionBackgroundCurrentImage.color = Color.white;
            playerInfos[player].evolutionBackgroundNextImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        public void HurtPlayer(int player, int value)
        {
            playerInfos[player].currentLife -= value;

            SoundManager.instance.BaseDamage();

            if (playerInfos[player].currentLife <= 0)
                OnGameOver(player);
        }

        public void OnGameOver(int loserPlayer)
        {
            gameIsReady = false;

            gameOverDisplay.alpha = 0;
            gameOverDisplay.DOFade(1f, 0.5f);

            for (int i = 0; i < 2; ++i)
            {
                playerGameOverTexts[i].text = (i == loserPlayer) ? "Loser!" : "Winner!";
                playerGameOverTexts[i].color = Color.clear;
                playerGameOverTexts[i].DOColor((i == loserPlayer) ? Color.red : Color.yellow, 0.5f);
            }
        }

        GameObject entityToSpawn;
        public void ReadCardNFC_GameBoard(string cardID, int playerID, int wayID)
        {
            int entityLevel = 0;

            if (playerInfos[playerID].entitiesLevelTwo.Find(x => x.Contains(cardID)) != null)
                entityLevel = 1;
            if (playerInfos[playerID].entitiesLevelThree.Find(x => x.Contains(cardID)) != null)
                entityLevel = 2;

            foreach (SettingEntities.Entity entity in settingEntities.entities)
            {
                if (entity.tag == cardID)
                {
                    entityToSpawn = entity.entityPrefabs[entityLevel];
                }
            }

            SpawnEntity(entityToSpawn, playerID, wayID);
        }

        public void ReadCardNFC_Training(string cardID, int playerID)
        {

        }
    }
}