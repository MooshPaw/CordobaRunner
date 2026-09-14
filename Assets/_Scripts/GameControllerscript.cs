using UnityEngine;
using UnityEngine.SceneManagement;

public class StarterController : MonoBehaviour
{
    public string mainMenuScene = "MainMenu";
    public float waitTime = 2f;

    private void Start()
    {
        Invoke(nameof(GoToMainMenu), waitTime);
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}