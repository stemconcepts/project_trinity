using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.scripts.Models.statusModels;

namespace AssemblyCSharp
{
    public class BaseDamageManager : MonoBehaviour
    {
        public List<AudioClip> damagedSounds;
        public BaseCharacterManagerGroup baseManager;
        public Dictionary<string, BaseDamageModel> skillDmgModels = new Dictionary<string, BaseDamageModel>();
        public Dictionary<string, BaseDamageModel> autoAttackDmgModels = new Dictionary<string, BaseDamageModel>();
        public GameObject hitFX;
        public BattleDetailsManager battleDetailsManager;

        public void TakeDmg<T>(T damageModel, string eventName) where T : BaseDamageModel
        {
            BattleManager.gameEffectManager.ScreenShake(0.6f, 2);
            if (eventName == "hit" && hitFX)
            {
                var skillEffect = new SkillEffect(1, hitFX, fxPosEnum.center);
                MainGameManager.instance.gameEffectManager.CallEffectOnTarget(baseManager.effectsManager.fxCenter, skillEffect);
            }

            if (baseManager.characterManager.characterModel.absorbPoints > 0)
            {
                var initialAbsorbAmount = baseManager.characterManager.characterModel.absorbPoints;
                var remainingAbsorbed = baseManager.characterManager.characterModel.absorbPoints - damageModel.damageTaken;
                damageModel.damageAbsorbed = baseManager.characterManager.characterModel.absorbPoints > damageModel.damageTaken ?
                    remainingAbsorbed : baseManager.characterManager.characterModel.absorbPoints;
                if (remainingAbsorbed < 0)
                {
                    damageModel.damageTaken = (float)Math.Abs(Math.Ceiling(remainingAbsorbed));
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
                var damageBlocked = damageModel.damageTaken * 0.5f;
                damageModel.damageTaken -= damageBlocked;
                baseManager.characterManager.characterModel.Health = baseManager.characterManager.characterModel.Health - damageModel.damageTaken;
                battleDetailsManager.getDmg(damageModel, extraInfo: "<size=100><i>(block:" + damageBlocked + ")</i></size>");
                MainGameManager.instance.soundManager.playSound("block");
                PlayDamagedSounds();
            }
            else
            {
                baseManager.characterManager.characterModel.Health = baseManager.characterManager.characterModel.Health - damageModel.damageTaken;
                battleDetailsManager.getDmg(damageModel);
                PlayDamagedSounds();
            }

            if ( !baseManager.animationManager.inAnimation && !baseManager.autoAttackManager.isAttacking )
            {
                baseManager.animationManager.PlaySetAnimation(baseManager.animationManager.hitAnimation.ToString(), false);
                baseManager.animationManager.PlayAddAnimation(baseManager.animationManager.idleAnimation.ToString(), true, 0);
            }
            //if tumor on player
            if (baseManager.statusManager.DoesStatusExist(StatusNameEnum.Tumor) && damageModel.dmgSource != null)
            {
                var tumor = baseManager.statusManager.GetStatusIfExist(StatusNameEnum.Tumor);
                tumor.buffPower += damageModel.damageTaken * 0.60f;
            }
            //if thorns on player
            if (baseManager.statusManager.DoesStatusExist(StatusNameEnum.Thorns) && damageModel.dmgSource != null)
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

            baseManager.EventManagerV2.CreateEventOrTriggerEvent(EventTriggerEnum.OnTakingDmg);
        }

        void PlayDamagedSounds()
        {
            if (damagedSounds.Count > 0)
            {
                MainGameManager.instance.soundManager.playSound(damagedSounds);
            }
        }

        public (float damage, float resisted) GetValueAfterResist(float incomingDmg, float resistance)
        {
            var resisted = (float)Math.Abs(Math.Ceiling(resistance * incomingDmg));
            return (damage : (float)Math.Abs(incomingDmg - resisted), resisted : resisted);
        }

        float GetDefenceValue(bool isMagicDmg)
        {
            var defences = isMagicDmg ? baseManager.characterManager.characterModel.MDef : baseManager.characterManager.characterModel.PDef;
            return (defences + BattleManager.DefenceConstant) / BattleManager.DefenceConstant;
        }

        public float GetValueAfterDefence(float incomingDmg, bool isMagicDmg)
        {
            return (float)Math.Abs(
                    Math.Ceiling(incomingDmg / GetDefenceValue(isMagicDmg))
                );
        }

        public void calculatedamage(BaseDamageModel damageModel)
        {
            damageModel.baseManager = damageModel.baseManager ?? this.baseManager;
            if (damageModel.skillModel != null)
            {
                damageModel.skillSource = damageModel.skillModel != null ? damageModel.skillModel.skillName : damageModel.skillSource;
            } else if (damageModel.enemySkillModel != null)
            {
                damageModel.skillSource = damageModel.enemySkillModel != null ? damageModel.enemySkillModel.skillName : damageModel.skillSource;
            }

            if (baseManager != null)
            {
                if (damageModel.element == elementType.trueDmg)
                {
                    damageModel.damageTaken = damageModel.incomingDmg;
                } else
                {
                    var resistance = damageModel.element != elementType.none ? baseManager.characterManager.GetResistanceValue(damageModel.element.ToString()) : 0;
                    var damageTaken = GetValueAfterDefence(damageModel.incomingDmg, damageModel.isMagicDmg);

                    //Remove additional damage based off resistance
                    if (damageModel.element != elementType.none)
                    {
                        var result = GetValueAfterResist(damageTaken, resistance);
                        damageModel.showExtraInfo = true;
                        damageModel.skillSource = $": {result.resisted} Resisted";
                        damageTaken = result.damage;
                    }

                    damageModel.damageTaken = damageTaken;
                }

                if (baseManager.statusManager.DoesStatusExist(StatusNameEnum.DamageImmune))
                {
                    battleDetailsManager.Immune(damageModel);
                }
                else
                {
                    if (baseManager.animationManager.skeletonAnimation == null || damageModel.damageImmidiately)
                    {
                        baseManager.characterManager.characterModel.Health = baseManager.characterManager.characterModel.Health - damageModel.damageTaken;

                        //if tumor on player
                        if (baseManager.statusManager.DoesStatusExist(StatusNameEnum.Tumor))
                        {
                            var tumor = baseManager.statusManager.GetStatusIfExist(StatusNameEnum.Tumor);
                            tumor.buffPower += damageModel.damageTaken * 0.70f;
                        }

                        //Used for immediate effects
                        if (damageModel.damageImmidiately)
                        {
                            battleDetailsManager.getDmg(damageModel, damageModel.showExtraInfo ? damageModel.skillSource : "");
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

                /*var eventModel = new EventModel
                {
                    eventName = EventTriggerEnum.OnHeal.ToString(),
                    extTarget = baseManager.characterManager,
                    eventCaller = baseManager.characterManager
                };
                BattleManager.eventManager.BuildEvent(eventModel);*/
                baseManager.EventManagerV2.CreateEventOrTriggerEvent(EventTriggerEnum.OnHeal);
            }
        }

        public void DoDamage(int damage, BaseCharacterManager target, GenericSkillModel skill = null, bool isMagic = false, bool isFlat = false)
        {
            var dmgModel = new BaseDamageModel()
            {
                baseManager = baseManager,
                incomingDmg = !isFlat ? damage : 0,
                flatDmgTaken = isFlat ? damage : 0,
                enemySkillModel = skill ? (typeof(enemySkill) == skill.GetType() ? (enemySkill)skill : null) : null,
                skillModel = skill ? (typeof(SkillModel) == skill.GetType() ? (SkillModel)skill : null) : null,
                dmgSource = baseManager.characterManager,
                dueDmgTargets = new List<BaseCharacterManager>() { target },
                hitEffectPositionScript = target.baseManager.effectsManager.fxCenter.transform,
                modifiedDamage = skill ? skill.useModifier : false,
                damageImmidiately = true,
                isMagicDmg = isMagic
            };
            if (isFlat)
            {
                calculateFlatDmg(dmgModel);
            } else
            {
                calculatedamage(dmgModel);
            }
        }
    }
}



