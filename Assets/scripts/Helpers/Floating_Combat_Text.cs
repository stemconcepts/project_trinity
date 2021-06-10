using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace AssemblyCSharp
{
    public class Floating_Combat_Text : Generic_Manager
    {
        private float currentPositionX;
        private float currentPositionY;
        public string skillLabel;
        public bool isDmg;
        public bool isAbsorb;
        public bool isImmune;
        public bool showDmgNumber = true;
        public int healData;
        public int absorbData;
        public int damageData;
        public string extraInfo;
        public int fixedData;
        public GameObject textObject;
        private Text displayText;
        public int fontSize;

        void Move(){
            currentPositionX = this.transform.position.x;
            currentPositionY = this.transform.position.y;
            var dmgPositionY = currentPositionY + 0.01f;
            var healPositionY = currentPositionY - 0.01f;
            if( isDmg ){
                this.transform.position = new Vector2( currentPositionX, dmgPositionY ) ;
            } else {
                this.transform.position = new Vector2( currentPositionX, healPositionY ) ;
            }
        }
    
        void CreateData() {
            displayText = textObject.GetComponent<Text>();
            displayText.fontSize = fontSize == 0 ? 200 : fontSize;
            if ( isDmg ){
                //displayText.text = skillLabel + ": -" + damageData.ToString();
                displayText.text = string.IsNullOrEmpty(extraInfo) ? (showDmgNumber ? damageData.ToString() : "") : (showDmgNumber ? damageData.ToString() : "") + extraInfo;
                //displayText.text += (modifiedDamage ? "*" : "");
            } else 
            if ( isAbsorb ) {
                //displayText.text = skillLabel + ":" + absorbData.ToString();
                displayText.text = /*absorbData.ToString() +*/extraInfo;
                displayText.color = new Color(0.1f, 0.5f, 0.9f, 1f);
                //displayText.text += (modifiedDamage ? "*" : "");
            } else 
            if ( isImmune ) {
                    //displayText.text = skillLabel + ": Immune";
                    displayText.text = "Immune";
                    displayText.color = Color.white;
            } else {
                //displayText.text = "+" + healData.ToString();
                displayText.text = healData.ToString();
                displayText.color = Color.green;
                //displayText.text += (modifiedDamage ? "*" : "");
            }
        }
    
        // Use this for initialization
        void Start () {
            CreateData();
                    CanvasGroup cg = displayText.GetComponent<CanvasGroup>();
                        if( cg ) {
                            cg.alpha = 1;
                            canvasFader cf = cg.gameObject.GetComponent<canvasFader>();
            
                            if ( cf ){
                                cf.Restart();
                            } else {
                                cg.gameObject.AddComponent<canvasFader>();
                            }
                        }
            Battle_Manager.taskManager.CallTask(3f, () => {
                Destroy(this.gameObject);
            });
        }
        
        // Update is called once per frame
        void Update () {
            Move();
        }
    }
}

