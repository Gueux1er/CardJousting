using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LibLabGames.NewGame
{
    public class Entity : MonoBehaviour
    {
        [Header("Control Values")]
        public float distanceWithNextEntity;

        [Header("Infos To Fill")]
        public float speed = 1;
        public bool isSecondEntity;
        public GameObject secondEntityPrefab;

        [Header("On Play Info")]
        public bool isReady;

        public int currentEvolutionLevel;
        public int playerID;
        public int wayID;
        public float currentSpeed;

        public bool isWalk;
        public bool isOvertake;
        public bool haveItemOnSide;

        public Entity nextEntity;
        public Entity behindEntity;
        public Entity enemyEntity;

        [Header("Elements")]
        public GameObject spriteObject;
        public MeshRenderer coreMeshRenderer;
        public Color[] attackLvlColors;

        public Transform coreTransform;
        public Transform itemRightParent;
        public Transform itemForwardParent;
        public Transform itemLeftParent;

        RaycastHit hit;
        Ray m_rayFw;
        Ray rayFw { get { return _RayFw(); } }
        Ray _RayFw()
        {
            m_rayFw.origin = transform.position;
            m_rayFw.direction = (itemForwardParent.position - transform.position).normalized;
            return m_rayFw;
        }

        private void OnDrawGizmos()
        {
            Debug.DrawLine(rayFw.origin, rayFw.origin + rayFw.direction, Color.blue);
        }

        public void DOSpawn(int player, int way)
        {
            spriteObject.SetActive(!GameManager.instance.DEBUG_displayMeshArm);
            coreMeshRenderer.enabled = GameManager.instance.DEBUG_displayMeshArm;

            playerID = player;
            wayID = way;

            transform.localPosition = Vector3.zero;

            if (isSecondEntity)
            {
                transform.localPosition += Vector3.back * distanceWithNextEntity;
            }

            currentSpeed = speed;

            // Repositionnement des items dans leurs parents
            for (int i = 0; i < itemRightParent.childCount; ++i)
            {
                itemRightParent.GetChild(i).localPosition += Vector3.forward * i;
            }
            for (int i = 0; i < itemForwardParent.childCount; ++i)
            {
                itemForwardParent.GetChild(i).localPosition += Vector3.forward * i;
            }
            for (int i = 0; i < itemLeftParent.childCount; ++i)
            {
                itemLeftParent.GetChild(i).localPosition += Vector3.forward * i;
            }

            if (secondEntityPrefab != null)
            {
                GameManager.instance.SpawnSecondEntity(secondEntityPrefab, playerID, wayID);
            }

            isReady = true;
            
            CheckCanWalk();
        }

        private void Update()
        {
            if (!isReady || GameManager.instance.isDrawPhase || !GameManager.instance.gameIsReady)
                return;

            if (!isWalk)
            {
                if (nextEntity != null)
                    CheckCanWalk();

                return;
            }

            if (currentSpeed != speed)
                CheckCanWalk();

            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        }

        public void CheckCanWalk()
        {
            if (isOvertake)
                return;

            if (enemyEntity != null)
            {
                if (enemyEntity.isReady)
                {
                    isWalk = false;
                    currentSpeed = 0;
                }
            }

            else if (!isWalk || currentSpeed != speed)
            {
                if (nextEntity == null || !nextEntity.isReady)
                {
                    isWalk = true;
                    currentSpeed = speed;
                }

                else if ((nextEntity.transform.X() - transform.X()) * ((playerID == 0) ? 1 : -1) > distanceWithNextEntity)
                {
                    isWalk = true;

                    if (currentSpeed > nextEntity.currentSpeed && (nextEntity.transform.X() - transform.X()) * ((playerID == 0) ? 1 : -1) < (distanceWithNextEntity + 0.5f))
                        currentSpeed = nextEntity.currentSpeed;
                    else
                        currentSpeed = speed;
                }

                else
                {
                    isWalk = false;

                    if (nextEntity.currentSpeed < currentSpeed)
                        currentSpeed = nextEntity.currentSpeed;
                    else
                        currentSpeed = speed;
                }
            }

            if (behindEntity != null)
            {
                behindEntity.CheckCanWalk();
            }
        }

        private Entity colEntity;
        private void OnTriggerEnter(Collider col)
        {
            if (!isReady)
                return;

            if (col.CompareTag(string.Format("GoalPlayer{0}", playerID)))
            {
                DOKillEntity();

                return;
            }

            if (col.CompareTag("Entity"))
            {
                colEntity = col.GetComponentInParent<Entity>();

                if (colEntity == this)
                    return;

                if (colEntity.playerID != playerID)
                {
                    colEntity.DOKillEntity();
                    DOKillEntity();
                }
                else if (!isOvertake)
                {
                    currentSpeed = colEntity.currentSpeed;
                    isWalk = false;
                }
            }
        }

        public void HurtPlayer(int value)
        {
            GameManager.instance.HurtPlayer(playerID == 0 ? 1 : 0, value);
            DOKillEntity();
        }

        public void DOKillEntity()
        {
            if (!isReady)
                return;

            isReady = false;

            if (behindEntity != null)
                behindEntity.CheckCanWalk();

            Destroy(gameObject);
        }
    }
}