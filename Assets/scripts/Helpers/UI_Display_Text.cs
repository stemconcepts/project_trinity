using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace AssemblyCSharp
{
    public class UI_Display_Text : Generic_Manager
    {
        private Text displayText;
        public string healthdata;
        public bool On = true;
        public CharacterManager characterManager;
        public GameObject characterObject;

        public UI_Display_Text()
        {
        }
        void Start(){
            /*if( On ){
                if( characterObject ){
                    characterManager = characterObject.GetComponent<Character_Manager_Group>().characterManager;
                }
                displayText = GetComponent<Text>();
            }*/
        }

        void Update () {
            if( On ){
                getData();
            }
        }

        void getData(){
            if( characterManager ){
                healthdata = characterManager.characterModel.Health.ToString();
                displayText.text = healthdata;
            }
        }
        
        /*public void SetDataObjects( int i ){
            characterObject = transform.parent.parent.parent.GetChild(2 + i).gameObject;
            characterManager = transform.parent.parent.parent.GetChild(2 + i).GetComponent<Character_Manager>();
            displayText = GetComponent<Text>();
        }*/
    }
}

