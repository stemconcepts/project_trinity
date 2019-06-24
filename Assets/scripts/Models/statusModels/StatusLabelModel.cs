using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class StatusLabelModel : MonoBehaviour
    {
        public StatusLabelModel()
        {
        }
        public StatusModel statusModel;
        public string statusname;
        private Image boxcolor;
        public Sprite statusIcon;
        public int labelid;
        public int positionid;
        public bool buff;
        public float buffPower;
        public int stacks = 1;
        public SkillModel onHitSkillPlayer;
        public SkillModel onHitSkillEnemy;
        private GameObject liveStatusHoverObj;
        public bool dispellable;
        public void DestroyMe(){
            Destroy (gameObject);
            if( liveStatusHoverObj ){
                Destroy(liveStatusHoverObj);
            }
        }
        public GameObject statusHoverObj;
        public Task tickTimer;
    
        public void OnMouseEnter(){
            Vector3 rayPoint = Camera.current.ScreenToWorldPoint(Input.mousePosition);
                rayPoint.z = 0f;
                rayPoint.x += 4f;
                if( gameObject.transform.parent.gameObject.transform.parent.gameObject != GameObject.Find("Bossstatus") ){
                    rayPoint.y +=  3f;
                } else {
                    rayPoint.y -=  3f;
                }
                liveStatusHoverObj = (GameObject)Instantiate( statusHoverObj, rayPoint, Quaternion.identity );
                var statusName = liveStatusHoverObj.transform.Find("statusName").GetComponent<Text>();
                var statusDesc = liveStatusHoverObj.transform.Find("statusDesc").GetComponent<Text>();
                liveStatusHoverObj.transform.SetParent( GameObject.Find("Canvas - UI").transform );
                liveStatusHoverObj.transform.localScale = new Vector3(1f,1f,1f);
                var substatusName = statusModel.subStatus != null ? "<i>(" + statusModel.subStatus.subStatusLabel + ")</i>" : "";
                statusName.text = "<b>" + statusModel.singleStatus.displayName + " " + substatusName + "</b>";
                statusDesc.text = statusModel.singleStatus.statusDesc;
        }

        public void OnMouseExit(){
            Destroy(liveStatusHoverObj);
        }
    
        // Use this for initialization
        void Start () {
    
            this.transform.localScale = new Vector3(1.8f,1.8f,1.8f);
    
            boxcolor = GetComponent<Image>();
    
    
            //buffs
            if( statusModel != null && statusModel.singleStatus.buff ){
                buff = true;
            } else {
                buff = false;
            }
    
            if( buff ){
                boxcolor.color = new Color32(185, 233, 0, 255);
            } else {
                boxcolor.color = Color.red;
            }
    
        }
    }
}

