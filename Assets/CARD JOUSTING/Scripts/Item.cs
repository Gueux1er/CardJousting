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
        private int positionID;

        public bool onTheSide;

        public GameObject meshObject;

        public Collider forwardCollider;
        public Collider sideCollider;

        private void Start()
        {
            entity = GetComponentInParent<Entity>();
            playerID = entity.playerID;
            positionID = transform.GetSiblingIndex();

            onTheSide = transform.parent != entity.itemForwardParent;

            forwardCollider.enabled = !onTheSide;
            sideCollider.enabled = onTheSide;
            entity.haveItemOnSide = onTheSide;

            meshObject.SetActive(GameManager.instance.DEBUG_displayMeshArm);
        }

        private Entity colEntity;
        private Item colItem;
        private RaycastHit[] hits;
        private void OnTriggerEnter(Collider col)
        {
            if (onDestoy)
                return;

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
            if (col.CompareTag(string.Format("GoalPlayer{0}", playerID)))
            {
                DODestroy();

                return;
            }

            if (col.CompareTag("Entity"))
            {
                colEntity = col.GetComponentInParent<Entity>();

                if (colEntity == entity)
                    return;

                if (colEntity.playerID != playerID && colEntity.wayID != entity.wayID && typeItem != eTypeItem.defence && !colEntity.haveItemOnSide)
                    colEntity.DOKillEntity();
                else if (colEntity.wayID == entity.wayID)
                {
                    if (typeItem != eTypeItem.grab)
                    {
                        entity.currentSpeed = colEntity.currentSpeed;

                        entity.isWalk = false;
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
            if (col.CompareTag(string.Format("GoalPlayer{0}", playerID)))
            {
                int damage = 1;

                if (typeItem == eTypeItem.attack)
                {
                    damage += attackForce + 1;
                }

                entity.HurtPlayer(damage);

                DODestroy();

                return;
            }

            if (col.CompareTag("Entity"))
            {
                colEntity = col.GetComponentInParent<Entity>();

                if (colEntity == entity)
                    return;

                if (colEntity.playerID != playerID && colEntity.wayID == entity.wayID)
                    colEntity.DOKillEntity();
                else if (colEntity.wayID == entity.wayID)
                {
                    if (typeItem != eTypeItem.grab)
                    {
                        entity.currentSpeed = colEntity.currentSpeed;

                        entity.isWalk = false;
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
                            SoundManager.instance.BothDestroyed();

                            colItem.DODestroy();
                            DODestroy();
                        }
                        else if (attackForce > colItem.attackForce)
                        {
                            SoundManager.instance.Kill();

                            colItem.DODestroy();
                        }
                        else if (attackForce < colItem.attackForce)
                        {
                            SoundManager.instance.Kill();

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
                        SoundManager.instance.Kill();

                        colItem.DODestroy();
                    }
                }

                // I am a DEFENCE
                else if (typeItem == eTypeItem.defence)
                {
                    if (colItem.typeItem == eTypeItem.attack)
                    {
                        SoundManager.instance.Blocked();

                        colItem.entity.isWalk = false;
                        entity.isWalk = false;
                        entity.enemyEntity = colItem.entity;
                    }
                    else if (colItem.typeItem == eTypeItem.defence)
                    {
                        SoundManager.instance.Blocked();

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
                        SoundManager.instance.Kill();

                        entity.isOvertake = false;
                        colItem.DODestroy();
                    }
                    else if (colItem.typeItem == eTypeItem.grab)
                    {
                        SoundManager.instance.BothDestroyed();

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