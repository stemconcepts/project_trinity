using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.scripts.Models.statusModels;
using TMPro;

namespace AssemblyCSharp
{
    [ExecuteInEditMode()]
    public class StatusLabelModel : MonoBehaviour
    {
        public StatusModel statusModel;
        public StatusNameEnum statusName
        {
            get
            {
                return (StatusNameEnum)Enum.Parse(typeof(StatusNameEnum), this.statusModel.singleStatus.name);
            }
        }
        private Image boxcolor;
        public Sprite statusIcon;
        public int labelid;
        public int positionid;
        public bool buff
        {
            get
            {
                if (statusModel != null)
                {
                    return statusModel.singleStatus.buff;
                }
                return false;
            }
        }
        public float buffPower;
        //public int stacks = 1;
        public int _stacks = 1;
        public int stacks
        {
            get => _stacks;
            set
            {
                var statusSuffix = value > 1 ? $" x{value}" : "";
                if (ToolTipController && !string.IsNullOrEmpty(ToolTipId))
                {
                    ToolTipController.EditToolTip(
                        ToolTipId,
                        $"{statusModel.singleStatus.displayName}{statusSuffix}");
                }
                _stacks = value;
            }
        }
        public SkillModel onHitSkillPlayer;
        public enemySkill onHitSkillEnemy;
        public LayoutElement layoutElement;
        public int characterWrapLimit;
        public bool dispellable;
        public void DestroyMe(){
            Destroy (gameObject);
        }
        public Task tickTimer;
        public Task durationTimer;
        public TextMeshPro StacksText;
        private string ToolTipId;
        private ToolTipTriggerController ToolTipController;

        // Use this for initialization
        private void Start () {
    
            this.transform.localScale = new Vector3(1.8f,1.8f,1.8f);
    
            boxcolor = GetComponent<Image>();
    
            if( buff ){
                boxcolor.color = new Color32(185, 233, 0, 255);
                StacksText.color = new Color32(0, 63, 236, 255);
            } else {
                boxcolor.color = Color.red;
                StacksText.color = Color.white;
            }

            ToolTipController = gameObject.GetComponent<ToolTipTriggerController>();
            ToolTipId = ToolTipController.AddtoolTip(
                $"{statusModel.singleStatus.name}", 
                $"{statusModel.singleStatus.displayName}", 
                statusModel.singleStatus.statusDesc);

        }
    }
}

