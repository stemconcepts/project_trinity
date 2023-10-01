using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using DG.Tweening.Core.Easing;
using Assets.scripts.Models.skillModels.swapSkills;
using System.Collections.Generic;
using Spine;
using UnityEditor.MPE;

namespace AssemblyCSharp
{
    public class GearSwapManager : MonoBehaviour
    {
        public bool swapReady = true;
        public float gearSwapTime = 10f;
        public GameObject choiceHolder;
        public GameObject corruptionReadyEffect;
        public List<AudioClip> corruptionReadySound;

        public void CheckCurroptionAndRun()
        {
            if (BattleManager.eyeSkill != null && BattleManager.eyeSkill.curroptionRequired <= MainGameManager.instance.exploreManager.GetCurroption())
            {
                ShowChoices();
            } else
            {
                SwapGear();
            }
        }

        public void ShowChoices()
        {
            choiceHolder.SetActive(choiceHolder.activeSelf ? false : true);
        }

        public void CastEyeSkill()
        {
            var curroptionAmount = MainGameManager.instance.exploreManager.GetCurroption();
            var skill = BattleManager.eyeSkill;
            float power;
            if (skill.isFlat)
            {
                power = curroptionAmount;
            }
            else
            {
                power = curroptionAmount * 1.2f;
            }
            DealHealDmg(skill, GetTargets(skill), power);
            var eM = new EventModel()
            {
                eventName = "OnEyeSkillCast"
            };
            BattleManager.eventManager.BuildEvent(eM);
            SwapGear();
            choiceHolder.SetActive(false);
            MainGameManager.instance.exploreManager.ResetCurroption();
            corruptionReadyEffect.SetActive(false);
        }

        private List<BaseCharacterManager> GetTargets(EyeSkill skill)
        {
            var finalTargets = new List<BaseCharacterManager>();
            var enemyPlayers = BattleManager.GetEnemyCharacterManagers();
            var friendlyPlayers = BattleManager.GetFriendlyCharacterManagers();
            if (skill.allEnemy)
            {
                finalTargets.AddRange(enemyPlayers);
            }
            if (skill.allFriendly) { 
                finalTargets.AddRange(friendlyPlayers); 
            }
            return finalTargets;
        }

        private void DealHealDmg(EyeSkill skill, List<BaseCharacterManager> targets, float power)
        {
            foreach (var target in targets)
            {
                SkillData data = new SkillData()
                {
                    target = target,
                };
                var didHit = true; //skill.isSpell ? true : target.GetChanceToBeHit(2);
                skill.RunExtraEffect(data);
                if (skill.doesDamage)
                {
                    var dmgModel = new BaseDamageModel()
                    {
                        baseManager = target.baseManager,
                        incomingDmg = skill.useModifier ? power + (power * skill.modifierAmount) : 
                            (target.characterModel.characterType == BaseCharacterModel.CharacterTypeEnum.Player ? power * 0.8f : power),
                        skill = skill,
                        damageImmidiately = true,
                        dueDmgTargets = targets,
                        hitEffectPositionScript = target.baseManager.effectsManager.fxCenter.transform,
                        modifiedDamage = skill.useModifier
                    };
                    if (didHit)
                    {
                        if (target.baseManager.damageManager.skillDmgModels.ContainsKey(gameObject.name))
                        {
                            target.baseManager.damageManager.skillDmgModels.Remove(gameObject.name);
                        }
                        target.baseManager.damageManager.skillDmgModels.Add(gameObject.name, dmgModel);
                        target.baseManager.damageManager.calculatedamage(dmgModel);
                    }
                    else
                    {
                        dmgModel.incomingDmg = 0;
                        dmgModel.showDmgNumber = false;
                        MainGameManager.instance.soundManager.PlayMissSound();
                        BattleManager.battleDetailsManager.ShowDamageNumber(dmgModel, extraInfo: "Miss");
                    }
                };
                if (skill.healsDamage)
                {
                    var dmgModel = new BaseDamageModel()
                    {
                        baseManager = target.baseManager,
                        incomingHeal = skill.useModifier ? power + (power * skill.modifierAmount) : power,
                        skill = skill,
                        dueDmgTargets = targets,
                        hitEffectPositionScript = target.baseManager.effectsManager.fxCenter.transform,
                        modifiedDamage = skill.useModifier
                    };
                    target.baseManager.damageManager.calculateHdamage(dmgModel);
                };
                if ((didHit || skill.healsDamage) && skill.skillEffects.Count() > 0)
                {
                    skill.skillEffects.ForEach(effect =>
                    {
                        MainGameManager.instance.gameEffectManager.CallEffectOnTarget(target.baseManager.effectsManager.GetGameObjectByFXPos(effect.fxPos), effect);
                    });
                }
                if (didHit || skill.healsDamage)
                {
                    AddStatuses(target, skill);
                }
            }
            if (skill.hitSounds.Count > 0)
            {
                MainGameManager.instance.soundManager.playAllSounds(skill.hitSounds, 0.3f);
            }
            else if (skill.isSpell)
            {
                MainGameManager.instance.soundManager.playAllSounds(MainGameManager.instance.soundManager.hitSounds, 0.3f);
            }
        }

