using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace AssemblyCSharp
{
	public class slotBehaviour : MonoBehaviour
	{
		public bool currentSlot;
		public int currentSlotID;
		public int currentItemID;
		//itemDetailsControl detailsControlScript;
		hoverManager hoverControlScript;
		public Image imageScript;
		public Color inactiveColor;
		public Color origColor;
		public BoxCollider2D colliderScript;

		void OnMouseOver()
		{
			hoverControlScript.hoveredSlot = this.gameObject;
		}

		// Use this for initialization
		void Start()
		{
			colliderScript.enabled = false;
			inactiveColor = new Vector4(1, 1, 1, 1f);
			origColor = inactiveColor;
			this.transform.localScale = new Vector3(1f, 1f, 1f);
		}

		void Awake()
		{
			colliderScript = GetComponent<BoxCollider2D>();
			imageScript = GetComponent<Image>();
			//detailsControlScript = GameObject.FindGameObjectWithTag("Panel-item-details").GetComponent<itemDetailsControl>();
			hoverControlScript = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<hoverManager>();
		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}