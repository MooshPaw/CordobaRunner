using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderScript : MonoBehaviour
{
    private bool _isPaused;
    [SerializeField] private GameObject _pauseUI;
    public void ChangeScene(int SceneSelect)
    {
        SceneManager.LoadScene(SceneSelect);
    }
    
    public void Pause()
    {
        // Si no esta pausado, pausar
        if (!_isPaused)
        {
            _isPaused = true;
            _pauseUI.SetActive(true);
            //_backgroundMusic.volume = 0.1f;
            Time.timeScale = 0f;
        }

        // Si esta pausado, despausar
        else if (_isPaused)
        {
            _isPaused = false;
            _pauseUI.SetActive(false);
            //_backgroundMusic.volume = 1f;
            Time.timeScale = 1f;
        }
    }

    // Sale de la aplicacion
    public void Quit()
    {
        Debug.Log("Application Exit");
        Application.Quit();
    }
}
