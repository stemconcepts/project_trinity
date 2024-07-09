using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Assets.scripts.Models.statusModels;

namespace AssemblyCSharp
{
    public class BattleInterfaceManager: MonoBehaviour
    {
        public int buttonID;
        public SkillModel skill;
        //public Text skillName;
        public Text skillCost;
        public Image iconImageScript;
        public Image skillCDImage;
        //public ToolTipController toolTip;
        public ToolTipTriggerController toolTip;
        public KeyCode KeyCode;
        public AudioClip skillSelectSound;

        void Awake()
        {
            skillCost = this.gameObject.GetComponent<Text>();
        }

        public void SkillSet ( PlayerSkillManager skillManager ) {
            toolTip.ClearToolTips();
            if ( buttonID != 3 ){
                if( skillManager.weaponSlot.ToString() == "Main" ){
                    skill = skillManager.primaryWeaponSkills[buttonID];
                } else {
                    skill = skillManager.secondaryWeaponSkills[buttonID];
                }
            } else {
                skill = skillManager.classSkillModel;
            }
            if( skill != null && skill.skillActive == true ){
                skillCDImage.fillAmount = 1;
                //CooldownClassDisplayCheck( skill );
            } else {
                ClearCD();
            }
            iconImageScript.sprite = skill?.skillIcon;
            iconImageScript.type = Image.Type.Filled;
            //skillName.text = skill.skillName;
            skillCost.text = skill.skillCost.ToString();
            toolTip.AddtoolTip(skill.skillName, skill.skillName, skill.skillDesc + skill.GetExtraDesc());
            //toolTip.toolTipName = skill.skillName;
            //toolTip.toolTipDesc = skill.skillDesc;

        }

        public void RunClassSkill(){
            //MainGameManager.instance.soundManager.playSound(skillSelectSound);
            if (BattleManager.battleStarted && !IsCharBusy() && BattleManager.turn == BattleManager.TurnEnum.PlayerTurn)
            {
                var charSelected = BattleManager.characterSelectManager.GetSelectedClassObject();
                charSelected.GetComponent<PlayerSkillManager>().PrepSkill(skill);
                /*var charSelected = BattleManager.characterSelectManager.GetSelectedClassObject();
                var charStatus = charSelected.GetComponent<StatusManager>();
                if (!charStatus.DoesStatusExist("stun"))
                {
                    charSelected.GetComponent<PlayerSkillManager>().PrepSkill(skill);
                }*/
            }
        }

        void KeyPressSkill(){
            //MainGameManager.instance.soundManager.playSound(skillSelectSound);
            RunClassSkill();
        }

        public void KeyPressCancelSkill()
        {
            if (skill.skillActive)
            {
                var charSelected = BattleManager.characterSelectManager.GetSelectedClassObject();
                charSelected.GetComponent<PlayerSkillManager>().SkillActiveSet(skill, false);
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
            var baseManager = BattleManager.characterSelectManager.GetSelectedClassObject().GetComponent<BaseCharacterManagerGroup>();
            if( iconImageScript ){
                if(skill != null && (skill.skillCost > BattleManager.actionPoints))
                {
                    iconImageScript.color = new Color(0.9f, 0.4f, 0.4f);
                } else if (BattleManager.turn == BattleManager.TurnEnum.EnemyTurn || baseManager.statusManager.DoesStatusExist(StatusNameEnum.Stun) || !skill.CanCastFromPosition(skill.compatibleRows, baseManager))
                {
                    iconImageScript.color = new Color(0.3f, 0.3f, 0.3f);
                } else {
                    iconImageScript.color = new Color(1f, 1f, 1f);
                }
            }
        }
    
        public bool IsCharBusy(){
            var charSelected = BattleManager.characterSelectManager.GetSelectedClassObject();
            var charStatus = charSelected.GetComponent<StatusManager>();
            var stunned = charStatus.DoesStatusExist(StatusNameEnum.Stun);
            var bm = BattleManager.characterSelectManager.GetSelectedClassObject().GetComponent<BaseCharacterManagerGroup>();
            return bm.animationManager.inAnimation || bm.skillManager.isCasting || stunned;
        }
    
        void Update () {
            if (BattleManager.battleStarted)
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
                    BattleManager.PauseGame();
                }
            }
        }
    }
}

