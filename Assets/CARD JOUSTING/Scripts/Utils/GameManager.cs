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

        [Header("DEBUG Additionals")]
        public bool DEBUG_displayMeshArm;

        [Header("Elements")]
        public bool gameIsReady;

        public SettingEntities settingEntities;

        public GameObject entityPrefab;

        [System.Serializable]
        public struct PlayerInfo
        {
            public int maxLife;
            public int currentLife;
            public Transform lifeParent;
            public Image backgroundLifeImage;
            public Slider lifeSlider;
            public List<bool> canSpawnOnWay;
            public List<Image> spawnCooldownFadeImage;
            public List<Transform> spawnTransforms;
            public Entity[] lastEntitiesOnGameBoard;
            public List<Entity> entitiesOnGameBoard;
            public TextMeshProUGUI gamePhaseTimerText;
            public RectTransform evolutionTransformParent;
            public RectTransform evolutionDisableTarget;
            public RectTransform evolutionActiveTarget;
            public RectTransform evolutionNextStatTransformParent;
            public RectTransform evolutionNextStatDisableTarget;
            public RectTransform evolutionNextStatActiveTarget;
            public Slider evolutionProgressionSlider;
            public Image evolutionCurrentStatImage;
            public Image evolutionNextStatImage;
            public string entityOnTraining;
            public List<string> entitiesLevelTwo; // tag des cartes NFC
            public List<string> entitiesLevelThree;
            public List<string> cardsOnGraveyard;

            public KeyCode[] DEBUG_selectionEntityKeys;
            public KeyCode[] DEBUG_summonKeys;
            public KeyCode DEBUG_evolveKey;
            public string[] DEBUG_cardOnHand;
            public int DEBUG_entityIDOn;
        }
        public PlayerInfo[] playerInfos;

        public Color baseGreyColor;

        public bool isDrawPhase;
        public CanvasGroup drawPhaseDisplay;
        public TextMeshProUGUI[] drawPhaseTimerTexts;

        public CanvasGroup gameOverDisplay;
        public TextMeshProUGUI[] playerGameOverTexts;

        public KeyCode[] debugKeyCodeSpawn;
        public KeyCode[] debugKeyCodeSpawnBis;
        public KeyCode[] debugKeyCodeEvolution;
        public int debugValue;

        [HideInInspector] public float cooldownSpawnValue;
        [HideInInspector] public int gamePhaseTimer;
        [HideInInspector] public int drawPhaseTimer;
        [HideInInspector] public float globalEntitySpeed;

        public override void GetSettingGameValues()
        {
            cooldownSpawnValue = settingValues.GetFloatValue("cooldownSpawn");
            gamePhaseTimer = (int) settingValues.GetFloatValue("gamePhaseTimer");
            drawPhaseTimer = (int) settingValues.GetFloatValue("drawPhaseTimer");
            globalEntitySpeed = settingValues.GetFloatValue("globalEntitySpeed");
        }

        private void Awake()
        {
            if (!DOAwake()) return;

            instance = this;
        }

        private void Start()
        {
            base.DOStart();

            SoundManager.instance.ResetSound();

            GetSettingGameValues();

            for (int i = 0; i < playerInfos.Length; ++i)
            {
                DisableEvolveUI(i);
                playerInfos[i].entityOnTraining = string.Empty;

                playerInfos[i].lastEntitiesOnGameBoard = new Entity[3];
                playerInfos[i].DEBUG_entityIDOn = -1;

                for (int j = 0; j < playerInfos[i].spawnCooldownFadeImage.Count; ++j)
                {
                    playerInfos[i].currentLife = playerInfos[i].maxLife;
                    playerInfos[i].canSpawnOnWay.Add(true);
                    playerInfos[i].spawnCooldownFadeImage[j].gameObject.SetActive(false);
                    playerInfos[i].evolutionTransformParent.position = playerInfos[i].evolutionDisableTarget.position;
                    playerInfos[i].evolutionNextStatTransformParent.position = playerInfos[i].evolutionDisableTarget.position;
                }
            }

            evolutionCoco = new IEnumerator[2];
            checkEvolutionActiveCoco = new IEnumerator[2];

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
                .OnComplete(() =>
                {
                    if (gameIsReady)
                        EnableDrawPhase();
                });
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

        private Entity entitySpwaned;
        private void Update()
        {
            if (isDrawPhase || !gameIsReady)
                return;

            for (int i = 0; i < playerInfos.Length; ++i)
            {
                playerInfos[i].gamePhaseTimerText.text = phaseTimer.ToString("00");

                // DEBUG Sélection de carte
                for (int j = 0; j < playerInfos[i].DEBUG_cardOnHand.Length; ++j)
                {
                    if (Input.GetKeyDown(playerInfos[i].DEBUG_selectionEntityKeys[j]) && playerInfos[i].DEBUG_cardOnHand[j] != string.Empty)
                    {
                        playerInfos[i].DEBUG_entityIDOn = j;
                        break;
                    }
                }

                // DEBUG invocation de la carte
                for (int j = 0; j < playerInfos[i].DEBUG_summonKeys.Length; ++j)
                {
                    if (playerInfos[i].DEBUG_entityIDOn != -1 && Input.GetKeyDown(playerInfos[i].DEBUG_summonKeys[j]))
                    {
                        SpawnEntity(playerInfos[i].DEBUG_cardOnHand[playerInfos[i].DEBUG_entityIDOn], i, j);

                        entityGo = null;

                        // DEBUG Enlève la carte de la main
                        playerInfos[i].DEBUG_cardOnHand[playerInfos[i].DEBUG_entityIDOn] = string.Empty;

                        playerInfos[i].DEBUG_entityIDOn = -1;
                    }
                }
                // DEBUG évolution d'une carte
                if (Input.GetKeyDown(playerInfos[i].DEBUG_evolveKey))
                {
                    if (playerInfos[i].entityOnTraining == string.Empty && playerInfos[i].DEBUG_entityIDOn != -1)
                    {
                        playerInfos[i].entityOnTraining = playerInfos[i].DEBUG_cardOnHand[playerInfos[i].DEBUG_entityIDOn];
                        playerInfos[i].DEBUG_entityIDOn = -1;

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

        private GameObject entityGo;
        public void SpawnEntity(string cardID, int player, int way)
        {
            if (playerInfos[player].cardsOnGraveyard.Find(x => x.Contains(cardID)) != null)
                return;

            playerInfos[player].cardsOnGraveyard.Add(cardID);

            NFC_Activate(player);

            int entityLevel = 0;

            // Chercher si la carte a été évolué au niveau 2
            if (playerInfos[player].entitiesLevelTwo.Find(x => x.Contains(cardID)) != null)
            {
                entityLevel = 1;
                for (int i = 0; i < playerInfos[player].entitiesLevelTwo.Count; ++i)
                {
                    if (playerInfos[player].entitiesLevelTwo[i] == cardID)
                    {
                        playerInfos[player].entitiesLevelTwo[i] = string.Empty;
                    }
                }
            }
            // Chercher si la carte a été évolué au niveau 3
            if (playerInfos[player].entitiesLevelThree.Find(x => x.Contains(cardID)) != null)
            {
                entityLevel = 2;
                for (int i = 0; i < playerInfos[player].entitiesLevelThree.Count; ++i)
                {
                    if (playerInfos[player].entitiesLevelThree[i] == cardID)
                    {
                        playerInfos[player].entitiesLevelThree[i] = string.Empty;
                    }
                }
            }

            // Chercher si la carte invoquée est dans la chambre d'évolution
            if (playerInfos[player].entityOnTraining == cardID)
            {
                playerInfos[player].entityOnTraining = string.Empty;

                if (evolutionCoco[player] != null)
                    StopCoroutine(evolutionCoco[player]);
                DisableEvolution(player);
            }

            // Cherche le gameObject à invoquer
            foreach (SettingEntities.Entity entity in settingEntities.entities)
            {
                for (int tg = 0; tg < entity.tagList.Count; ++tg)
                {
                    if (entity.tagList[tg] == cardID)
                    {
                        entityGo = entity.entityPrefabs[entityLevel];
                    }
                }
            }
            if (entityGo == null)
            {
                LLLog.LogE("Spawn Entity", string.Format("No found gameObject to spawn! CardID : {0}, Way : {1}", cardID, way));
            }

            //print(go.name);
            entitySpwaned = Instantiate(entityGo, playerInfos[player].spawnTransforms[way]).GetComponent<Entity>();
            entityGo = null;

            playerInfos[player].entitiesOnGameBoard.Add(entitySpwaned);

            if (playerInfos[player].lastEntitiesOnGameBoard[way] != null)
            {
                playerInfos[player].lastEntitiesOnGameBoard[way].behindEntity = entitySpwaned;
                entitySpwaned.nextEntity = playerInfos[player].lastEntitiesOnGameBoard[way];
            }

            playerInfos[player].lastEntitiesOnGameBoard[way] = entitySpwaned;

            entitySpwaned.DOSpawn(player, way);

            CooldownSpawn(player, way);
        }

        public void SpawnSecondEntity(GameObject entityPrefab, int player, int way)
        {
            entitySpwaned = Instantiate(entityPrefab, playerInfos[player].spawnTransforms[way]).GetComponent<Entity>();

            playerInfos[player].entitiesOnGameBoard.Add(entitySpwaned);

            if (playerInfos[player].lastEntitiesOnGameBoard[way] != null)
            {
                playerInfos[player].lastEntitiesOnGameBoard[way].behindEntity = entitySpwaned;
                entitySpwaned.nextEntity = playerInfos[player].lastEntitiesOnGameBoard[way];
            }

            entitySpwaned.DOSpawn(player, way);

            playerInfos[player].lastEntitiesOnGameBoard[way] = entitySpwaned;
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
                    for (int tg = 0; tg < entity.tagList.Count; ++tg)
                    {
                        if (entity.tagList[tg] == cardID)
                        {
                            if (playerInfos[player].entitiesLevelTwo.Find(x => x.Contains(cardID)) != null && playerInfos[player].entitiesLevelThree.Find(x => x.Contains(cardID)) == null)
                                currentLevel = 1;
                            else if (playerInfos[player].entitiesLevelThree.Find(x => x.Contains(cardID)) != null)
                                currentLevel = 2;

                            statSprites = entity.entitySprites;
                            playerInfos[player].evolutionCurrentStatImage.sprite = statSprites[currentLevel];
                            if (currentLevel + 1 != statSprites.Length)
                                playerInfos[player].evolutionNextStatImage.sprite = statSprites[currentLevel + 1];

                            if (entity.evolveTimes.Count == currentLevel)
                            {
                                canEvolve = false;
                                break;
                            }

                            canEvolve = entity.evolveTimes[currentLevel] > 0;

                            if (!canEvolve)
                                break;

                            if (entity.entityPrefabs.Length == 2 && playerInfos[player].entitiesLevelTwo.Find(x => x.Contains(cardID)) != null)
                                canEvolve = false;
                            else if (entity.entityPrefabs.Length == 3 && playerInfos[player].entitiesLevelThree.Find(x => x.Contains(cardID)) != null)
                                canEvolve = false;

                            if (canEvolve)
                                duration = entity.evolveTimes[currentLevel];
                        }
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
                    //float startTime = Time.time;
                    float t = 0;

                    while (t < 1)
                    {
                        while (isDrawPhase)
                            yield return null;

                        t += Time.deltaTime / duration;
                        playerInfos[player].evolutionProgressionSlider.value = Mathf.Lerp(0, 1, t);

                        yield return null;
                    }

                    if (playerInfos[player].entitiesLevelTwo.Find(x => x.Contains(cardID)) == null)
                    {
                        playerInfos[player].entitiesLevelTwo.Add(cardID);
                        currentLevel = 1;
                    }
                    else if (playerInfos[player].entitiesLevelThree.Find(x => x.Contains(cardID)) == null)
                    {
                        playerInfos[player].entitiesLevelThree.Add(cardID);
                        currentLevel = 2;
                    }
                    else
                        LLLog.LogE("GameManager", string.Format("Entity {0} try to evolve higher than level 3.", cardID));

                    playerInfos[player].evolutionProgressionSlider.value = 0;

                    playerInfos[player].evolutionCurrentStatImage.sprite = statSprites[currentLevel];
                    if (currentLevel + 1 != statSprites.Length)
                    {
                        playerInfos[player].evolutionNextStatImage.sprite = statSprites[currentLevel + 1];
                        playerInfos[player].evolutionNextStatImage.transform.DOScale(1.1f, 0.2f).SetLoops(2, LoopType.Yoyo);
                    }

                    playerInfos[player].evolutionCurrentStatImage.transform.DOScale(1.1f, 0.2f).SetLoops(2, LoopType.Yoyo);

                    yield return new WaitForSeconds(0.3f);
                }
            }
        }

        private void CooldownSpawn(int player, int way)
        {
            playerInfos[player].canSpawnOnWay[way] = false;

            playerInfos[player].spawnCooldownFadeImage[way].gameObject.SetActive(true);

            playerInfos[player].spawnCooldownFadeImage[way].DOKill();
            playerInfos[player].spawnCooldownFadeImage[way].color = new Color(0.7f, 0, 0, 1f);
            playerInfos[player].spawnCooldownFadeImage[way].DOColor(new Color(0.7f, 0, 0, 0.6f), cooldownSpawnValue)
                .OnComplete(() =>
                {
                    playerInfos[player].canSpawnOnWay[way] = true;
                    playerInfos[player].spawnCooldownFadeImage[way].color = new Color(0.85f, 0f, 0, 0.9f);
                    playerInfos[player].spawnCooldownFadeImage[way].DOColor(new Color(0.85f, 0, 0, 0f), 0.5f)
                        .OnComplete(() =>
                        {
                            playerInfos[player].spawnCooldownFadeImage[way].gameObject.SetActive(false);
                        });
                });
        }

        private void ActiveEvolution(int player)
        {
            playerInfos[player].evolutionCurrentStatImage.gameObject.SetActive(true);
            playerInfos[player].evolutionNextStatImage.gameObject.SetActive(true);
            playerInfos[player].evolutionProgressionSlider.value = 0;

            playerInfos[player].evolutionTransformParent.DOKill();
            playerInfos[player].evolutionTransformParent.DOMove(playerInfos[player].evolutionActiveTarget.position, 0.5f);

            playerInfos[player].evolutionNextStatTransformParent.DOKill();
            playerInfos[player].evolutionNextStatTransformParent.DOMove(playerInfos[player].evolutionNextStatActiveTarget.position, 0.7f);

            //playerInfos[player].evolutionBackgroundNextImage.color = Color.white;
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
            playerInfos[player].evolutionProgressionSlider.value = 0;

            playerInfos[player].evolutionTransformParent.DOKill();
            playerInfos[player].evolutionTransformParent.DOMove(playerInfos[player].evolutionDisableTarget.position, 0.5f);

            playerInfos[player].evolutionNextStatTransformParent.DOKill();
            playerInfos[player].evolutionNextStatTransformParent.DOMove(playerInfos[player].evolutionNextStatDisableTarget.position, 0.7f);

            //playerInfos[player].evolutionBackgroundNextImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        private void CannotMoreEvolve(int player)
        {
            playerInfos[player].evolutionCurrentStatImage.gameObject.SetActive(true);
            playerInfos[player].evolutionNextStatImage.gameObject.SetActive(false);
            playerInfos[player].evolutionProgressionSlider.value = 0;

            playerInfos[player].evolutionNextStatTransformParent.DOKill();
            playerInfos[player].evolutionNextStatTransformParent.DOMove(playerInfos[player].evolutionNextStatDisableTarget.position, 0.7f);

            //playerInfos[player].evolutionBackgroundCurrentImage.color = Color.white;
            //playerInfos[player].evolutionBackgroundNextImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        public void HurtPlayer(int player, int value)
        {
            playerInfos[player].currentLife -= value;

            playerInfos[player].lifeParent.DOKill();
            playerInfos[player].lifeParent.localPosition = Vector3.zero;
            playerInfos[player].lifeParent.DOShakePosition(0.2f, 5);

            playerInfos[player].backgroundLifeImage.DOKill();
            playerInfos[player].backgroundLifeImage.color = baseGreyColor;
            playerInfos[player].backgroundLifeImage.DOColor(Color.red, 0.25f).From().SetEase(Ease.InCirc);
            
            playerInfos[player].lifeSlider.DOKill();
            playerInfos[player].lifeSlider.DOValue((float) playerInfos[player].currentLife / (float) playerInfos[player].maxLife, 0.3f);
            //playerInfos[player].lifeSlider.value = (float) playerInfos[player].currentLife / (float) playerInfos[player].maxLife;

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
                playerGameOverTexts[i].text = (i == loserPlayer) ? "" : "Winner!";
                playerGameOverTexts[i].color = Color.clear;
                playerGameOverTexts[i].DOColor((i == loserPlayer) ? Color.red : Color.red, 0.5f);
            }
        }


        public void ReadCardNFC_GameBoard(string cardID, int playerID, int wayID)
        {
            if (isDrawPhase || !gameIsReady || !playerInfos[playerID].canSpawnOnWay[wayID])
                return;

            SpawnEntity(cardID, playerID, wayID);
        }

        public void ReadCardNFC_Training(string cardID, int playerID)
        {
            if (isDrawPhase || !gameIsReady || playerInfos[playerID].cardsOnGraveyard.Find(x => x.Contains(cardID)) != null)
                return;

            if (checkEvolutionActiveCoco[playerID] != null)
                StopCoroutine(checkEvolutionActiveCoco[playerID]);

            checkEvolutionActiveCoco[playerID] = checkEvolutionActive(playerID);
            StartCoroutine(checkEvolutionActiveCoco[playerID]);

            if (playerInfos[playerID].entityOnTraining == string.Empty)
            {
                playerInfos[playerID].entityOnTraining = cardID;

                NFC_Activate(playerID);
                ActiveEvolution(playerID);

                if (evolutionCoco[playerID] != null)
                    StopCoroutine(evolutionCoco[playerID]);

                evolutionCoco[playerID] = EvolutionEnum(playerID, playerInfos[playerID].entityOnTraining);
                StartCoroutine(evolutionCoco[playerID]);
            }
        }

        private IEnumerator[] checkEvolutionActiveCoco;
        private IEnumerator checkEvolutionActive(int playerID)
        {
            yield return new WaitForSeconds(1.5f);

            playerInfos[playerID].entityOnTraining = string.Empty;

            if (evolutionCoco[playerID] != null)
                StopCoroutine(evolutionCoco[playerID]);

            DisableEvolution(playerID);
        }
    }
}