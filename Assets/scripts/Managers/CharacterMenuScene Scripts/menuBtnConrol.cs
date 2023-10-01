using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ModularMotion;
using DG.Tweening;
using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class menuBtnConrol : MonoBehaviour
	{
		public GameObject itemMenu;
		public GameObject skillMenu;
        public GameObject eyeSkillMenu;
        public GameObject formationMenu;
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
                skillMenu.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
                skillMenu.GetComponent<UIMotion>().PlayAllBackward();
                skillMenu.SetActive(false);
                skillDesc.SetActive(false);
            }
            if (eyeSkillMenu.activeSelf)
            {
                eyeSkillMenu.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
                eyeSkillMenu.GetComponent<UIMotion>().PlayAllBackward();
                eyeSkillMenu.SetActive(false);
                skillDesc.SetActive(false);
            }
            if (formationMenu.activeSelf)
            {
                formationMenu.SetActive(false);
                formationMenu.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
                formationMenu.GetComponent<UIMotion>().Play();
            }
            if (!itemMenu.activeSelf)
            {
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
				itemMenu.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
                itemMenu.GetComponent<UIMotion>().PlayAllBackward();
                itemMenu.SetActive(false);
                itemDesc.SetActive(false);
            }
            if (eyeSkillMenu.activeSelf)
            {
                eyeSkillMenu.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
                eyeSkillMenu.GetComponent<UIMotion>().PlayAllBackward();
                eyeSkillMenu.SetActive(false);
            }
            if (formationMenu.activeSelf)
            {
                formationMenu.SetActive(false);
                formationMenu.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
                formationMenu.GetComponent<UIMotion>().Play();
            }
            if (!skillMenu.activeSelf)
            {
                skillMenu.SetActive(true);
                skillDesc.SetActive(true);
                skillMenu.transform.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                skillMenu.GetComponent<UIMotion>().Play();
            }
        }

        public void ShowEyeSkillMenu()
        {
            if (itemMenu.activeSelf)
            {
                itemDesc.SetActive(false);
                itemMenu.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
                itemMenu.GetComponent<UIMotion>().PlayAllBackward();
                itemMenu.SetActive(false);
            }
            if (skillMenu.activeSelf)
            {
                skillDesc.SetActive(false);
                skillMenu.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
                skillMenu.GetComponent<UIMotion>().Play();
                skillMenu.SetActive(false);
            }
            if (formationMenu.activeSelf)
            {
                formationMenu.SetActive(false);
                formationMenu.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
                formationMenu.GetComponent<UIMotion>().Play();
            }
            if (!eyeSkillMenu.activeSelf)
            {
                eyeSkillMenu.SetActive(true);
                skillDesc.SetActive(true);
                eyeSkillMenu.transform.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                eyeSkillMenu.GetComponent<UIMotion>().Play();
            }
        }

        public void ShowFormationMenu()
        {
            if (itemMenu.activeSelf)
            {
                itemDesc.SetActive(false);
                itemMenu.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
                itemMenu.GetComponent<UIMotion>().PlayAllBackward();
                itemMenu.SetActive(false);
            }
            if (skillMenu.activeSelf)
            {
                skillDesc.SetActive(false);
                skillMenu.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
                skillMenu.GetComponent<UIMotion>().Play();
                skillMenu.SetActive(false);
            }
            if (eyeSkillMenu.activeSelf)
            {
                eyeSkillMenu.SetActive(false);
                skillDesc.SetActive(false);
                eyeSkillMenu.transform.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                eyeSkillMenu.GetComponent<UIMotion>().Play();
            }
            if (!formationMenu.activeSelf)
            {
                formationMenu.SetActive(true);
                formationMenu.transform.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                formationMenu.GetComponent<UIMotion>().Play();
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
