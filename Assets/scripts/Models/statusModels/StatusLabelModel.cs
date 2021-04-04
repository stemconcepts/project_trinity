using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AssemblyCSharp
{
    [ExecuteInEditMode()]
    public class StatusLabelModel : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
    {
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
        public enemySkill onHitSkillEnemy;
        private GameObject liveStatusHoverObj;
        //public Text statusName;
        //public Text statusDesc;
        public LayoutElement layoutElement;
        public int characterWrapLimit;
        public bool dispellable;
        public void DestroyMe(){
            Destroy (gameObject);
            if( liveStatusHoverObj ){
                Destroy(liveStatusHoverObj);
            }
        }
        public GameObject statusHoverObj;
        public Task tickTimer;
        public Task durationTimer;

        /*public void OnPointerEnter(PointerEventData eventData)
        {

        }

        public void OnPointerExit(PointerEventData eventData)
        {

        }*/

        public void OnMouseEnter(){
            //Battle_Manager.taskManager.CallTask(0.5f, () =>
            //{
                //current
                /*Vector3 rayPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    rayPoint.z = 0f;
                    rayPoint.x += 4f;
                    if( gameObject.transform.parent.gameObject.transform.parent.gameObject != GameObject.Find("bossstatus") ){
                        rayPoint.y +=  1.5f;
                    } else {
                        rayPoint.y -=  1.5f;
                    }*/
                liveStatusHoverObj = (GameObject)Instantiate(statusHoverObj /*rayPoint, Quaternion.identity*/ );
                var statusName = liveStatusHoverObj.transform.Find("statusName").GetComponent<Text>();
                var statusDesc = liveStatusHoverObj.transform.Find("statusDesc").GetComponent<Text>();
                layoutElement = liveStatusHoverObj.GetComponent<LayoutElement>();
                liveStatusHoverObj.transform.SetParent(Battle_Manager.tooltipCanvas.transform);
                liveStatusHoverObj.transform.localScale = new Vector3(1f, 1f, 1f);
                var substatusName = statusModel.singleStatus.subStatus != null ? "<i>(" + statusModel.singleStatus.subStatus.subStatusLabel + ")</i>" : "";
                statusName.text = "<b>" + statusModel.singleStatus.displayName + " " + substatusName + "</b>";
                statusDesc.text = statusModel.singleStatus.statusDesc;
                int headerLength = statusName.text.Length;
                int contentLength = statusDesc.text.Length;
                layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
            //}, "tooltipTimer");
        }

        public void OnMouseExit(){
           // Battle_Manager.taskManager.taskList.Remove("tooltipTimer");
            Destroy(liveStatusHoverObj);
        }

        /*void Update()
        {
            if (statusName != null || statusDesc != null)
            {
                int headerLength = statusName.text.Length;
                int contentLength = statusDesc.text.Length;
                layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
            }
            
        }*/
    
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

