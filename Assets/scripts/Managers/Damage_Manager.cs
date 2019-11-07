using System;
using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class Damage_Manager : MonoBehaviour
    {
        public Base_Character_Manager baseManager;
        public DamageModel charDamageModel;
        public DamageModel currentTargetDmgModel;
        public GameObject hitFX;
        public Battle_Details_Manager battleDetailsManager;
        //public Character_Manager characterManager {get; set;}
        void Start(){
            baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
            battleDetailsManager = Battle_Manager.battleDetailsManager;
            if( baseManager.animationManager.skeletonAnimation ){
                baseManager.animationManager.skeletonAnimation.state.Event += OnEventHit;
            }
        }

        public void TakeDmg(DamageModel damageModel, string eventName)
        {
            GameObject skillHitEffect = damageModel.customHitFX != null ? damageModel.customHitFX : hitFX;
            Battle_Manager.gameEffectManager.ScreenShake(1f);
            if (gameObject.tag == "Player" && eventName == "hit")
            {
                damageModel.effectObject = (GameObject)Instantiate(skillHitEffect, new Vector2(damageModel.hitEffectPositionScript.transform.position.x, 
                    damageModel.hitEffectPositionScript.transform.position.y), new Quaternion(0, 180, 0, 0));
            }
            else if (eventName == "hit")
            {
                damageModel.effectObject = (GameObject)Instantiate(skillHitEffect, new Vector2(damageModel.hitEffectPositionScript.transform.position.x, 
                    damageModel.hitEffectPositionScript.transform.position.y), damageModel.hitEffectPositionScript.transform.rotation);
            }

            if (baseManager.characterManager.characterModel.absorbPoints > 0)
            {
                var absorbedDmg = baseManager.characterManager.characterModel.absorbPoints -= damageModel.damageTaken;
                baseManager.characterManager.characterModel.absorbPoints -= damageModel.damageTaken;
                if (absorbedDmg < 0)
                {
                    absorbedDmg -= (absorbedDmg *= 2);
                    baseManager.characterManager.characterModel.Health = baseManager.characterManager.characterModel.Health - absorbedDmg;
                     battleDetailsManager.getDmg(damageModel);
                }
                damageModel.absorbAmount = baseManager.characterManager.characterModel.absorbPoints - damageModel.damageTaken;
                Battle_Manager.battleDetailsManager.ShowAbsorbNumber(damageModel);
            }
            else if (baseManager.characterManager.characterModel.blockPoints > 0)
            {
                baseManager.characterManager.characterModel.blockPoints -= 1f;
                var damageBlocked = damageModel.damageTaken * 0.25f;
                damageModel.damageTaken -= damageBlocked;
                baseManager.characterManager.characterModel.Health = baseManager.characterManager.characterModel.Health - damageModel.damageTaken;
                battleDetailsManager.getDmg(damageModel, extraInfo: "<size=100><i>(block:" + damageBlocked + ")</i></size>");
            }
            else
            {
                baseManager.characterManager.characterModel.Health = baseManager.characterManager.characterModel.Health - damageModel.damageTaken;
                battleDetailsManager.getDmg(damageModel);
            }

            if ( baseManager.animationManager.inAnimation == false && baseManager.autoAttackManager.isAttacking == false)
            {
                if (!string.IsNullOrEmpty(damageModel.hitAnimation))
                {
                    baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, damageModel.hitAnimation, false);
                    baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, damageModel.holdAnimation, true, 0);
                    baseManager.animationManager.inAnimation = true;
                }
                else
                {
                    baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, damageModel.hitAnimNormal, false);
                    baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, baseManager.animationManager.idleAnim, true, 0);
                }
            }
            //if tumor on player
            if (baseManager.statusManager.DoesStatusExist("tumor") && damageModel.dmgSource != null)
            {
                var tumor = baseManager.statusManager.GetStatusIfExist("tumor");
                tumor.buffPower += damageModel.damageTaken * 0.70f;
            }
            //if thorns on player
            if (baseManager.statusManager.DoesStatusExist("thorns") && damageModel.dmgSource != null)
            {
                var sourceCalDmg = damageModel.dmgSource.GetComponent<Damage_Manager>();
                var dmgModel = new DamageModel(baseManager){
                            incomingDmg = baseManager.characterManager.characterModel.thornsDmg
                        };
                sourceCalDmg.calculatedamage(damageModel);
            }

            //if On hit
            if (baseManager.statusManager.DoesStatusExist("onHit"))
            {
                var onHitSkill = baseManager.statusManager.GetStatusIfExist("onHit");
                if (baseManager.characterManager.characterModel.characterType == Character_Model.CharacterTypeEnum.enemy)
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
            Battle_Manager.eventManager.BuildEvent(eventModel);
        }

        public void OnEventHit(Spine.TrackEntry state, Spine.Event e)
        {
            if (e.Data.name == "hit" || e.Data.name == "SFXhit")
            {
                foreach (var target in currentTargetDmgModel.dueDmgTargets)
                {
                    var targetDamageManager = target.GetComponent<Base_Character_Manager>().damageManager;
                    //var dM = new DamageModel();
                    targetDamageManager.TakeDmg(currentTargetDmgModel, e.Data.name);
                    var eventModel = new EventModel{
                        eventName = "OnDealingDmg",
                        extTarget = target,
                        eventCaller = baseManager.characterManager,
                        extraInfo = targetDamageManager.currentTargetDmgModel.damageTaken
                    };
                    Battle_Manager.eventManager.BuildEvent(eventModel);
                }
            }
        }

        public void calculatedamage(DamageModel damageModel)
        {
            var skillSource = "";
            if (damageModel.skillModel != null)
            {
                damageModel.skillSource = damageModel.skillModel != null ? damageModel.skillModel.skillName : damageModel.skillSource;
            }

            if (damageModel.baseManager != null)
            {
                var defences = damageModel.isMagicDmg ? damageModel.baseManager.characterManager.characterModel.MDef : 
                    damageModel.baseManager.characterManager.characterModel.PDef;
                var damageTaken = (damageModel.incomingDmg - defences) < 0 ? 0 : 
                    damageModel.incomingDmg - defences;
                damageModel.damageTaken = damageModel.trueDmg ? damageModel.incomingDmg : damageTaken;
                damageModel.dmgSource = baseManager.characterManager;
                if (baseManager.statusManager.DoesStatusExist("damageImmune"))
                {
                    battleDetailsManager.Immune(damageModel);
                }
                else
                {
                    if (baseManager.animationManager.skeletonAnimation == null)
                    {
                        baseManager.characterManager.characterModel.Health = baseManager.characterManager.characterModel.Health - damageTaken;

                        //if tumor on player
                        if (baseManager.statusManager.DoesStatusExist("tumor"))
                        {
                            var tumor = baseManager.statusManager.GetStatusIfExist("tumor");
                            tumor.buffPower += damageTaken * 0.70f;
                        }

                        battleDetailsManager.getDmg(damageModel);
                    }
                    else
                    {
                        if (damageModel.skillModel != null)
                        {
                            damageModel.customHitFX = damageModel.skillModel.hitEffect;
                        }
                    }
                }
            }
        }

        public void calculateFlatDmg(DamageModel damageModel)
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

        public void calculateMdamge(DamageModel damageModel)
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

        public void calculateHdamage(DamageModel damageModel)
        {
            if (damageModel.incomingHeal > 0)
            {
                baseManager.characterManager.characterModel.Health += damageModel.incomingHeal;
                battleDetailsManager.getHeal(damageModel);
            }
        }
    }
}



