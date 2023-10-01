using UnityEngine;
using System.Collections;
using System.Linq;

namespace AssemblyCSharp
{
    public class ToolTipManager : MonoBehaviour
    {
        //public GameObject mapHolder;
        public GameObject canvasTooltip;

        // Use this for initialization
        void Start()
        {
            LoadUICanvas();
        }

        void LoadUICanvas()
        {
            //mapHolder = GameObject.Find("MapHolder");
            //canvasTooltip = GameObject.Find("Canvas - Tooltip"); //Explore_Manager.assetFinder.GetGameObjectFromPath("Assets/prefabs/helpers/Canvas - Tooltip.prefab");
            //canvasTooltip.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void DestroyAllToolTips()
        {
            canvasTooltip.transform.GetComponentsInChildren<ToolTipBehaviour>().ToList().ForEach(tooltip =>
            {
                Destroy(tooltip.gameObject);
            });
        }

    }
}