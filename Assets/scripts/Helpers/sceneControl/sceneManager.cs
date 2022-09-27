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
			} else
			{
                var cam = (Camera)FindObjectOfType(typeof(Camera));
                MainGameManager.instance.SetCurrentCamera(cam);
            }
        }

		public void LoadInventory(bool additive)
        {
            SceneManager.LoadScene("Inventory", additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
			if (!additive)
			{
				MainGameManager.instance.soundManager.ChangeMainMusicTrack(MainGameManager.instance.TutorialInventoryTrack);
			}
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
            MainGameManager.instance.soundManager.ChangeMainMusicTrack(MainGameManager.instance.TutorialExploreTrack);
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
				MainGameManager.instance.SceneManager.enemies = enemies;
				SceneManager.LoadScene("battle", LoadSceneMode.Additive);
			}
		}
	}
}