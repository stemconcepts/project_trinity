﻿using UnityEngine;
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
		public Image imageScript;
		public bool activeSlot;

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
		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}