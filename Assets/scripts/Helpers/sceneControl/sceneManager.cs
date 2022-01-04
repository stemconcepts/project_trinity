using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace AssemblyCSharp
{
	public class sceneManager : MonoBehaviour
	{
		//public GameObject menuManager;
		//equipmentManager equipManagerScript;
		public bool tankReady = false;
		public bool healerReady = false;
		public bool dpsReady = false;

		public void LoadInventory()
        {
			SceneManager.LoadScene("Inventory", LoadSceneMode.Additive);
		}

		public void UnLoadInventory()
		{
			int n = SceneManager.sceneCount;
			if (n > 1)
			{
				SceneManager.UnloadSceneAsync("Inventory");
			}
		}

		public void LoadBattle()
		{
			if (tankReady && healerReady && dpsReady)
			{
				SceneManager.LoadScene("battle", LoadSceneMode.Single);
			}
			else if (tankReady == false)
			{
				print("Please equip a weapon and/or skill to the Guardian");
			}
			else if (healerReady == false)
			{
				print("Please equip a weapon and/or skill to the Walker");
			}
			else if (dpsReady == false)
			{
				print("Please equip a weapon and/or skill to the Stalker");
			}
		}

		// Use this for initialization
		void Awake()
		{
			//equipManagerScript = GetComponent<equipmentManager>();
		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}