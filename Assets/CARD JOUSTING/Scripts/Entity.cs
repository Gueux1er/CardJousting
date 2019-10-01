using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LibLabGames.NewGame
{
    public class Entity : MonoBehaviour
    {
        [Header("Info Entity")]
        public EntityInfo info;

        public int currentLevel;
        public int playerID;
        public int wayID;
        public float speed = 1;
        public float currentSpeed;

        public bool isWalk;
        public bool isReady;

        [Header("Elements")]
        public MeshRenderer coreMeshRenderer;
        public Color[] attackLvlColors;

        public Transform coreTransform;
        public Transform itemRightParent;
        public Transform itemForwardParent;
        public Transform itemLeftParent;

        [Header("Items Prefab")]
        public GameObject attackObjectPrefab;
        public GameObject defenceObjectPrefab;
        public GameObject grabObjectPrefab;

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
            // TO DELETE
            /**/
            info = GameManager.instance.settingEntities.entityInfos[GameManager.instance.debugValue];
            /**/
            currentLevel = 0;
            // TO DELETE

            playerID = player;
            wayID = way;

            speed = (info.unitSpeed != 0) ? info.unitSpeed : 1;
            currentSpeed = speed;

            transform.localPosition = Vector3.zero;

            for (int i = 0; i < info.entityEvolutionStats[currentLevel].entityItems.Length; ++i)
            {
                // TODO pooling system
                GameObject go = Instantiate(
                    // Prefab
                    info.entityEvolutionStats[currentLevel].entityItems[i].itemType == EntityInfo.eItemType.attack ? attackObjectPrefab :
                    info.entityEvolutionStats[currentLevel].entityItems[i].itemType == EntityInfo.eItemType.defence ? defenceObjectPrefab :
                    grabObjectPrefab,
                    // Parent
                    info.entityEvolutionStats[currentLevel].entityItems[i].wayType == EntityInfo.eWayType.right ? itemRightParent :
                    info.entityEvolutionStats[currentLevel].entityItems[i].wayType == EntityInfo.eWayType.forward ? itemForwardParent :
                    itemLeftParent);

                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;

                if (info.entityEvolutionStats[currentLevel].entityItems[i].itemType == EntityInfo.eItemType.attack)
                {
                    go.GetComponent<Item>().attackForce = info.entityEvolutionStats[currentLevel].entityItems[i].level;
                    go.transform.GetChild(0).GetComponent<MeshRenderer>().material.color =
                        attackLvlColors[info.entityEvolutionStats[currentLevel].entityItems[i].level];
                }
            }

            // Repositionnement des items chez les parents
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

            isReady = true;
            isWalk = true;
        }

        private void Update()
        {
            if (!isReady)
                return;

            if (Physics.Raycast(rayFw, out hit, 2.5f, 1 << 8) && !isWalk)
            {
                if (Physics.Raycast(transform.position + transform.forward * 2f, transform.position + transform.forward * 10f, out hit, 3f, 1 << 8) &&
                    playerID == hit.transform.GetComponentInParent<Entity>().playerID &&
                    hit.transform.GetComponentInParent<Entity>().isWalk &&
                    !hit.transform.GetComponentInChildren<Item>().goToEnemyDefence)
                {
                    currentSpeed = hit.transform.GetComponentInParent<Entity>().speed;
                    isWalk = true;
                }
                else
                {
                    return;
                }
            }

            if (currentSpeed != speed)
            {
                if (Physics.Raycast(rayFw, out hit, 2.5f, 1 << 8) &&
                    playerID == hit.transform.GetComponentInParent<Entity>().playerID &&
                    hit.transform.GetComponentInParent<Entity>().isWalk)
                {
                    currentSpeed = hit.transform.GetComponentInParent<Entity>().speed;
                }
                else if (!Physics.SphereCast(transform.position, 2.5f, transform.forward, out hit, 2.5f, 1 << 8))
                {
                    currentSpeed = speed;
                }
            }

            if (isWalk)
                transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        }

        public void DOKillEntity()
        {
            Destroy(gameObject);
        }
    }
}