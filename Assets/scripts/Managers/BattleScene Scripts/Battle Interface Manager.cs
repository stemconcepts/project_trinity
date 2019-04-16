using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class Battle_Interface_Manager: BasicManager
    {
        public int buttonID { get; set; }
        public string skillNames {get; set;}
        public classSkills skill {get; set;}
        public string skillName { get; set; }
        public Sprite skillIcon { get; set; }
        private Image iconImageScript { get; set; }
        private Image skillCDImage { get; set; }
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
                skillLoaderScript.PrepSkillNew( skill );
            }
        }

        void KeyPressSkill(){
            skillLoaderScript.PrepSkillNew( skill );
        }
    
        //For skill : Run to check if skills are on Cooldown - if so set the fill amount to the remaining time left
        public void CooldownClassDisplayCheck( classSkills skill ){
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
            var skillactive = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<skill_cd>();
            if( swapReady == true && !skillactive.skillActive ){
                var buttonDataScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<sortbuttondata>();
                var allRoles = GameObject.FindGameObjectsWithTag("Player"); 
                for( int i = 0; i < allRoles.Length; i++ ){
                    var currentWSlot = allRoles[i].GetComponent<skill_effects>();
                    var currentWeaponData = allRoles[i].GetComponent<equipedWeapons>();
                    if( currentWSlot.weaponSlot == skill_effects.weaponSlotEnum.Main ){
                        currentWSlot.weaponSlot = skill_effects.weaponSlotEnum.Alt;
                        //print( currentWeaponData.secondaryWeapon.type.ToString() );
                        currentWeaponData.currentWeaponEnum = equipedWeapons.currentWeapon.Secondary;
                        //CheckGearType();
                    } else {
                        currentWSlot.weaponSlot = skill_effects.weaponSlotEnum.Main;
                        //print( currentWeaponData.primaryWeapon.type.ToString() );
                        currentWeaponData.currentWeaponEnum = equipedWeapons.currentWeapon.Primary;
                        //CheckGearType();
                    }
                    //restore Action Points - should be changed to GearSwap ability
                    var charData = allRoles[i].GetComponent<character_data>();
                    charData.actionPoints = charData.originalactionPoints;
                }
                CheckGearType();
                buttonDataScript.SkillSet();
                allRoles[0].GetComponent<button_clicks>().DisplaySkillsSecond();
                swapReady = false;
                GearSwapTimer(time);
                soundContScript.playSound( gearSwapSound );
            } else {
                print ("Gear Swap not Ready");
            }
        }
    
        void GearSwapTimer( float time ){
            swapTimer = new Task( SwapTimer( time ) );
            soundContScript.playSound( gearSwapReady );
        }
    
        IEnumerator SwapTimer( float time){
            yield return new WaitForSeconds(time);
            swapReady = true;
        }
    
        private void CheckGearType(){
            var allRoles = GameObject.FindGameObjectsWithTag("Player"); 
            foreach (var playerRole in allRoles) {
                var currentWeaponData = playerRole.GetComponent<equipedWeapons>();  
                var playerSkeletonAnim = playerRole.GetComponentInChildren<SkeletonAnimation>();
                var AAutoAttack = playerRole.GetComponent<auto_attack>();
                var charMovementScript = playerRole.GetComponent<characterMovementController>();
                var calculateDmgScript = playerRole.GetComponent<calculateDmg>();
                var currentWSlot = playerRole.GetComponent<skill_effects>();
                var weaponType = currentWSlot.weaponSlot == skill_effects.weaponSlotEnum.Main ? currentWeaponData.primaryWeapon : currentWeaponData.secondaryWeapon;
                if( weaponType.type != weapons.weaponType.heavyHanded && weaponType.type != weapons.weaponType.cursedGlove && weaponType.type != weapons.weaponType.clawAndCannon ){
                    if( playerRole.name == "Stalker" ){
                        playerSkeletonAnim.skeleton.SetSkin("light");
                    }
                    playerSkeletonAnim.state.AddAnimation(0, "idle", true, 0 );
                    AAutoAttack.AAanimation = "attack1";
                    charMovementScript.idleAnim = "idle";
                    charMovementScript.hopAnim = "hop";
                    calculateDmgScript.hitAnimNormal = "hit";
                } else {
                    if( playerRole.name == "Stalker" ){
                        playerSkeletonAnim.skeleton.SetSkin("heavy");
                    }
                    playerSkeletonAnim.state.SetAnimation(0, "toHeavy", false );
                    playerSkeletonAnim.state.AddAnimation(0, "idleHeavy", true, 0 );
                    AAutoAttack.AAanimation = "attack1Heavy";
                    charMovementScript.idleAnim = "idleHeavy";
                    charMovementScript.hopAnim = "hopHeavy";
                    calculateDmgScript.hitAnimNormal = "hitHeavy";
                }
            }   
        }
    }
}

