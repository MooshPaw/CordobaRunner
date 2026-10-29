using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalJuego : MonoBehaviour
{
    public GameObject panelVictoria;

    // Esta función se activa cuando el jugador entra al cubo invisible
    private void OnTriggerEnter(Collider other)
    {
        // Asegúrate de que tu personaje tenga el Tag "Player"
        if (other.CompareTag("Player"))
        {
            MostrarVictoria();
        }
    }

    public void MostrarVictoria()
    {
        panelVictoria.SetActive(true);
        Time.timeScale = 0f; // Congela el juego para que los enemigos no te ataquen de fondo
    }

    public void IrMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("NombreDeTuEscenaMenu");
    }

    public void SalirDelJuego()
    {
        // Nota: Application.Quit no cierra el juego dentro del editor de Unity, 
        // pero sí lo cerrará cuando lo exportes a tu celular Android.
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}