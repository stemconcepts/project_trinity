using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace AssemblyCSharp
{
    public class Battle_Interface_Manager: BasicManager
    {
        bool swapReady = true;
        float gearSwapTime {get; set;}
        public int buttonID { get; set; }
        public string skillNames {get; set;}
        public SkillModel skill {get; set;}
        public string skillName { get; set; }
        public Sprite skillIcon { get; set; }
        public Image iconImageScript { get; set; }
        public Image skillCDImage { get; set; }
        public KeyCode KeyCode;
        public Battle_Interface_Manager()
        {
        }

        public void SkillSet ( Skill_Manager skillManager ) {
            if( skillManager.weaponSlot.ToString() == "Main" ){
                skill = skillManager.primaryWeaponSkills[buttonID];
            } else {
                skill = skillManager.secondaryWeaponSkills[buttonID];
            }
            if( skill.skillActive == true ){
                CooldownClassDisplayCheck( skill );
            } else {
                ClearCD();
            }
            iconImageScript.sprite = skillIcon;
            iconImageScript.type = Image.Type.Filled;
        }

        public void RunClassSkill(){
            if( !IsCharBusy() ){
                var charSelected = Battle_Manager.characterSelectManager.GetSelectedClassObject();
                charSelected.GetComponent<Skill_Manager>().PrepSkillNew(skill);
            }
        }

        void KeyPressSkill(){
            var charSelected = Battle_Manager.characterSelectManager.GetSelectedClassObject();
            charSelected.GetComponent<Skill_Manager>().PrepSkillNew(skill);
        }
    
        //For skill : Run to check if skills are on Cooldown - if so set the fill amount to the remaining time left
        public void CooldownClassDisplayCheck( SkillModel skill ){
            float timeLeft = 1f/skill.skillCooldown * skill.currentCDAmount;
            skillCDImage.fillAmount = 1f - timeLeft;
        }

        public void ClearCD(){
            skillCDImage.fillAmount = 0f;
        }
    
        void CanAffordSkill(){
            var characterManager = Battle_Manager.characterSelectManager.GetSelectedClassObject().GetComponent<Character_Manager>();
            if( skill.skillCost > characterManager.characterModel.actionPoints ){
                this.gameObject.GetComponent<Image>().color = new Color(0.9f, 0.2f, 0.2f);
            } else {
                this.gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f);
            }
        }
    
        bool IsCharBusy(){
            var characterManager = Battle_Manager.characterSelectManager.GetSelectedClassObject().GetComponent<Character_Manager>();
            return characterManager.animationManager.inAnimation;
        }
    
        void Update () {
            CanAffordSkill();
            if( Input.GetKeyDown( KeyCode ) && !IsCharBusy() ){
                KeyPressSkill();
            }
        }

        public void SwapGear(){
            var skillactive = Battle_Manager.friendlyCharacters.Any( x => x.skillManager.isSkillactive == true );
            if( swapReady && !skillactive ){
                //var buttonDataScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<sortbuttondata>();
                var allRoles = Battle_Manager.friendlyCharacters;
                for( int i = 0; i < allRoles.Count; i++ ){
                    var currentWSlot = allRoles[i].GetComponent<Skill_Manager>();
                    var currentWeaponData = allRoles[i].GetComponent<Equipment_Manager>();
                    if( currentWSlot.weaponSlot == Skill_Manager.weaponSlotEnum.Main ){
                        currentWSlot.weaponSlot = Skill_Manager.weaponSlotEnum.Alt;
                        currentWeaponData.currentWeaponEnum = Equipment_Manager.currentWeapon.Secondary;
                    } else {
                        currentWSlot.weaponSlot = Skill_Manager.weaponSlotEnum.Main;
                        currentWeaponData.currentWeaponEnum = Equipment_Manager.currentWeapon.Primary;
                    }
                    //restore Action Points - should be changed to GearSwap ability
                    var charData = allRoles[i].GetComponent<Character_Manager>();
                    charData.characterModel.actionPoints = charData.characterModel.originalactionPoints;
                    SkillSet( allRoles[i].skillManager );
                    allRoles[i].GetComponent<Character_Interaction_Manager>().DisplaySkills();
                }
                CheckGearType();
                swapReady = false;
                GearSwapTimer(gearSwapTime);
                Battle_Manager.soundManager.playSound("gearSwapSound");
            } else {
                print ("Gear Swap not Ready");
            }
        }
    
        void GearSwapTimer( float time ){
            Battle_Manager.taskManager.CallTask( time, () => {
                swapReady = true;
            });
            Battle_Manager.soundManager.playSound( "gearSwapReady" );
        }
    
        private void CheckGearType(){
            //var allRoles = GameObject.FindGameObjectsWithTag("Player"); 
            foreach (var playerRole in Battle_Manager.friendlyCharacters ) {
                var characterManager = playerRole.GetComponent<Character_Manager>(); 
                var currentWeaponData = characterManager.equipmentManager;
                var playerSkeletonAnim = characterManager.animationManager;
                var AAutoAttack = characterManager.autoAttackManager;
                var charMovementScript = characterManager.movementManager;
                var calculateDmgScript = characterManager.damageManager;
                var currentWSlot = characterManager.skillManager;
                var weaponType = currentWSlot.weaponSlot == Skill_Manager.weaponSlotEnum.Main ? currentWeaponData.primaryWeapon : currentWeaponData.secondaryWeapon;
                if( weaponType.type != weaponModel.weaponType.heavyHanded && weaponType.type != weaponModel.weaponType.cursedGlove && weaponType.type != weaponModel.weaponType.clawAndCannon ){
                    if( playerRole.name == "Stalker" ){
                        playerSkeletonAnim.skeletonAnimation.skeleton.SetSkin("light");
                    }
                    playerSkeletonAnim.skeletonAnimation.state.AddAnimation(0, "idle", true, 0 );
                    AAutoAttack.AAanimation = "attack1";
                    charMovementScript.idleAnim = "idle";
                    charMovementScript.hopAnim = "hop";
                    calculateDmgScript.charDamageModel.hitAnimNormal = "hit";
                } else {
                    if( playerRole.name == "Stalker" ){
                        playerSkeletonAnim.skeletonAnimation.skeleton.SetSkin("heavy");
                    }
                    playerSkeletonAnim.skeletonAnimation.state.SetAnimation(0, "toHeavy", false );
                    playerSkeletonAnim.skeletonAnimation.state.AddAnimation(0, "idleHeavy", true, 0 );
                    AAutoAttack.AAanimation = "attack1Heavy";
                    charMovementScript.idleAnim = "idleHeavy";
                    charMovementScript.hopAnim = "hopHeavy";
                    calculateDmgScript.charDamageModel.hitAnimNormal = "hitHeavy";
                }
            }   
        }
    }
}