        private void AddStatuses(BaseCharacterManager target, EyeSkill skill)
        {
            if (skill.statusGroupFriendly.Count() > 0)
            {
                ProcessStatus(target, skill, skill.statusGroupFriendly);
            }
            if (skill.statusGroup.Count() > 0)
            {
                ProcessStatus(target, skill, skill.statusGroup);
            }
        }

        private void ProcessStatus(BaseCharacterManager target, GenericSkillModel skill, List<StatusItem> statusItems)
        {
            var hitAnimation = statusItems.Where(statusItem => statusItem.status.hitAnim != animationOptionsEnum.none).Select(o => o.status.hitAnim).FirstOrDefault();
            var hitIdleAnimation = statusItems.Where(statusItem => statusItem.status.holdAnim != animationOptionsEnum.none).Select(o => o.status.holdAnim).FirstOrDefault();
            skill.AttachStatus(statusItems, target.baseManager, skill);
            target.baseManager.animationManager.SetHitIdleAnimation(hitAnimation, hitIdleAnimation);
            MainGameManager.instance.soundManager.PlayNegativeEffectSound();
        }

        public void SwapGear()
        {
            var skillactive = BattleManager.characterSelectManager.friendlyCharacters.Any(x => x.baseManager.skillManager.isSkillactive);
            var isAttacking = BattleManager.characterSelectManager.friendlyCharacters.Any(x => x.baseManager.autoAttackManager.isAttacking);
            if (swapReady && !skillactive && !isAttacking && BattleManager.battleStarted && BattleManager.turn == BattleManager.TurnEnum.PlayerTurn)
            {
                var allRoles = BattleManager.characterSelectManager.friendlyCharacters;
                for (int i = 0; i < allRoles.Count; i++)
                {
                    var currentWSlot = allRoles[i].GetComponent<BaseSkillManager>();
                    var currentWeaponData = allRoles[i].GetComponent<EquipmentManager>();
                    if (((PlayerSkillManager)currentWSlot).weaponSlot == PlayerSkillManager.weaponSlotEnum.Main)
                    {
                        ((PlayerSkillManager)currentWSlot).weaponSlot = PlayerSkillManager.weaponSlotEnum.Alt;
                        currentWeaponData.currentWeaponEnum = EquipmentManager.currentWeapon.Secondary;
                        allRoles[i].characterModel.canAutoAttack = allRoles[i].baseManager.equipmentManager.secondaryWeapon.enablesAutoAttacks;
                    }
                    else
                    {
                        ((PlayerSkillManager)currentWSlot).weaponSlot = PlayerSkillManager.weaponSlotEnum.Main;
                        currentWeaponData.currentWeaponEnum = EquipmentManager.currentWeapon.Primary;
                        allRoles[i].characterModel.canAutoAttack = allRoles[i].baseManager.equipmentManager.primaryWeapon.enablesAutoAttacks;
                    }
                    var charData = allRoles[i].GetComponent<CharacterManager>();
                    BattleManager.battleInterfaceManager.ForEach(o =>
                    {
                        o.SkillSet((PlayerSkillManager)allRoles[i].baseManager.skillManager);
                    });
                    BattleManager.characterSelectManager.GetSelectedClassObject().GetComponent<CharacterInteractionManager>().DisplaySkills();
                }
                CheckGearType();
                swapReady = false;
                GearSwapTimer(gearSwapTime);
                MainGameManager.instance.soundManager.playSound("gearSwapSound");
            }
            else
            {
                print("Gear Swap not Ready");
            }
            choiceHolder.SetActive(false);
            /*if (BattleManager.eyeSkill != null && BattleManager.eyeSkill.curroptionRequired <= MainGameManager.instance.exploreManager.GetCurroption())
            {
                CastEyeSkill(BattleManager.eyeSkill, MainGameManager.instance.exploreManager.GetCurroption());
            }*/
        }

