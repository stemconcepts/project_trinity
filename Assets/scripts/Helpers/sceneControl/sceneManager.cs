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
		public string currentScene;

		void OnEnable()
        {
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			MainGameManager.instance.GetCanvasAndMainCamera();
			Debug.Log("OnSceneLoaded: " + scene.name);
			Debug.Log(mode);
		}

		public void LoadInventory(bool additive)
        {
			SceneManager.LoadScene("Inventory", additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
			MainGameManager.instance.SaveScene("Inventory");
		}

		public void LoadExploration(bool additive)
		{
			SceneManager.LoadScene("Exploration", additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
			MainGameManager.instance.SaveScene("Exploration");
		}

		public void UnLoadScene(string sceneName)
		{
			int n = SceneManager.sceneCount;
			if (n > 1)
			{
				SceneManager.UnloadSceneAsync(sceneName);
			}
		}

		public void UnLoadInventory()
		{
			int n = SceneManager.sceneCount;
			if (n > 1)
			{
				SceneManager.UnloadSceneAsync("Inventory");
			}
		}

		public bool TeamReady()
        {
			return tankReady && healerReady && dpsReady;
		}

		public void LoadBattle(List<GameObject> enemies)
		{
			if (MainGameManager.instance.SceneManager.TeamReady())
			{
				//this.enemies = enemies;
				MainGameManager.instance.SceneManager.enemies = enemies;
                SceneManager.LoadScene("battle", LoadSceneMode.Additive);
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