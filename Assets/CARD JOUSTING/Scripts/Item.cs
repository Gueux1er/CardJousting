using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LibLabGames.NewGame
{
    public class Item : MonoBehaviour
    {
        public enum eTypeItem { attack, defence, grab }
        public eTypeItem typeItem;
        public int attackForce;
        
        public bool goToEnemyDefence;

        private Entity entity;

        private int playerID;
        private int wayID;
        private int positionID;

        private void Start()
        {
            entity = GetComponentInParent<Entity>();
            playerID = entity.playerID;
            wayID = transform.parent.GetSiblingIndex();
            positionID = transform.GetSiblingIndex();
        }

        private Entity colEntity;
        private Item colItem;
        private RaycastHit[] hits;
        private void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("Entity"))
            {
                colEntity = col.GetComponentInParent<Entity>();

                if (colEntity == entity || colEntity.currentSpeed != colEntity.speed)
                    return;

                if (colEntity.playerID != playerID)
                    colEntity.DOKillEntity();
                else
                {
                    if (typeItem != eTypeItem.grab)
                    {
                        entity.currentSpeed = colEntity.currentSpeed;
                        if (!colEntity.isWalk)
                            entity.isWalk = false;
                    }
                    else if (entity.currentSpeed == entity.speed)
                    {
                        hits = Physics.RaycastAll(transform.position, transform.forward, Mathf.Infinity, 1 << 8);
                        hits = hits.OrderBy(x => Vector2.Distance(this.transform.position, x.transform.position * ((entity.playerID == 0) ? 1 : -1))).ToArray();

                        for (int i = 0; i < hits.Length; ++i)
                        {
                            if (hits[i].transform.GetComponentInParent<Entity>().playerID != playerID)
                            {
                                if (hits[i].transform.GetComponentInParent<Entity>().itemForwardParent
                                    .GetChild(hits[i].transform.GetComponentInParent<Entity>().itemForwardParent.childCount - 1)
                                    .GetComponent<Item>().typeItem != eTypeItem.defence)
                                {
                                    entity.currentSpeed = colEntity.currentSpeed;
                                    
                                    entity.isWalk = false;

                                    goToEnemyDefence = true;
                                    
                                    return;
                                }
                            }
                        }

                        entity.currentSpeed = entity.speed / 2;
                    }
                }
                return;
            }

            colItem = col.GetComponent<Item>();

            if (colItem != null)
            {
                if (colItem.playerID == playerID)
                    return;

                // I am an ATTACK
                if (typeItem == eTypeItem.attack)
                {
                    if (colItem.typeItem == eTypeItem.attack)
                    {
                        if (attackForce == colItem.attackForce)
                        {
                            colItem.DODestroy();
                            DODestroy();
                        }
                        else if (attackForce > colItem.attackForce)
                        {
                            colItem.DODestroy();
                        }
                        else if (attackForce < colItem.attackForce)
                        {
                            DODestroy();
                        }
                    }
                    else if (colItem.typeItem == eTypeItem.defence)
                    {
                        colItem.entity.isWalk = false;
                        entity.isWalk = false;
                    }
                    else if (colItem.typeItem == eTypeItem.grab)
                    {
                        colItem.DODestroy();
                    }
                }

                // I am a DEFENCE
                else if (typeItem == eTypeItem.defence)
                {
                    if (colItem.typeItem == eTypeItem.attack)
                    {
                        colItem.entity.isWalk = false;
                        entity.isWalk = false;
                    }
                    else if (colItem.typeItem == eTypeItem.defence)
                    {
                        colItem.entity.isWalk = false;
                        entity.isWalk = false;
                        goToEnemyDefence = false;
                    }
                    else if (colItem.typeItem == eTypeItem.grab)
                    {
                        DODestroy();
                    }
                }

                // I am a GRAB
                else if (typeItem == eTypeItem.grab)
                {
                    if (colItem.typeItem == eTypeItem.attack)
                    {
                        DODestroy();
                    }
                    else if (colItem.typeItem == eTypeItem.defence)
                    {
                        colItem.DODestroy();
                    }
                    else if (colItem.typeItem == eTypeItem.grab)
                    {
                        colItem.DODestroy();
                        DODestroy();
                    }
                }
            }
        }

        private bool onDestoy;
        public void DODestroy()
        {
            if (onDestoy) return;

            onDestoy = true;

            if (positionID == 0)
            {
                entity.DOKillEntity();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}