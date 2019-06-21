using System;
using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class Damage_Manager : Base_Character_Manager
    {
        public DamageModel charDamageModel { get; set; }
        //public Status_Manager statusManager { get; set; }
        public Battle_Details_Manager battleDetailsManager { get; set; }
        //public Character_Manager characterManager {get; set;}
        public Damage_Manager(){
            //characterManager = this.gameObject.GetComponent<Character_Manager>();
            // 'statusManager = this.gameObject.GetComponent<Status_Manager>();
            charDamageModel = new DamageModel();
            battleDetailsManager = Battle_Manager.battleDetailsManager;
        }

        public void TakeDmg(DamageModel damageModel, string eventName)
        {
            GameObject skillHitEffect = damageModel.customHitFX != null ? damageModel.customHitFX : damageModel.hitEffect;
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

            if (characterManager.characterModel.absorbPoints > 0)
            {
                var absorbedDmg = characterManager.characterModel.absorbPoints -= damageModel.damageTaken;
                characterManager.characterModel.absorbPoints -= damageModel.damageTaken;
                if (absorbedDmg < 0)
                {
                    absorbedDmg -= (absorbedDmg *= 2);
                    characterManager.characterModel.Health = characterManager.characterModel.Health - absorbedDmg;
                     battleDetailsManager.getDmg(damageModel);
                }
                var absorbAmount = characterManager.characterModel.absorbPoints - damageModel.damageTaken;
                Battle_Manager.battleDetailsManager.ShowAbsorbNumber(damageModel);
            }
            else if (characterManager.characterModel.blockPoints > 0)
            {
                characterManager.characterModel.blockPoints -= 1f;
                var damageBlocked = damageModel.damageTaken * 0.25f;
                damageModel.damageTaken -= damageBlocked;
                characterManager.characterModel.Health = characterManager.characterModel.Health - damageModel.damageTaken;
                battleDetailsManager.getDmg(damageModel, extraInfo: "<size=100><i>(block:" + damageBlocked + ")</i></size>");
            }
            else
            {
                characterManager.characterModel.Health = characterManager.characterModel.Health - damageModel.damageTaken;
                battleDetailsManager.getDmg(damageModel);
            }

            if ( characterManager.animationManager.inAnimation == false && characterManager.characterModel.isAttacking == false)
            {
                if (damageModel.hitAnimation != "")
                {
                    characterManager.animationManager.skeletonAnimation.state.SetAnimation(0, damageModel.hitAnimation, false);
                    characterManager.animationManager.skeletonAnimation.state.AddAnimation(0, damageModel.holdAnimation, true, 0);
                    characterManager.animationManager.inAnimation = true;
                }
                else
                {
                    var hitAnim = gameObject.tag == "Enemy" ? "hit" : damageModel.hitAnimNormal;
                    var idleAnim = gameObject.tag == "Enemy" ? "idle" : characterManager.animationManager.idleAnim;
                    characterManager.animationManager.skeletonAnimation.state.SetAnimation(0, hitAnim, false);
                    characterManager.animationManager.skeletonAnimation.state.AddAnimation(0, idleAnim, true, 0);
                }
            }
            //if tumor on player
            if (characterManager.statusManager.DoesStatusExist("tumor") && damageModel.dmgSource != null)
            {
                var tumor = characterManager.statusManager.GetStatusIfExist("tumor");
                tumor.buffPower += damageModel.damageTaken * 0.70f;
            }
            //if thorns on player
            if (characterManager.statusManager.DoesStatusExist("thorns") && damageModel.dmgSource != null)
            {
                var sourceCalDmg = damageModel.dmgSource.GetComponent<Damage_Manager>();
                var dmgModel = new DamageModel{
                            incomingDmg = characterManager.characterModel.thornsDmg
                        };
                sourceCalDmg.calculatedamage(damageModel);
            }

            //if On hit
            if (characterManager.statusManager.DoesStatusExist("onHit"))
            {
                var onHitSkill = characterManager.statusManager.GetStatusIfExist("onHit");
                if (characterManager.characterModel.characterType == "enemy")
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
                eventCaller = this.gameObject
            };
            EventManager.BuildEvent(eventModel);
        }

        public void OnEventHit(Spine.TrackEntry state, Spine.Event e)
        {
            if (e.Data.name == "hit" || e.Data.name == "SFXhit")
            {
                foreach (var target in charDamageModel.dueDmgTargets)
                {
                    var targetDamageManager = target.GetComponent<Damage_Manager>();
                    targetDamageManager.TakeDmg(e.Data.name);
                    var eventModel = new EventModel{
                        eventName = "OnDealingDmg",
                        extTarget = target,
                        eventCaller = this.gameObject,
                        extraInfo = targetDamageManager.charDamageModel.damageTaken
                    };
                    EventManager.BuildEvent(eventModel);
                }
            }
        }

        public void calculatedamage(DamageModel damageModel)
        {
            var skillSource = "";
            var isSpell = damageModel.classSkill == null ? damageModel.enemySkill.isSpell : damageModel.classSkill.isSpell;
            if (damageModel.classSkill != null)
            {
                skillSource = damageModel.classSkill != null ? damageModel.classSkill.skillName : damageModel.skillSource;
            }
            else if (damageModel.enemySkill != null)
            {
                skillSource = damageModel.enemySkill != null ? damageModel.enemySkill.skillName : damageModel.skillSource;
            }

            if (damageModel.characterManager != null)
            {
                var defences = isSpell ? damageModel.characterManager.characterModel.MDef : 
                    damageModel.characterManager.characterModel.PDef;
                var damageTaken = (damageModel.incomingDmg - defences) < 0 ? 0 : 
                    damageModel.incomingDmg - defences;
                damageTaken = damageModel.trueDmg ? damageModel.incomingDmg : damageModel.damageTaken;
                damageModel.dmgSource = this.gameObject;
                if (statusManager.DoesStatusExist("damageImmune"))
                {
                    battleDetailsManager.Immune(skillSource);
                }
                else
                {
                    if (characterManager.animationManager.skeletonAnimation == null)
                    {
                        characterManager.characterModel.Health = characterManager.characterModel.Health - damageTaken;

                        //if tumor on player
                        if (statusManager.DoesStatusExist("tumor"))
                        {
                            var tumor = statusManager.GetStatusIfExist("tumor");
                            tumor.buffPower += damageTaken * 0.70f;
                        }

                        battleDetailsManager.getDmg(damageTaken, skillSource);
                    }
                    else
                    {
                        if (damageModel.classSkill != null)
                        {
                            damageModel.customHitFX = damageModel.classSkill.hitEffect;
                        }
                        else if (enemySkill != null)
                        {
                            damageModel.customHitFX = damageModel.enemySkill.hitEffect;
                        }
                    }
                }
            }
        }

        public void calculateFlatDmg(DamageModel damageModel)
        {
            if (damageModel.flatDmgTaken > 0)
            {
                if (characterManager.characterModel.absorbPoints > 0)
                {
                    characterManager.characterModel.absorbPoints -= damageModel.flatDmgTaken;
                }
                else
                {
                    characterManager.characterModel.Health = characterManager.characterModel.Health - damageModel.flatDmgTaken;
                    battleDetailsManager.getDmg(damageModel.flatDmgTaken, damageModel.skillSource);
                }
                characterManager.characterModel.incomingMDmg = 0;
            }
            else if (damageModel.flatDmgTaken <= 0)
            {
                characterManager.characterModel.incomingMDmg = 0;
            }
        }

        public void calculateMdamge(DamageModel damageModel)
        {
            damageModel.MdamageTaken = characterManager.characterModel.incomingMDmg - characterManager.characterModel.MDef;
            if (damageModel.MdamageTaken > 0)
            {
                if (characterManager.characterModel.absorbPoints > 0)
                {
                    characterManager.characterModel.absorbPoints -= damageModel.MdamageTaken;
                }
                else
                {
                    characterManager.characterModel.Health = characterManager.characterModel.Health - damageModel.MdamageTaken;
                    battleDetailsManager.getDmg(damageModel.MdamageTaken, damageModel.skillSource);
                }
                characterManager.characterModel.incomingMDmg = 0;
                damageModel.MdamageTaken = 0;
            }
            else if (damageModel.MdamageTaken <= 0)
            {
                damageModel.MdamageTaken = 0;
                characterManager.characterModel.incomingMDmg = 0;
            }
        }

        public void calculateHdamage(DamageModel damageModel)
        {
            if (damageModel.incomingHeal > 0)
            {
                characterManager.characterModel.Health += damageModel.incomingHeal;
                battleDetailsManager.getHeal(damageModel.incomingHeal);
            }
        }
    }
}



