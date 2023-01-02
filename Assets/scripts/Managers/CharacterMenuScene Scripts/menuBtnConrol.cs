using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ModularMotion;
using DG.Tweening;

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
			if (skillMenu.activeSelf)
			{
                skillMenu.SetActive(false);
                skillDesc.SetActive(false);
                skillMenu.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
                skillMenu.GetComponent<UIMotion>().PlayAllBackward();
                itemMenu.SetActive(true);
                itemDesc.SetActive(true);
                itemMenu.transform.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                itemMenu.GetComponent<UIMotion>().Play();
            }
        }

		public void ShowSkillMenu()
		{
			if (itemMenu.activeSelf)
			{
				itemMenu.SetActive(false);
				itemDesc.SetActive(false);
				itemMenu.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
				itemMenu.GetComponent<UIMotion>().PlayAllBackward();
				skillMenu.SetActive(true);
				skillDesc.SetActive(true);
				skillMenu.transform.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
				skillMenu.GetComponent<UIMotion>().Play();
			}
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
