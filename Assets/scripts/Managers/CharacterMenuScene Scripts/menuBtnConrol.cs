using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace AssemblyCSharp
{
	public class menuBtnConrol : MonoBehaviour
	{
		public GameObject itemMenu;
		public GameObject skillMenu;
		public GameObject menuTitle;

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
			itemMenu.SetActive(true);
			menuTitle.transform.Find("Title").GetComponent<Text>().text = "EQUIPMENT";
		}

		public void ShowSkillMenu()
		{
			itemMenu.SetActive(false);
			skillMenu.SetActive(true);
			menuTitle.transform.Find("Title").GetComponent<Text>().text = "SKILLS";
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
