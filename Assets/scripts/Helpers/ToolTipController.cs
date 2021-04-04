using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AssemblyCSharp
{
    public class ToolTipController : MonoBehaviour
    {
        public string toolTipName;
        public string toolTipDesc;
        private GameObject liveHoverObj;
        public GameObject hoverObj;
        public LayoutElement layoutElement;
        public int characterWrapLimit;

        public void OnMouseEnter()
        {
            liveHoverObj = (GameObject)Instantiate(hoverObj);
            layoutElement = liveHoverObj.GetComponent<LayoutElement>();
            liveHoverObj.transform.SetParent(Battle_Manager.tooltipCanvas.transform);
            liveHoverObj.transform.localScale = new Vector3(1f, 1f, 1f);
            var statusName = liveHoverObj.transform.Find("statusName").GetComponent<Text>();
            var statusDesc = liveHoverObj.transform.Find("statusDesc").GetComponent<Text>();
            statusName.text = toolTipName;
            statusDesc.text = toolTipDesc;
            int headerLength = toolTipName.Length;
            int contentLength = toolTipDesc.Length;
            layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
        }

        public void OnMouseExit()
        {
            Destroy(liveHoverObj);
        }

        void Awake()
        {
            //hoverObj = Battle_Manager.assetFinder.GetGameObjectFromPath("Assets/prefabs/status/status_HoverDetails_Obj.prefab");
        }

        // Start is called before the first frame update
        void Start()
        {

        }
    }
}