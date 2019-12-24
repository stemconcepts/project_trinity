using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace AssemblyCSharp
{
    public class Battle_Interface_Manager: MonoBehaviour
    {
        
        public int buttonID;
        public SkillModel skill;
        public Text skillName;
        public Image iconImageScript;
        public Image skillCDImage;
        public KeyCode KeyCode;

        void Start()
        {
            skillName = this.gameObject.GetComponent<Text>();
        }

        public void SkillSet ( Skill_Manager skillManager ) {
            if( buttonID != 2 ){
                if( skillManager.weaponSlot.ToString() == "Main" ){
                    skill = skillManager.primaryWeaponSkills[buttonID];
                } else {
                    skill = skillManager.secondaryWeaponSkills[buttonID];
                }
            } else {
                skill = skillManager.skillModel;
            }
            if( skill != null && skill.skillActive == true ){
                CooldownClassDisplayCheck( skill );
            } else {
                ClearCD();
            }
            iconImageScript.sprite = skill.skillIcon;
            iconImageScript.type = Image.Type.Filled;
            skillName.text = skill.skillName;
        }

        public void RunClassSkill(){
            if( Battle_Manager.battleStarted && !IsCharBusy() ){
                var charSelected = Battle_Manager.characterSelectManager.GetSelectedClassObject();
                charSelected.GetComponent<Skill_Manager>().PrepSkill(skill);
            }
        }

        void KeyPressSkill(){
            var charSelected = Battle_Manager.characterSelectManager.GetSelectedClassObject();
            charSelected.GetComponent<Skill_Manager>().PrepSkill(skill);
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
            if( iconImageScript ){
                if( skill != null && (skill.skillCost > characterManager.characterModel.actionPoints) ){
                    iconImageScript.color = new Color(0.9f, 0.2f, 0.2f);
                } else {
                    iconImageScript.color = new Color(1f, 1f, 1f);
                }
            }
        }
    
        bool IsCharBusy(){
            var bm = Battle_Manager.characterSelectManager.GetSelectedClassObject().GetComponent<Base_Character_Manager>();
            return bm.animationManager.inAnimation;
        }
    
        void Update () {
            if (Battle_Manager.battleStarted)
            {
                CanAffordSkill();
                if (Input.GetKeyDown(KeyCode) && !IsCharBusy())
                {
                    KeyPressSkill();
                }
            }
        }
    }
}

