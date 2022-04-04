using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class BaseDamageManager : MonoBehaviour
    {
        public BaseCharacterManagerGroup baseManager;
        public Dictionary<string, BaseDamageModel> skillDmgModels = new Dictionary<string, BaseDamageModel>();
        public Dictionary<string, BaseDamageModel> autoAttackDmgModels = new Dictionary<string, BaseDamageModel>();
        public GameObject hitFX;
        public BattleDetailsManager battleDetailsManager;

        /*void Start(){
            baseManager = this.gameObject.GetComponent<Base_Character_Manager_Group>();
            battleDetailsManager = Battle_Manager.battleDetailsManager;
        }*/

        public void TakeDmg<T>(T damageModel, string eventName) where T : BaseDamageModel
        {
            GameObject skillHitEffect = damageModel.customHitFX != null ? damageModel.customHitFX : hitFX;
            BattleManager.gameEffectManager.ScreenShake(0.6f, 2);
            if (damageModel.hitEffectPositionScript != null && gameObject.tag == "Player" && eventName == "hit")
            {
                damageModel.effectObject = Instantiate(skillHitEffect, new Vector2(damageModel.hitEffectPositionScript.transform.position.x, 
                    damageModel.hitEffectPositionScript.transform.position.y), new Quaternion(0, 180, 0, 0));
            }
            else if (damageModel.hitEffectPositionScript != null && eventName == "hit")
            {
                damageModel.effectObject = Instantiate(skillHitEffect, new Vector2(damageModel.hitEffectPositionScript.transform.position.x, 
                    damageModel.hitEffectPositionScript.transform.position.y), damageModel.hitEffectPositionScript.transform.rotation);
            }

            if (baseManager.characterManager.characterModel.absorbPoints > 0)
            {
                var initialAbsorbAmount = baseManager.characterManager.characterModel.absorbPoints;
                var remainingAbsorbed = baseManager.characterManager.characterModel.absorbPoints - damageModel.damageTaken;
                damageModel.damageAbsorbed = baseManager.characterManager.characterModel.absorbPoints > damageModel.damageTaken ?
                    remainingAbsorbed : baseManager.characterManager.characterModel.absorbPoints;
                if (remainingAbsorbed < 0)
                {
                    damageModel.damageTaken = Math.Abs(remainingAbsorbed);
                    baseManager.characterManager.characterModel.Health -= damageModel.damageTaken;
                    battleDetailsManager.getDmg(damageModel);
                } else
                {
                    damageModel.damageTaken = 0f;
                }
                baseManager.characterManager.characterModel.absorbPoints = remainingAbsorbed > 0f ? remainingAbsorbed : 0f;
                BattleManager.battleDetailsManager.ShowAbsorbNumber(damageModel, initialAbsorbAmount);
            }
            else if (baseManager.characterManager.characterModel.blockPoints > 0)
            {
                baseManager.characterManager.characterModel.blockPoints -= 1f;
                var damageBlocked = damageModel.damageTaken * 0.25f;
                damageModel.damageTaken -= damageBlocked;
                baseManager.characterManager.characterModel.Health = baseManager.characterManager.characterModel.Health - damageModel.damageTaken;
                battleDetailsManager.getDmg(damageModel, extraInfo: "<size=100><i>(block:" + damageBlocked + ")</i></size>");
                BattleManager.soundManager.playSound("block");
            }
            else
            {
                baseManager.characterManager.characterModel.Health = baseManager.characterManager.characterModel.Health - damageModel.damageTaken;
                battleDetailsManager.getDmg(damageModel);
            }

            if ( !baseManager.animationManager.inAnimation && !baseManager.autoAttackManager.isAttacking )
            {
                baseManager.animationManager.PlaySetAnimation(baseManager.animationManager.hitAnimation.ToString(), false);
                baseManager.animationManager.PlayAddAnimation(baseManager.animationManager.idleAnimation.ToString(), true, 0);
                //baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, baseManager.animationManager.hitAnimation.ToString(), false);
                //baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, baseManager.animationManager.idleAnimation.ToString(), true, 0);
            }
            //if tumor on player
            if (baseManager.statusManager.DoesStatusExist("tumor") && damageModel.dmgSource != null)
            {
                var tumor = baseManager.statusManager.GetStatusIfExist("tumor");
                tumor.buffPower += damageModel.damageTaken * 0.60f;
            }
            //if thorns on player
            if (baseManager.statusManager.DoesStatusExist("thorns") && damageModel.dmgSource != null)
            {
                var sourceCalDmg = damageModel.dmgSource.baseManager.damageManager;
                var thornsDmgModel = new BaseDamageModel(){
                            baseManager = damageModel.dmgSource.baseManager,
                            incomingDmg = baseManager.characterManager.characterModel.thornsDmg,
                            damageImmidiately = true,
                            element = elementType.trueDmg
                        };
                sourceCalDmg.calculatedamage(thornsDmgModel);
            }

            //if On hit
            if (baseManager.statusManager.DoesStatusExist("onHit"))
            {
                var onHitSkill = baseManager.statusManager.GetStatusIfExist("onHit");
                if (baseManager.characterManager.characterModel.characterType == CharacterModel.CharacterTypeEnum.enemy)
                {
                    //enemySkillScript.PrepSkill( characterScript.role, 0, onHitSkill.onHitSkillEnemy );
                }
                else
                {
                    //this.GetComponent<enemySkillSelection>().PrepSkill( characterScript.role, 0, onHitSkill.onHitSkillPlayer );
                }
            }
            var eventModel = new EventModel{
                eventName = "OnTakingDmg",
                extTarget = damageModel.dmgSource,
                eventCaller = baseManager.characterManager
            };
            BattleManager.eventManager.BuildEvent(eventModel);
        }

        public float GetValueAfterResist(float incomingDmg, float resistance)
        {
            return incomingDmg - (incomingDmg * resistance);
        }

        public void calculatedamage(BaseDamageModel damageModel)
        {
            if (damageModel.skillModel != null)
            {
                damageModel.skillSource = damageModel.skillModel != null ? damageModel.skillModel.skillName : damageModel.skillSource;
            } else if (damageModel.enemySkillModel != null)
            {
                damageModel.skillSource = damageModel.enemySkillModel != null ? damageModel.enemySkillModel.skillName : damageModel.skillSource;
            }

            if (baseManager != null)
            {
                var defences = damageModel.isMagicDmg ? baseManager.characterManager.characterModel.MDef : baseManager.characterManager.characterModel.PDef;
                var resistance = damageModel.element != elementType.none ? baseManager.characterManager.GetResistanceValue(damageModel.element.ToString()) : 0;
                var damageTaken = (damageModel.incomingDmg - defences) < 0 ? 0 : damageModel.incomingDmg - defences;
                damageTaken = BattleManager.gameManager.GetChance((int)baseManager.characterManager.characterModel.accuracy) ? damageTaken + baseManager.characterManager.characterModel.piercing : damageTaken;
                var elementDamageTaken = GetValueAfterResist(damageModel.incomingDmg, resistance) < 0 ? 0 : GetValueAfterResist(damageModel.incomingDmg, resistance);
                damageModel.damageTaken = damageModel.element != elementType.none ? elementDamageTaken : damageTaken;
                if (baseManager.statusManager.DoesStatusExist("damageImmune"))
                {
                    battleDetailsManager.Immune(damageModel);
                }
                else
                {
                    if (baseManager.animationManager.skeletonAnimation == null || damageModel.damageImmidiately)
                    {
                        baseManager.characterManager.characterModel.Health = baseManager.characterManager.characterModel.Health - damageModel.damageTaken;

                        //if tumor on player
                        if (baseManager.statusManager.DoesStatusExist("tumor"))
                        {
                            var tumor = baseManager.statusManager.GetStatusIfExist("tumor");
                            tumor.buffPower += damageModel.damageTaken * 0.70f;
                        }

                        battleDetailsManager.getDmg(damageModel, damageModel.showExtraInfo ? damageModel.skillSource : "");
                    }
                    else
                    {
                        if (damageModel.skillModel != null)
                        {
                            damageModel.customHitFX = damageModel.skillModel.hitEffect;
                        }
                        if (damageModel.enemySkillModel != null)
                        {
                            damageModel.customHitFX = damageModel.enemySkillModel.hitEffect;
                        }
                    }
                }
            }
        }

        public void calculateFlatDmg(BaseDamageModel damageModel)
        {
            if (damageModel.flatDmgTaken > 0)
            {
                if (baseManager.characterManager.characterModel.absorbPoints > 0)
                {
                    baseManager.characterManager.characterModel.absorbPoints -= damageModel.flatDmgTaken;
                }
                else
                {
                    baseManager.characterManager.characterModel.Health = baseManager.characterManager.characterModel.Health - damageModel.flatDmgTaken;
                    battleDetailsManager.getDmg(damageModel);
                }
                baseManager.characterManager.characterModel.incomingMDmg = 0;
            }
            else if (damageModel.flatDmgTaken <= 0)
            {
                baseManager.characterManager.characterModel.incomingMDmg = 0;
            }
        }

        public void calculateMdamge(BaseDamageModel damageModel)
        {
            damageModel.MdamageTaken = baseManager.characterManager.characterModel.incomingMDmg - baseManager.characterManager.characterModel.MDef;
            if (damageModel.MdamageTaken > 0)
            {
                if (baseManager.characterManager.characterModel.absorbPoints > 0)
                {
                    baseManager.characterManager.characterModel.absorbPoints -= damageModel.MdamageTaken;
                }
                else
                {
                    baseManager.characterManager.characterModel.Health = baseManager.characterManager.characterModel.Health - damageModel.MdamageTaken;
                    battleDetailsManager.getDmg(damageModel);
                }
                baseManager.characterManager.characterModel.incomingMDmg = 0;
                damageModel.MdamageTaken = 0;
            }
            else if (damageModel.MdamageTaken <= 0)
            {
                damageModel.MdamageTaken = 0;
                baseManager.characterManager.characterModel.incomingMDmg = 0;
            }
        }

        public void calculateHdamage(BaseDamageModel damageModel)
        {
            if (damageModel.incomingHeal > 0)
            {
                baseManager.characterManager.characterModel.Health += damageModel.incomingHeal;
                battleDetailsManager.getHeal(damageModel);
            }
        }

       /* public void SetHitIdleAnimation(string hitAnimation, string hitIdleAnimation)
        {
            this.hitAnimation = !string.IsNullOrEmpty(hitAnimation) ? hitAnimation : "hit";
            this.hitIdleAnimation = !string.IsNullOrEmpty(hitIdleAnimation) ? hitIdleAnimation : "idle";
        }*/
    }
}



