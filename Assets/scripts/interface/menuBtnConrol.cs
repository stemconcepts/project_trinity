using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class menuBtnConrol : MonoBehaviour {
	public GameObject itemMenu;
	public GameObject skillMenu;
	public GameObject menuTitle;

	public void ShowItemMenu(){
		skillMenu.SetActive(false);
		itemMenu.SetActive(true);
		menuTitle.transform.Find("Title").GetComponent<Text>().text = "EQUIPMENT";
	}

	public void ShowSkillMenu(){
		itemMenu.SetActive(false);
		skillMenu.SetActive(true);
		menuTitle.transform.Find("Title").GetComponent<Text>().text = "SKILLS";
	}

	public void Start(){
	}
}
