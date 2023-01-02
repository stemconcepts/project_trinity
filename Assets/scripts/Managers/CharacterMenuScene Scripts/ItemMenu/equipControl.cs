using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace AssemblyCSharp
{
	public class equipControl : MonoBehaviour
	{
		hoverManager hoverControlScript;
		public GameObject equippedWeapon;
		public GameObject equippedBauble;
		public SkillModel equippedSkill;
		public Image qualityDisplay;
		public Image imageScript;
		public bool activeSlot;

		[Header("Quality Colours")]
		public Color epic;
        public Color rare;
		public Color common;

        void OnMouseOver()
		{
			hoverControlScript.hoveredEquipSlot = this.gameObject;
			activeSlot = true;
		}

		void OnMouseExit()
		{
			hoverControlScript.hoveredEquipSlot = null;
			activeSlot = false;
		}

		// Use this for initialization
		void Awake()
		{
			imageScript = GetComponent<Image>();
			hoverControlScript = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<hoverManager>();
			epic = new Color(0.42f, 0.11f, 0.42f, 1);
			rare = new Color(0.11f, 0.17f, 0.42f, 1);
			common = new Color(0.24f, 0.31f, 0.26f, 1);
		}

		public void ClearItemQuality()
		{
			if (qualityDisplay)
			{
                qualityDisplay.gameObject.SetActive(false);
            }
        }

		/// <summary>
		/// Show item quality colour behind item;
		/// </summary>
		public void ShowItemQuality()
		{
            qualityDisplay.gameObject.SetActive(true);
			var itemQuality = equippedWeapon.GetComponent<itemBehaviour>().weaponItemScript.quality;

            switch (itemQuality)
			{
				case itemQuality.Common:
                    qualityDisplay.color = common;
                    break;
                case itemQuality.Rare:
                    qualityDisplay.color = rare;
                    break;
                case itemQuality.Epic:
                    qualityDisplay.color = epic;
                    break;
			}
		}

        public void ShowSkillQuality()
        {
            qualityDisplay.gameObject.SetActive(true);
            var skillQuality = equippedSkill.quality;

            switch (skillQuality)
            {
                case itemQuality.Common:
                    qualityDisplay.color = common;
                    break;
                case itemQuality.Rare:
                    qualityDisplay.color = rare;
                    break;
                case itemQuality.Epic:
                    qualityDisplay.color = epic;
                    break;
            }
        }

        // Update is called once per frame
        void Update()
		{

		}
	}
}