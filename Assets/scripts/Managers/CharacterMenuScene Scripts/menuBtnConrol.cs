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
			Explore_Manager.gameManager.SceneManager.LoadInventory();
		}

		public void HideInventory()
		{
			Explore_Manager.gameManager.SceneManager.UnLoadInventory();
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

		public void Start()
		{
		}
	}
}
