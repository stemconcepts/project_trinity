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
			SceneManager.sceneUnloaded += OnSceneUnloaded;
		}

		private void OnSceneUnloaded(Scene current)
		{
            if (SceneManager.GetActiveScene().name == "exploration")
            {
                ExploreManager.explorerCamera.gameObject.SetActive(true);
				MainGameManager.instance.SaveScene("exploration");
			}
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			MainGameManager.instance.GetCanvasAndMainCamera();
            if (mode == LoadSceneMode.Additive)
            {
                ExploreManager.explorerCamera.gameObject.SetActive(false);
				MainGameManager.instance.SaveScene(scene.name);
			}
			Debug.Log(mode);
        }

        public void LoadInventory(bool additive)
        {
            SceneManager.LoadScene("Inventory", additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            MainGameManager.instance.SaveScene("Inventory");
        }

		public void LoadCrafting()
        {
			MainGameManager.instance.GetActiveBoxColliders().ForEach(o =>
			{
				o.enabled = false;
			});
			SceneManager.LoadScene("craftingOverlay", LoadSceneMode.Additive);
		}

        public void LoadExploration(bool additive)
        {
            SceneManager.LoadScene("Exploration", additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            MainGameManager.instance.SaveScene("Exploration");
        }

		public void LoadScene(string sceneName, bool additive)
        {
			SceneManager.LoadScene(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
			MainGameManager.instance.SaveScene(sceneName);
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