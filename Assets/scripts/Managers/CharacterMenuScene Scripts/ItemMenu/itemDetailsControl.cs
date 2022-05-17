using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace AssemblyCSharp
{
	public class itemDetailsControl : MonoBehaviour
	{
		public GameObject menuManager;
		public GameObject panelItemDetail;
		public GameObject selectedItemObject;
		//item details
		public GameObject itemNameHolder;
		public GameObject itemPreviewHolder;
		public GameObject itemSkillHolder;
		public Sprite tempItemIcon;
		//weaponItems weaponItemsScript;

		//skill details
		public GameObject skillNameHolder;
		public GameObject skillPreviewHolder;
		//skillItems skillItemsScript;

		public void DisplayWeaponData(weaponModel weapon)
		{
			panelItemDetail.SetActive(true);
			if (weapon.itemIcon != null)
			{
				selectedItemObject.GetComponent<Image>().sprite = weapon.itemIcon;
			}
			else
			{
				selectedItemObject.GetComponent<Image>().sprite = tempItemIcon;
			}
			var itemName = weapon.DisplayName;
			var itemDetails = weapon.WeaponDescription;
			var attachedSkills = weapon;
			var nameText = itemNameHolder.transform.Find("Text").GetComponent<Text>();
			var previewText = itemPreviewHolder.transform.Find("Text").GetComponent<Text>();
			var skillsTitle2 = itemSkillHolder.transform.Find("Panel2").Find("skillTextHolder").Find("Text-title2").GetComponent<Text>();
			var skillsCost2 = itemSkillHolder.transform.Find("Panel2").Find("skillTextHolder").Find("CostandCD").Find("Text-cost").GetComponent<Text>();
			var skillsCd2 = itemSkillHolder.transform.Find("Panel2").Find("skillTextHolder").Find("CostandCD").Find("Text-cd").GetComponent<Text>();
			var skillsDetail2 = itemSkillHolder.transform.Find("Panel2").Find("skillTextHolder").Find("Text-detail2").GetComponent<Text>();
			var skillsDuration2 = itemSkillHolder.transform.Find("Panel2").Find("skillTextHolder").Find("CostandCD").Find("Text-duration").GetComponent<Text>();
			var skillIcon2 = itemSkillHolder.transform.Find("Panel2").Find("skillIcon").GetComponent<Image>();
			var skillsTitle3 = itemSkillHolder.transform.Find("Panel3").Find("skillTextHolder").Find("Text-title3").GetComponent<Text>();
			var skillsCost3 = itemSkillHolder.transform.Find("Panel3").Find("skillTextHolder").Find("CostandCD").Find("Text-cost").GetComponent<Text>();
			var skillsCd3 = itemSkillHolder.transform.Find("Panel3").Find("skillTextHolder").Find("CostandCD").Find("Text-cd").GetComponent<Text>();
			var skillsDetail3 = itemSkillHolder.transform.Find("Panel3").Find("skillTextHolder").Find("Text-detail3").GetComponent<Text>();
			var skillsDuration3 = itemSkillHolder.transform.Find("Panel3").Find("skillTextHolder").Find("CostandCD").Find("Text-duration").GetComponent<Text>();
			var skillIcon3 = itemSkillHolder.transform.Find("Panel3").Find("skillIcon").GetComponent<Image>();
			nameText.text = itemName;
			previewText.text = itemDetails;
			skillsTitle2.text = attachedSkills.skillTwo.skillName;
			skillsCost2.text = "Cost: <color=#ff7849>" + attachedSkills.skillTwo.skillCost.ToString() + "</color>";
			skillsDuration2.text = "Duration: <color=#ff7849>" + attachedSkills.skillTwo.turnDuration.ToString() + "</color>";
			skillsCd2.text = "Cooldown: <color=#ff7849>" + attachedSkills.skillTwo.skillCooldown.ToString() + "</color>";
			skillsDetail2.text = attachedSkills.skillTwo.skillDesc;
			skillIcon2.sprite = attachedSkills.skillTwo.skillIcon;
			skillsTitle3.text = attachedSkills.skillThree.skillName;
			skillsCost3.text = "Cost: <color=#ff7849>" + attachedSkills.skillThree.skillCost.ToString() + "</color>";
			skillsDuration3.text = "Duration: <color=#ff7849>" + attachedSkills.skillThree.turnDuration.ToString() + "</color>";
			skillsCd3.text = "Cooldown: <color=#ff7849>" + attachedSkills.skillThree.skillCooldown.ToString() + "</color>";
			skillsDetail3.text = attachedSkills.skillThree.skillDesc;
			skillIcon3.sprite = attachedSkills.skillThree.skillIcon;
		}

		public void DisplayBaubleData(bauble bauble)
		{
			panelItemDetail.SetActive(false);
			var itemName = bauble.baubleName;
			var itemDetails = bauble.baubleDesc;
			var attachedEffect = bauble;
			var nameText = itemNameHolder.transform.Find("Text").GetComponent<Text>();
			var previewText = itemPreviewHolder.transform.Find("Text").GetComponent<Text>();
			var skillsTitle2 = itemSkillHolder.transform.Find("Panel2").Find("skillTextHolder").Find("Text-title2").GetComponent<Text>();
			var skillsCost2 = itemSkillHolder.transform.Find("Panel2").Find("skillTextHolder").Find("CostandCD").Find("Text-cost").GetComponent<Text>();
			var skillsCd2 = itemSkillHolder.transform.Find("Panel2").Find("skillTextHolder").Find("CostandCD").Find("Text-cd").GetComponent<Text>();
			var skillsDetail2 = itemSkillHolder.transform.Find("Panel2").Find("skillTextHolder").Find("Text-detail2").GetComponent<Text>();
			var skillsDuration2 = itemSkillHolder.transform.Find("Panel2").Find("skillTextHolder").Find("CostandCD").Find("Text-duration").GetComponent<Text>();
			var skillIcon2 = itemSkillHolder.transform.Find("Panel2").Find("skillIcon").GetComponent<Image>();
			var skillsTitle3 = itemSkillHolder.transform.Find("Panel3").Find("skillTextHolder").Find("Text-title3").GetComponent<Text>();
			var skillsCost3 = itemSkillHolder.transform.Find("Panel3").Find("skillTextHolder").Find("CostandCD").Find("Text-cost").GetComponent<Text>();
			var skillsCd3 = itemSkillHolder.transform.Find("Panel3").Find("skillTextHolder").Find("CostandCD").Find("Text-cd").GetComponent<Text>();
			var skillsDetail3 = itemSkillHolder.transform.Find("Panel3").Find("skillTextHolder").Find("Text-detail3").GetComponent<Text>();
			var skillsDuration3 = itemSkillHolder.transform.Find("Panel3").Find("skillTextHolder").Find("CostandCD").Find("Text-duration").GetComponent<Text>();
			var skillIcon3 = itemSkillHolder.transform.Find("Panel3").Find("skillIcon").GetComponent<Image>();
			nameText.text = itemName;
			previewText.text = itemDetails;
			if (bauble.itemIcon != null)
			{
				selectedItemObject.GetComponent<Image>().sprite = bauble.itemIcon;
			}
			else
			{
				selectedItemObject.GetComponent<Image>().sprite = null;
			}
			//skillsTitle.text = attachedSkills.skillOne.displayName;
			//skillsCost.text = attachedSkills.skillOne.skillCost.ToString();
			//skillsDetail.text = attachedSkills.skillOne.skillDetails;
			/*skillsTitle2.text = attachedEffect.effectsOnEvent;
			skillsCost2.text = "Cost: <color=#ff7849>" + attachedSkills.skillTwo.skillCost.ToString() + "</color>";
			skillsDuration2.text = "Duration: <color=#ff7849>" + attachedSkills.skillTwo.duration.ToString() + "</color>";
			skillsCd2.text = "Cooldown: <color=#ff7849>" + attachedSkills.skillTwo.skillCooldown.ToString() + "</color>";
			skillsDetail2.text = attachedSkills.skillTwo.skillDesc;
			skillIcon2.sprite = attachedSkills.skillTwo.skillIcon;
			skillsTitle3.text = attachedSkills.skillThree.displayName;
			skillsCost3.text = "Cost: <color=#ff7849>" + attachedSkills.skillThree.skillCost.ToString() + "</color>";
			skillsDuration3.text = "Duration: <color=#ff7849>" + attachedSkills.skillThree.duration.ToString() + "</color>";
			skillsCd3.text = "Cooldown: <color=#ff7849>" + attachedSkills.skillThree.skillCooldown.ToString() + "</color>";
			skillsDetail3.text = attachedSkills.skillThree.skillDesc;
			skillIcon3.sprite = attachedSkills.skillThree.skillIcon;*/
		}

		public void DisplaySkillData(SkillModel classSkill)
		{
			var nameText = itemNameHolder.transform.Find("Text-skill name").GetComponent<Text>();
			var previewText = itemPreviewHolder.transform.Find("Panel-skill preview").Find("Text-skill detail").GetComponent<Text>();
			var skillsCost = itemSkillHolder.transform.Find("Panel-skill preview").Find("Text-skill cost").GetComponent<Text>();
			nameText.text = classSkill.skillName;
			previewText.text = classSkill.skillDesc;
			skillsCost.text = "Cost: <color=#ff7849>" + classSkill.skillCost + "</color>";
		}

		// Use this for initialization
		void Awake()
		{
			//skillItemsScript = menuManager.GetComponent<skillItems>();
		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}