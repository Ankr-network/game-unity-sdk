using UnityEngine;
using UnityEngine.SceneManagement;
using WalletConnectSharp.Unity;

public class CustomSceneManager : MonoBehaviour
{
    public void NextScene()
    {
        // save account
        string Account = WalletConnect.ActiveSession.Accounts[0];
        PlayerPrefs.SetString("Account", Account);
        // move to next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
