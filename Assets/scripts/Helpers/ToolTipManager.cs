using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class ToolTipManager : MonoBehaviour
    {
        public static GameObject mapHolder;
        public static GameObject canvasTooltip;

        // Use this for initialization
        void Start()
        {
            LoadUICanvas();
        }

        void LoadUICanvas()
        {
            mapHolder = GameObject.Find("MapHolder");
            canvasTooltip = GameObject.Find("Canvas - Tooltip"); //Explore_Manager.assetFinder.GetGameObjectFromPath("Assets/prefabs/helpers/Canvas - Tooltip.prefab");
            canvasTooltip.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {

        }


    }
}