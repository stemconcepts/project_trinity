using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class ToolTipItem
    {
        public string id;
        public string toolTipName;
        public string toolTipDesc;
        public int characterWrapLimit;
        public GameObject liveHoverObj;
    }

    public class ToolTipTriggerController : MonoBehaviour
    {
        public List<ToolTipItem> toolTipList = new List<ToolTipItem>();
        //public string toolTipName;
       // public string toolTipDesc;
        //private GameObject liveHoverObj;
        public GameObject hoverObj;
        LayoutElement layoutElement;
        //public int characterWrapLimit;

        public void OnMouseEnter()
        {
            if (this.isActiveAndEnabled)
            {
                GenerateToolTips();
            }
        }

        public void ClearToolTips()
        {
            toolTipList = new List<ToolTipItem>();
        }

        public void AddtoolTip(string id, string title, string desc)
        {
            var t = new ToolTipItem()
            {
                id = id,
                toolTipName = title,
                toolTipDesc = desc
            };
            toolTipList.Add(t);
        }

        void GenerateToolTips()
        {
            toolTipList.ForEach(t =>
            {
                t.liveHoverObj = (GameObject)Instantiate(hoverObj, MainGameManager.instance.tooltipManager.canvasTooltip.transform);
                layoutElement = t.liveHoverObj.GetComponent<LayoutElement>();
                t.liveHoverObj.transform.localScale = new Vector3(1f, 1f, 1f);
                var Name = t.liveHoverObj.transform.Find("Name").GetComponent<Text>();
                var Desc = t.liveHoverObj.transform.Find("Desc").GetComponent<Text>();
                Name.text = t.toolTipName;
                Desc.text = t.toolTipDesc;
                if (Desc.text.Length == 0)
                {
                    Desc.gameObject.SetActive(false);
                }
                layoutElement.enabled = (t.toolTipName.Length > t.characterWrapLimit || t.toolTipDesc.Length > t.characterWrapLimit) ? true : false;
            });
        }

        public void DestroyToolTipDisplay(string id)
        {
            if (toolTipList.Any(o => o.id.ToLower() == id.ToLower()))
            {
                Destroy(toolTipList.First(o => o.id.ToLower() == id.ToLower()).liveHoverObj);
                //toolTipList.Remove(toolTipList.Where(o => o.id.ToLower() == id.ToLower()).FirstOrDefault());
            }
        }

        public void DestroyAllToolTips()
        {
            toolTipList.ForEach(t =>
            {
                //Debug.Log($"Destroying tooltip {t.id}");
                Destroy(t.liveHoverObj);
            });
        }

        public void OnMouseExit()
        {
            DestroyAllToolTips();
        }

        void OnMouseDown()
        {
            DestroyAllToolTips();
        }

        // Use this for initialization
        void Start()
        {
            hoverObj = MainGameManager.instance.exploreManager.assetFinder.GetGameObjectFromPath("Assets/prefabs/helpers/tooltipHoverObject.prefab");
            hoverObj.GetComponent<ToolTipBehaviour>().canvas = MainGameManager.instance.tooltipManager.canvasTooltip.GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}