        void GearSwapTimer(float time)
        {
            BattleManager.taskManager.CallTask(time, () =>
            {
                swapReady = true;
            });
            //Battle_Manager.soundManager.playSound("gearSwapReady");
        }

        public void CheckGearType()
        {
            foreach (var playerRole in BattleManager.characterSelectManager.friendlyCharacters)
            {
                var bm = playerRole.GetComponent<BaseCharacterManagerGroup>();
                var currentWeaponData = bm.equipmentManager;
                var playerSkeletonAnim = bm.animationManager;
                var currentWSlot = (PlayerSkillManager)bm.skillManager;
                var weaponType = currentWSlot.weaponSlot == PlayerSkillManager.weaponSlotEnum.Main ? currentWeaponData.primaryWeapon : currentWeaponData.secondaryWeapon;
                if (weaponType.type != weaponModel.weaponType.heavyHanded && weaponType.type != weaponModel.weaponType.cursedGlove && weaponType.type != weaponModel.weaponType.clawAndCannon)
                {
                    if (playerRole.name == "Stalker")
                    {
                        playerSkeletonAnim.skeletonAnimation.skeleton.SetSkin("light");
                    }
                    bm.animationManager.attackAnimation = animationOptionsEnum.attack1;
                    bm.animationManager.idleAnimation = animationOptionsEnum.idle;
                    bm.animationManager.hopAnimation = animationOptionsEnum.hop;
                    bm.animationManager.hitAnimation = animationOptionsEnum.hit;
                }
                else
                {
                    if (playerRole.name == "Stalker")
                    {
                        playerSkeletonAnim.skeletonAnimation.skeleton.SetSkin("heavy");
                    }
                    bm.animationManager.attackAnimation = (animationOptionsEnum)Enum.Parse(typeof(animationOptionsEnum), $"{bm.animationManager.attackAnimation}Heavy");
                    bm.animationManager.idleAnimation = bm.animationManager.idleAnimation == animationOptionsEnum.stunned ? bm.animationManager.idleAnimation : (animationOptionsEnum)Enum.Parse(typeof(animationOptionsEnum), $"{bm.animationManager.idleAnimation}Heavy");
                    bm.animationManager.hopAnimation = (animationOptionsEnum)Enum.Parse(typeof(animationOptionsEnum), $"{bm.animationManager.hopAnimation}Heavy");
                    bm.animationManager.hitAnimation = bm.animationManager.hitAnimation == animationOptionsEnum.toStunned ? animationOptionsEnum.toStunned : (animationOptionsEnum)Enum.Parse(typeof(animationOptionsEnum), $"{bm.animationManager.hitAnimation}Heavy");
                }
                var delay = bm.animationManager.PlaySetAnimation(bm.animationManager.toHeavy.ToString(), false);
                bm.animationManager.PlayAddAnimation(bm.animationManager.idleAnimation.ToString(), true, delay);
            }
        }

        void Start()
        {
            if (BattleManager.eyeSkill != null && BattleManager.eyeSkill.curroptionRequired <= MainGameManager.instance.exploreManager.GetCurroption())
            {
                MainGameManager.instance.taskManager.CallTask(2f, () =>
                    {
                        MainGameManager.instance.soundManager.playAllSounds(corruptionReadySound, 0.3f);
                        corruptionReadyEffect.SetActive(true);
                    }
                );
            }
        }

        void Update()
        {
            /*var skillactive = BattleManager.characterSelectManager.friendlyCharacters.Any(x => x.baseManager.skillManager.isSkillactive);
            var isAttacking = BattleManager.characterSelectManager.friendlyCharacters.Any(x => x.baseManager.autoAttackManager.isAttacking);
            if (!swapReady || skillactive || isAttacking)
            {
                iconButton.interactable = false;
            } else
            {
                iconButton.interactable = true;
            }*/
        }
    }
}