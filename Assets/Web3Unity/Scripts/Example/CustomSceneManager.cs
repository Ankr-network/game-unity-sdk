using UnityEngine;
using UnityEngine.SceneManagement;
using WalletConnectSharp.Unity;

public class CustomSceneManager : MonoBehaviour
{
    public void SaveAccount()
    {
        // save account
        string Account = WalletConnect.ActiveSession.Accounts[0];
        PlayerPrefs.SetString("Account", Account);
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
