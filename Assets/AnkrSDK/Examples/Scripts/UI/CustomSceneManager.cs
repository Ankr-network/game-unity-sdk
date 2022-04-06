using UnityEngine;
using UnityEngine.SceneManagement;

namespace AnkrSDK.UI
{
	public class CustomSceneManager : MonoBehaviour
	{
		public void LoadScene(string sceneName)
		{
			SceneManager.LoadScene(sceneName);
		}
	}
}