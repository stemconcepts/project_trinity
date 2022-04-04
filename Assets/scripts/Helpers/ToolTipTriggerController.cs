using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AssemblyCSharp
{
    public class ToolTipTriggerController : MonoBehaviour
    {
        public string toolTipName;
        public string toolTipDesc;
        private GameObject liveHoverObj;
        public GameObject hoverObj;
        LayoutElement layoutElement;
        public int characterWrapLimit;

        public void OnMouseEnter()
        {
            if (this.isActiveAndEnabled)
            {
                liveHoverObj = (GameObject)Instantiate(hoverObj, ToolTipManager.canvasTooltip.transform);
                layoutElement = liveHoverObj.GetComponent<LayoutElement>();
                liveHoverObj.transform.localScale = new Vector3(1f, 1f, 1f);
                var Name = liveHoverObj.transform.Find("Name").GetComponent<Text>();
                var Desc = liveHoverObj.transform.Find("Desc").GetComponent<Text>();
                Name.text = toolTipName;
                Desc.text = toolTipDesc;
                int headerLength = toolTipName.Length;
                int contentLength = toolTipDesc.Length;
                layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
            }
        }

        public void DestroyToolTipDisplay()
        {
            Destroy(liveHoverObj);
        }

        public void OnMouseExit()
        {
            Destroy(liveHoverObj);
        }

        // Use this for initialization
        void Start()
        {
            hoverObj = ExploreManager.assetFinder.GetGameObjectFromPath("Assets/prefabs/helpers/tooltipHoverObject.prefab");
            hoverObj.GetComponent<ToolTipBehaviour>().canvas = ToolTipManager.canvasTooltip.GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}