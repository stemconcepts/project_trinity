using UnityEngine;
using System.Linq;

namespace AssemblyCSharp
{
    public class GearSwapManager : MonoBehaviour
    {
        public bool swapReady = true;
        public float gearSwapTime;

        public void SwapGear()
        {
            var skillactive = Battle_Manager.friendlyCharacters.Any(x => x.baseManager.skillManager.isSkillactive);
            if (swapReady && !skillactive)
            {
                //var buttonDataScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<sortbuttondata>();
                var allRoles = Battle_Manager.friendlyCharacters;
                for (int i = 0; i < allRoles.Count; i++)
                {
                    var currentWSlot = allRoles[i].GetComponent<Skill_Manager>();
                    var currentWeaponData = allRoles[i].GetComponent<Equipment_Manager>();
                    if (currentWSlot.weaponSlot == Skill_Manager.weaponSlotEnum.Main)
                    {
                        currentWSlot.weaponSlot = Skill_Manager.weaponSlotEnum.Alt;
                        currentWeaponData.currentWeaponEnum = Equipment_Manager.currentWeapon.Secondary;
                    }
                    else
                    {
                        currentWSlot.weaponSlot = Skill_Manager.weaponSlotEnum.Main;
                        currentWeaponData.currentWeaponEnum = Equipment_Manager.currentWeapon.Primary;
                    }
                    //restore Action Points - should be changed to GearSwap ability
                    var charData = allRoles[i].GetComponent<Character_Manager>();
                    //charData.characterModel.actionPoints = charData.characterModel.originalactionPoints;
                    Battle_Manager.battleInterfaceManager.ForEach(o =>
                    {
                        o.SkillSet(allRoles[i].baseManager.skillManager);
                    });
                    
                    //allRoles[i].GetComponent<Character_Interaction_Manager>().DisplaySkills();
                    Battle_Manager.characterSelectManager.GetSelectedClassObject().GetComponent<Character_Interaction_Manager>().DisplaySkills();
                }
                CheckGearType();
                swapReady = false;
                GearSwapTimer(gearSwapTime);
                Battle_Manager.soundManager.playSound("gearSwapSound");
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
            Battle_Manager.soundManager.playSound("gearSwapReady");
        }

        private void CheckGearType()
        {
            //var allRoles = GameObject.FindGameObjectsWithTag("Player"); 
            foreach (var playerRole in Battle_Manager.friendlyCharacters)
            {
                var bm = playerRole.GetComponent<Base_Character_Manager>();
                var currentWeaponData = bm.equipmentManager;
                var playerSkeletonAnim = bm.animationManager;
                var AAutoAttack = bm.autoAttackManager;
                var charMovementScript = bm.movementManager;
                var calculateDmgScript = bm.damageManager;
                var currentWSlot = bm.skillManager;
                var weaponType = currentWSlot.weaponSlot == Skill_Manager.weaponSlotEnum.Main ? currentWeaponData.primaryWeapon : currentWeaponData.secondaryWeapon;
                if (weaponType.type != weaponModel.weaponType.heavyHanded && weaponType.type != weaponModel.weaponType.cursedGlove && weaponType.type != weaponModel.weaponType.clawAndCannon)
                {
                    if (playerRole.name == "Stalker")
                    {
                        playerSkeletonAnim.skeletonAnimation.skeleton.SetSkin("light");
                    }
                    // playerSkeletonAnim.skeletonAnimation.state.AddAnimation(0, "idle", true, 0 );
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
    }
}