using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

namespace AssemblyCSharp
{
    public class GearSwapManager : MonoBehaviour
    {
        public bool swapReady = true;
        public float gearSwapTime = 10f;
        public Button iconButton;
        public void SwapGear()
        {
            var skillactive = BattleManager.characterSelectManager.friendlyCharacters.Any(x => x.baseManager.skillManager.isSkillactive);
            var isAttacking = BattleManager.characterSelectManager.friendlyCharacters.Any(x => x.baseManager.autoAttackManager.isAttacking);
            if (swapReady && !skillactive && !isAttacking && BattleManager.battleStarted)
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
                //Battle_Manager.characterSelectManager.friendlyCharacters.ForEach(o => o.characterModel.actionPoints = o.characterModel.originalactionPoints);
            }
            else
            {
                print("Gear Swap not Ready");
            }
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

        void Awake()
        {
            //iconButton = gameObject.GetComponent<Button>();
        }

        void Update()
        {
            var skillactive = BattleManager.characterSelectManager.friendlyCharacters.Any(x => x.baseManager.skillManager.isSkillactive);
            var isAttacking = BattleManager.characterSelectManager.friendlyCharacters.Any(x => x.baseManager.autoAttackManager.isAttacking);
            if (!swapReady || skillactive || isAttacking)
            {
                iconButton.interactable = false;
            } else
            {
                iconButton.interactable = true;
            }
        }
    }
}