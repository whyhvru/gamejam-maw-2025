using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private readonly int _gameSceneIndex = 2;

    public void OnPlayPressed()
    {
        SceneManager.LoadScene(_gameSceneIndex);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }
}