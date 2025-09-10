using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void OnPlayButton()
    {
        SceneManager.LoadScene((int)Scenes.Level1);
    }

    public void OnQuitButton()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif

#if !UNITY_WEBGL
        Application.Quit();
#endif
    }
}
