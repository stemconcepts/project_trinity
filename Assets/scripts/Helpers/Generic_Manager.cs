using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Generic_Manager : MonoBehaviour {
        private Text displayText;
        public string healthdata;
        public bool On = true;
        public Character_Manager characterManager;
        public GameObject characterObject;

        void Start(){
            if( On ){
                if( characterObject ){
                    characterManager = characterObject.GetComponent<Character_Manager>();
                }
                displayText = GetComponent<Text>();
            }
        }

        void Update () {
            if( On ){
                getData();
            }
        }

        public void DestroyObject(){
            Battle_Manager.battleDetailsManager.DestroyObject(this.gameObject);
        }

        void getData(){
            if( characterManager ){
                healthdata = characterManager.characterModel.Health.ToString();
                displayText.text = healthdata;
            }
        }
        
        public void SetDataObjects( int i ){
            characterObject = transform.parent.parent.parent.GetChild(2 + i).gameObject;
            characterManager = transform.parent.parent.parent.GetChild(2 + i).GetComponent<Character_Manager>();
            displayText = GetComponent<Text>();
        }
    }
}