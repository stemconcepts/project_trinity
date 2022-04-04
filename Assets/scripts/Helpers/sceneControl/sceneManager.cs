using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class sceneManager : MonoBehaviour
	{
		public bool tankReady = false;
		public bool healerReady = false;
		public bool dpsReady = false;
		public List<GameObject> enemies;

		public void LoadInventory(bool additive)
        {
			SceneManager.LoadScene("Inventory", additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
		}

		public void LoadExploration(bool additive)
		{
			SceneManager.LoadScene("Exploration", additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
		}

		public void UnLoadInventory()
		{
			int n = SceneManager.sceneCount;
			if (n > 1)
			{
				SceneManager.UnloadSceneAsync("Inventory");
			}
		}

		public void LoadBattle(List<GameObject> enemies)
		{
			if (tankReady && healerReady && dpsReady)
			{
				this.enemies = enemies;
				SceneManager.LoadScene("battle", LoadSceneMode.Single);
			}
			else if (tankReady == false)
			{
				//print("Please equip a weapon and/or skill to the Guardian");
			}
			else if (healerReady == false)
			{
				//print("Please equip a weapon and/or skill to the Walker");
			}
			else if (dpsReady == false)
			{
				//print("Please equip a weapon and/or skill to the Stalker");
			}
		}
	}
}