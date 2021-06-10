using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace AssemblyCSharp
{
    public class GearSwapManager : MonoBehaviour
    {
        public bool swapReady = true;
        public float gearSwapTime = 10f;
        private Button iconButton;
        public void SwapGear()
        {
            var skillactive = Battle_Manager.characterSelectManager.friendlyCharacters.Any(x => x.baseManager.skillManager.isSkillactive);
            var isAttacking = Battle_Manager.characterSelectManager.friendlyCharacters.Any(x => x.baseManager.autoAttackManager.isAttacking);
            if (swapReady && !skillactive && !isAttacking && Battle_Manager.battleStarted)
            {
                var allRoles = Battle_Manager.characterSelectManager.friendlyCharacters;
                for (int i = 0; i < allRoles.Count; i++)
                {
                    var currentWSlot = allRoles[i].GetComponent<Skill_Manager>();
                    var currentWeaponData = allRoles[i].GetComponent<Equipment_Manager>();
                    if (((Player_Skill_Manager)currentWSlot).weaponSlot == Player_Skill_Manager.weaponSlotEnum.Main)
                    {
                        ((Player_Skill_Manager)currentWSlot).weaponSlot = Player_Skill_Manager.weaponSlotEnum.Alt;
                        currentWeaponData.currentWeaponEnum = Equipment_Manager.currentWeapon.Secondary;
                        allRoles[i].characterModel.canAutoAttack = allRoles[i].baseManager.equipmentManager.secondaryWeapon.enablesAutoAttacks;
                    }
                    else
                    {
                        ((Player_Skill_Manager)currentWSlot).weaponSlot = Player_Skill_Manager.weaponSlotEnum.Main;
                        currentWeaponData.currentWeaponEnum = Equipment_Manager.currentWeapon.Primary;
                        allRoles[i].characterModel.canAutoAttack = allRoles[i].baseManager.equipmentManager.primaryWeapon.enablesAutoAttacks;
                    }
                    var charData = allRoles[i].GetComponent<Character_Manager>();
                    Battle_Manager.battleInterfaceManager.ForEach(o =>
                    {
                        o.SkillSet((Player_Skill_Manager)allRoles[i].baseManager.skillManager);
                    });
                    Battle_Manager.characterSelectManager.GetSelectedClassObject().GetComponent<Character_Interaction_Manager>().DisplaySkills();
                }
                CheckGearType();
                swapReady = false;
                GearSwapTimer(gearSwapTime);
                Battle_Manager.soundManager.playSound("gearSwapSound");
                //Battle_Manager.characterSelectManager.friendlyCharacters.ForEach(o => o.characterModel.actionPoints = o.characterModel.originalactionPoints);
            }
            else
            {
                print("Gear Swap not Ready");
            }
        }

        void GearSwapTimer(float time)
        {
            Battle_Manager.taskManager.CallTask(time, () =>
            {
                swapReady = true;
            });
            //Battle_Manager.soundManager.playSound("gearSwapReady");
        }

        private void CheckGearType()
        {
            foreach (var playerRole in Battle_Manager.characterSelectManager.friendlyCharacters)
            {
                var bm = playerRole.GetComponent<Base_Character_Manager>();
                var currentWeaponData = bm.equipmentManager;
                var playerSkeletonAnim = bm.animationManager;
                var AAutoAttack = bm.autoAttackManager;
                var charMovementScript = bm.movementManager;
                var calculateDmgScript = bm.damageManager;
                var currentWSlot = (Player_Skill_Manager)bm.skillManager;
                var weaponType = currentWSlot.weaponSlot == Player_Skill_Manager.weaponSlotEnum.Main ? currentWeaponData.primaryWeapon : currentWeaponData.secondaryWeapon;
                if (weaponType.type != weaponModel.weaponType.heavyHanded && weaponType.type != weaponModel.weaponType.cursedGlove && weaponType.type != weaponModel.weaponType.clawAndCannon)
                {
                    if (playerRole.name == "Stalker")
                    {
                        playerSkeletonAnim.skeletonAnimation.skeleton.SetSkin("light");
                    }
                    bm.animationManager.attackAnimation = "attack1";
                    bm.animationManager.idleAnimation = "idle";
                    bm.animationManager.hopAnimation = "hop";
                    bm.animationManager.hitAnimation = "hit";
                }
                else
                {
                    if (playerRole.name == "Stalker")
                    {
                        playerSkeletonAnim.skeletonAnimation.skeleton.SetSkin("heavy");
                    }
                    bm.animationManager.attackAnimation = bm.animationManager.attackAnimation + "Heavy";
                    bm.animationManager.idleAnimation = bm.animationManager.idleAnimation == "stunned" ? bm.animationManager.idleAnimation :  bm.animationManager.idleAnimation + "Heavy";
                    bm.animationManager.hopAnimation = bm.animationManager.hopAnimation + "Heavy";
                    bm.animationManager.hitAnimation = bm.animationManager.hitAnimation + "Heavy";
                }
                bm.animationManager.skeletonAnimation.state.SetAnimation(0, bm.animationManager.toHeavy, false);
                bm.animationManager.skeletonAnimation.state.AddAnimation(0, bm.animationManager.idleAnimation, true, 0);
            }
        }

        void Awake()
        {
            iconButton = gameObject.GetComponent<Button>();
        }

        void Update()
        {
            var skillactive = Battle_Manager.characterSelectManager.friendlyCharacters.Any(x => x.baseManager.skillManager.isSkillactive);
            var isAttacking = Battle_Manager.characterSelectManager.friendlyCharacters.Any(x => x.baseManager.autoAttackManager.isAttacking);
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