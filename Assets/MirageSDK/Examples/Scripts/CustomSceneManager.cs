using UnityEngine;
using UnityEngine.SceneManagement;

namespace MirageSDK.Examples
{
	public class CustomSceneManager : MonoBehaviour
	{
		public void NextScene()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}

		public void PrevScene()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
		}

		public void LoadScene(string sceneName)
		{
			SceneManager.LoadScene(sceneName);
		}
	}
}