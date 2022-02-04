using UnityEngine;
using UnityEngine.SceneManagement;
using WalletConnectSharp.Unity;

namespace MirageSDK.Examples.Scripts
{
    public class CustomSceneManager : MonoBehaviour
    {
        public void SaveAccount()
        {
            // save account
            var account = WalletConnect.ActiveSession.Accounts[0];
            PlayerPrefs.SetString("Account", account);
        }
    
        public void NextScene()
        {
            // move to next scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    
        public void PrevScene()
        {
            // move to previous scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }

        public void LoadScene(string sceneName) {
            // move to previous scene
            SceneManager.LoadScene(sceneName);
        }
    }
}
