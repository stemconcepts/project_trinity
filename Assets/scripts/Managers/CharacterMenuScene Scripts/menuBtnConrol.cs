using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace AssemblyCSharp
{
	public class menuBtnConrol : MonoBehaviour
	{
		public GameObject itemMenu;
		public GameObject skillMenu;
		//public GameObject menuTitle;
		public GameObject skillDesc;
        public GameObject itemDesc;

        public void ShowInventory()
		{
			MainGameManager.instance.SceneManager.LoadInventory(true);
		}

		public void HideInventory()
		{
			MainGameManager.instance.SceneManager.UnLoadScene("Inventory");
		}

		public void ShowItemMenu()
		{
			skillMenu.SetActive(false);
            skillDesc.SetActive(false);
            itemMenu.SetActive(true);
            itemDesc.SetActive(true);
            //menuTitle.transform.Find("Title").GetComponent<Text>().text = "EQUIPMENT";
        }

		public void ShowSkillMenu()
		{
			itemMenu.SetActive(false);
            itemDesc.SetActive(false);
            skillMenu.SetActive(true);
            skillDesc.SetActive(true);
            //menuTitle.transform.Find("Title").GetComponent<Text>().text = "SKILLS";
        }

		public void LoadExploration()
		{
            if (MainGameManager.instance.SceneManager.TeamReady()){
				MainGameManager.instance.SceneManager.LoadExploration(false);
			}
		}

		public void Start()
		{
		}
	}
}
