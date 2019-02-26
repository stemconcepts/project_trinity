using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class Character_Interactions : MonoBehaviour
    {
        bool isSkillactive { get; set; }
        List<GameObject> skillDisplayControllers { get; set; }
        public character_Manager characterManager { get; set; }
        public Button_Click_Manager buttonClickManager { get; set; }
        public Battle_Manager battleManager { get; set; }
        public Character_Interactions()
        {
            skillDisplayControllers = GameObject.FindGameObjectsWithTag("skillDisplayControl");
        }

        public void DisplaySkillsSecond( ){
                for( int x = 0; x < 2 ; x++ ){
                    skillDisplayControllers[x].GetComponent<skillLabeldisplay>().GetSkillData();
                }
                skillDisplayControllers[2].GetComponent<class_skillLabelDisplay>().BuildClassSkill();
        }
        
        //recieves click action
        void OnMouseUp(){
        //-------------------------------------- Enemy Selection ----------------------------------------------//
                if( isSkillactive == true ){
                    skill_targetting.instance.currentTarget = this.gameObject;
                }
        //---------------------------------------- Skill swap ------------------------------------------------//
                //swap to class skills
                if( characterManager.characterModel.role == buttonClickManager.GetClassRole() && characterManager.characterModel.isAlive && !isSkillactive ){
                    // ConvertRoleToBool(GetClassRole());
                    if( buttonClickManager.GetClassRole() != "Boss" ){
                        battleManager.characterSelectManager.guardianSelected = buttonClickManager.ConvertRoleToBool( buttonClickManager.GetClassRole(), "tankSelected" );
                        battleManager.characterSelectManager.walkerSelected = buttonClickManager.ConvertRoleToBool( buttonClickManager.GetClassRole(), "healerSelected" );
                        battleManager.characterSelectManager.stalkerSelected = buttonClickManager.ConvertRoleToBool( buttonClickManager.GetClassRole(), "dpsSelected" );
                        DisplaySkillsSecond();
                    }
                }
                battleManager.soundManager.playSound( battleManager.soundManager.uiSounds[0] );
        }
        
        void OnMouseEnter(){
            if( Input.GetKeyDown( KeyCode.LeftShift ) && buttonClickManager.selectionOverlapScript.overlappedObj != null && !GameObject.Find("CharSelectUI(Clone)") && buttonClickManager.GetClassRole() != "Boss" ){
                var partnerRowOccupied = GetComponent<movement_script>().IsParnetRowOccupied( buttonClickManager.selectionOverlapScript.overlappedObj );
                if( partnerRowOccupied ){
                    var distance = Vector2.Distance(buttonClickManager.selectionOverlapScript.overlappedObj.transform.position, Camera.main.transform.position);
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Vector3 rayPoint = ray.GetPoint(distance);
                    Vector3 spawnPos = gameObject.transform.Find("FXpositions").Find("FXcenter").transform.position;
                    var charOverlapUI = (GameObject)Instantiate( Resources.Load("CharSelectUI"), spawnPos, Quaternion.identity );
                    buttonClickManager.selectionOverlapScript.BuildOverlapList( charOverlapUI );
                    charOverlapUI.transform.SetParent( GameObject.Find("Canvas - UI").transform );
                    charOverlapUI.transform.localScale = new Vector3(1f,1f,1f );
                }
                /*if( overlapUITimer != null && overlapUITimer.Running ){
                    overlapUITimer.Stop();
                }*/
            } else {
                characterManager.characterModel.currentPanel.GetComponent<Image>().color = new Color( 1f, 0.3f, 0.3f, 1f );
            }
        }
        
        void OnMouseExit(){
            var currentPanel = characterManager.characterModel.currentPanel;
            /*if( GameObject.Find("CharSelectUI(Clone)") && GetClassRole() != "Boss" ){
                overlapUITimer = new Task( overlapTimer( 5f ) );
            } else {*/
                if ( currentPanel.GetComponent<movementPanelController>().isVoidZone ){
                    currentPanel.GetComponent<Image>().color = currentPanel.GetComponent<movementPanelController>().voidZoneColor;
                } else if ( currentPanel.GetComponent<movementPanelController>().isVoidCounter ){
                    currentPanel.GetComponent<Image>().color = currentPanel.GetComponent<movementPanelController>().counterZoneColor;
                } else {
                    currentPanel.GetComponent<Image>().color = currentPanel.GetComponent<movementPanelController>().panelColor;
                }
            //}
        }
        
        /*IEnumerator overlapTimer( float waitTime ){
            yield return new WaitForSeconds( waitTime );
            Destroy( GameObject.Find("CharSelectUI(Clone)") );
        }*/
        
        public void SelectChar(){
            if( isSkillactive == true ){
                skill_targetting.instance.currentTarget = characterManager.gameObject;
                print( skill_targetting.instance.currentTarget );
            } 
            if( isSkillactive == false && characterManager.characterModel.isAlive ){
                battleManager.characterSelectManager.guardianSelected = buttonClickManager.ConvertRoleToBool( buttonClickManager.GetClassRole(), "tankSelected" );
                battleManager.characterSelectManager.walkerSelected = buttonClickManager.ConvertRoleToBool( buttonClickManager.GetClassRole(), "healerSelected" );
                battleManager.characterSelectManager.stalkerSelected = buttonClickManager.ConvertRoleToBool( buttonClickManager.GetClassRole(), "dpsSelected" );
                DisplaySkillsSecond();
            }
        }
        
        bool isAlive( string charObj ){
            var charactersdata = GameObject.Find(charObj).GetComponent<character_data>();
            if( charactersdata.isAlive ){
                    return true;
            }   else {
                    return false;
            }
        }
        
        //Change Character
        void CharSwap(){
            if( characterManager ){
        //---------------------------------------- Tab Skill swap ------------------------------------------------//
                battleManager.guardian.Selected = battleManager.characterSelectManager.guardianSelected;
                battleManager.walker.Selected = battleManager.characterSelectManager.walkerSelected;
                battleManager.stalker.Selected = battleManager.characterSelectManager.stalkerSelected;
                
                //string something = GetAlive( charClass );
                string swapTo = battleManager.GetAlive( battleManager.charClass );
                
                switch( swapTo ){
                    case "guardian":
                    battleManager.characterSelectManager.guardianSelected = battleManager.guardian.Selected = battleManager.walker.LastSelected = true;
                    battleManager.characterSelectManager.stalkerSelected = battleManager.stalker.Selected = false;
                    battleManager.characterSelectManager.walkerSelected = battleManager.walker.Selected = battleManager.stalker.LastSelected = false;
                    DisplaySkillsSecond();
                    break;
                    case "walker":
                    battleManager.characterSelectManager.walkerSelected = battleManager.walker.Selected = battleManager.stalker.LastSelected = true;
                    battleManager.characterSelectManager.guardianSelected = battleManager.guardian.Selected = false;
                    battleManager.characterSelectManager.stalkerSelected = battleManager.stalker.Selected = battleManager.guardian.LastSelected = false;
                    DisplaySkillsSecond();
                    break;
                    case "stalker":
                    battleManager.characterSelectManager.stalkerSelected = battleManager.stalker.Selected = battleManager.guardian.LastSelected = true;
                    battleManager.characterSelectManager.guardianSelected = battleManager.guardian.Selected = battleManager.walker.LastSelected = false;
                    battleManager.characterSelectManager.walkerSelected = battleManager.walker.Selected = false;
                    DisplaySkillsSecond();
                    break;
                }
                battleManager.soundManager.playSound( battleManager.soundManager.charSwapSound );
            }
        }

        
        // Update is called once per frame
        void Update () {
            //recieve Tab press
            if( Input.GetKeyUp( KeyCode.Tab ) && !isSkillactive && gameObject.name == "Guardian" ){
                CharSwap();
            }
        }
    }
}

