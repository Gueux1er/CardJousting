using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LibLabGames.NewGame
{
    public class Item : MonoBehaviour
    {
        public enum eTypeItem { attack, defence, grab }
        public eTypeItem typeItem;
        public int attackForce;

        private Entity entity;

        private int playerID;
        private int wayID;
        private int positionID;

        public bool onTheSide;

        public Collider forwardCollider;
        public Collider sideCollider;

        private void Start()
        {
            entity = GetComponentInParent<Entity>();
            playerID = entity.playerID;
            wayID = transform.parent.GetSiblingIndex();
            positionID = transform.GetSiblingIndex();

            onTheSide = transform.parent != entity.itemForwardParent;

            forwardCollider.enabled = !onTheSide;
            sideCollider.enabled = onTheSide;
        }

        private Entity colEntity;
        private Item colItem;
        private RaycastHit[] hits;
        private void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag(string.Format("GoalPlayer{0}", playerID)))
            {
                entity.HurtPlayer();
            }

            if (onTheSide)
            {
                OnTriggerSide(col);
            }
            else
            {
                OnTriggerNoSide(col);
            }
        }

        private void OnTriggerSide(Collider col)
        {
            if (col.CompareTag("Entity"))
            {
                colEntity = col.GetComponentInParent<Entity>();

                if (colEntity == entity)
                    return;

                if (colEntity.playerID != playerID)
                    colEntity.DOKillEntity();
                else
                {
                    if (typeItem != eTypeItem.grab)
                    {
                        entity.currentSpeed = colEntity.currentSpeed;

                        if (!colEntity.isWalk)
                        {
                            entity.isWalk = false;
                        }
                    }
                    else if (!entity.isOvertake)
                    {
                        Entity ent = entity.nextEntity;

                        while (ent.nextEntity != null)
                            ent = ent.nextEntity;

                        if (ent.enemyEntity == null ||
                            ent.enemyEntity.itemForwardParent.GetComponentInChildren<Item>().typeItem != eTypeItem.defence)
                        {
                            entity.currentSpeed = colEntity.currentSpeed;

                            entity.isWalk = false;
                        }
                        else
                        {
                            entity.nextEntity.behindEntity = entity.behindEntity;
                            entity.nextEntity = null;
                            entity.behindEntity = ent;
                            entity.behindEntity.nextEntity = entity;

                            entity.isOvertake = true;
                        }
                    }
                }

                entity.CheckCanWalk();

                return;
            }

            colItem = col.GetComponentInParent<Item>();

            if (colItem != null)
            {
                if (colItem.playerID == playerID || !colItem.onTheSide)
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
                        entity.enemyEntity = colItem.entity;
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
                        entity.enemyEntity = colItem.entity;
                    }
                    else if (colItem.typeItem == eTypeItem.defence)
                    {
                        colItem.entity.isWalk = false;
                        entity.isWalk = false;
                        //goToEnemyDefence = false;
                        entity.enemyEntity = colItem.entity;
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
                        entity.isOvertake = false;
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

        private void OnTriggerNoSide(Collider col)
        {
            if (col.CompareTag("Entity"))
            {
                colEntity = col.GetComponentInParent<Entity>();

                if (colEntity == entity)
                    return;

                if (colEntity.playerID != playerID)
                    colEntity.DOKillEntity();
                else
                {
                    if (typeItem != eTypeItem.grab)
                    {
                        entity.currentSpeed = colEntity.currentSpeed;

                        if (!colEntity.isWalk)
                        {
                            entity.isWalk = false;
                        }
                    }
                    else if (!entity.isOvertake)
                    {
                        Entity ent = entity.nextEntity;

                        while (ent.nextEntity != null)
                            ent = ent.nextEntity;

                        if (ent.enemyEntity == null ||
                            ent.enemyEntity.itemForwardParent.GetComponentInChildren<Item>().typeItem != eTypeItem.defence)
                        {
                            entity.currentSpeed = colEntity.currentSpeed;

                            entity.isWalk = false;
                        }
                        else
                        {
                            entity.nextEntity.behindEntity = entity.behindEntity;
                            entity.nextEntity = null;
                            entity.behindEntity = ent;
                            entity.behindEntity.nextEntity = entity;

                            entity.isOvertake = true;
                        }
                    }
                }

                entity.CheckCanWalk();

                return;
            }

            colItem = col.GetComponentInParent<Item>();

            if (colItem != null)
            {
                if (colItem.playerID == playerID || colItem.onTheSide)
                {
                    return;
                }

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
                        entity.enemyEntity = colItem.entity;
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
                        entity.enemyEntity = colItem.entity;
                    }
                    else if (colItem.typeItem == eTypeItem.defence)
                    {
                        colItem.entity.isWalk = false;
                        entity.isWalk = false;
                        entity.enemyEntity = colItem.entity;
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
                        entity.isOvertake = false;
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

            entity.CheckCanWalk();

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