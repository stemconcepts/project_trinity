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
        //public Text skillName;
        public Text skillCost;
        public Image iconImageScript;
        public Image skillCDImage;
        public ToolTipController toolTip;
        public KeyCode KeyCode;
        public AudioClip skillSelectSound;

        void Awake()
        {
            skillCost = this.gameObject.GetComponent<Text>();
        }

        public void SkillSet ( Player_Skill_Manager skillManager ) {
            if( buttonID != 3 ){
                if( skillManager.weaponSlot.ToString() == "Main" ){
                    skill = skillManager.primaryWeaponSkills[buttonID];
                } else {
                    skill = skillManager.secondaryWeaponSkills[buttonID];
                }
            } else {
                skill = skillManager.skillModel;
            }
            if( skill != null && skill.skillActive == true ){
                skillCDImage.fillAmount = 1;
                //CooldownClassDisplayCheck( skill );
            } else {
                ClearCD();
            }
            iconImageScript.sprite = skill.skillIcon;
            iconImageScript.type = Image.Type.Filled;
            //skillName.text = skill.skillName;
            skillCost.text = skill.skillCost.ToString();
            toolTip.toolTipName = skill.skillName;
            toolTip.toolTipDesc = skill.skillDesc;

        }

        public void RunClassSkill(){
            Battle_Manager.soundManager.playSound(skillSelectSound);
            if (Battle_Manager.battleStarted && !IsCharBusy() && Battle_Manager.turn == Battle_Manager.TurnEnum.PlayerTurn)
            {
                var charSelected = Battle_Manager.characterSelectManager.GetSelectedClassObject();
                var charStatus = charSelected.GetComponent<Status_Manager>();
                if (!charStatus.DoesStatusExist("stun"))
                {
                    charSelected.GetComponent<Player_Skill_Manager>().PrepSkill(skill);
                }
            }
        }

        void KeyPressSkill(){
            Battle_Manager.soundManager.playSound(skillSelectSound);
            RunClassSkill();
            /*var charSelected = Battle_Manager.characterSelectManager.GetSelectedClassObject();
            var charStatus = charSelected.GetComponent<Status_Manager>();
            if (!charStatus.DoesStatusExist("stun"))
            {
                charSelected.GetComponent<Player_Skill_Manager>().PrepSkill(skill);
            }*/
        }

        public void KeyPressCancelSkill()
        {
            if (skill.skillActive)
            {
                var charSelected = Battle_Manager.characterSelectManager.GetSelectedClassObject();
                charSelected.GetComponent<Player_Skill_Manager>().SkillActiveSet(skill, false);
            }
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
            var baseManager = Battle_Manager.characterSelectManager.GetSelectedClassObject().GetComponent<Base_Character_Manager>();
            if( iconImageScript ){
                if(skill != null && (skill.skillCost > Battle_Manager.actionPoints))
                {
                    iconImageScript.color = new Color(0.9f, 0.2f, 0.2f);
                } else if (Battle_Manager.turn == Battle_Manager.TurnEnum.EnemyTurn || baseManager.statusManager.DoesStatusExist("Stun"))
                {
                    iconImageScript.color = new Color(0.4f, 0.4f, 0.4f);
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
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    KeyPressCancelSkill();
                }
                if (Input.GetKeyDown(KeyCode.Space))
                { 
                        Battle_Manager.PauseGame();
                }
            }
        }
    }
}

