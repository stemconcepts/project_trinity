using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.Managers;
using Assets.scripts.Managers.ExplorerScene_Scripts;

namespace AssemblyCSharp
{
	public class sceneManager : MonoBehaviour
	{
		public bool tankReady = false;
		public bool healerReady = false;
		public bool dpsReady = false;
		public List<GameObject> enemies;
		public List<ExplorerStatus> playerStatuses = new List<ExplorerStatus>();
        public List<ExplorerStatus> enemyStatuses = new List<ExplorerStatus>();
        public string currentScene;
		public Animator battleTransition;

        void OnEnable()
        {
			SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.sceneUnloaded += OnSceneUnloaded;
		}

		private void OnSceneUnloaded(Scene current)
		{
            if (SceneManager.GetActiveScene().name == "exploration")
            {
                MainGameManager.instance.soundManager.ChangeMainMusicTrack(MainGameManager.instance.TutorialExploreTrack);
                MainGameManager.instance.exploreManager.explorerCamera.gameObject.SetActive(true);
				MainGameManager.instance.SaveScene("exploration");
			}
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			MainGameManager.instance.tooltipManager.DestroyAllToolTips();
            MainGameManager.instance.GetCanvasAndMainCamera();
			if (SceneManager.GetActiveScene().name == "exploration")
			{
                MainGameManager.instance.exploreManager = GameObject.Find("ExplorerManager").GetComponent<ExploreManagerV2>();
            }
			if (mode == LoadSceneMode.Additive)
			{
				MainGameManager.instance.exploreManager?.explorerCamera.gameObject.SetActive(false);
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
            MainGameManager.instance.DisableEnableLiveBoxColliders(false);
            SceneManager.LoadScene("craftingOverlay", LoadSceneMode.Additive);
		}

        public void LoadExploration(bool additive)
        {
            MainGameManager.instance.gameEffectManager.TransitionToScene(battleTransition, 1f, () => {
                SceneManager.LoadScene("Exploration", additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
                MainGameManager.instance.soundManager.ChangeMainMusicTrack(MainGameManager.instance.TutorialExploreTrack);
                MainGameManager.instance.SaveScene("Exploration");
            });
        }

		public void LoadScene(string sceneName, bool additive)
        {
            MainGameManager.instance.gameEffectManager.TransitionToScene(battleTransition, 1f, () => {
                SceneManager.LoadScene(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
                //MainGameManager.instance.soundManager.ChangeMainMusicTrack(MainGameManager.instance.TutorialExploreTrack);
                MainGameManager.instance.SaveScene(sceneName);
            });
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

		public void LoadBattle(List<GameObject> enemies, StatusModel[] playerStatuses = null, StatusModel[] enemyStatuses = null)
		{
			if (MainGameManager.instance.SceneManager.TeamReady())
			{
				MainGameManager.instance.gameEffectManager.TransitionToScene(battleTransition, 1f, () =>
				{
                    MainGameManager.instance.soundManager.ChangeMainMusicTrack(MainGameManager.instance.TutorialCombatTrack);
                    MainGameManager.instance.SceneManager.enemies = enemies;
                    SceneManager.LoadScene("battle", LoadSceneMode.Additive);
                });
			}
		}
	}
}