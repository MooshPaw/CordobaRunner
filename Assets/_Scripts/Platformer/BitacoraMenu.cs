using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Para manipular imágenes

public class BitacoraMenu : MonoBehaviour
{
    [Header("Interfaz")]
    public GameObject panelBitacoraGeneral;
    [Tooltip("El objeto vacío que tendrá el Grid Layout Group")]
    public Transform contenedorGrid; 
    [Tooltip("El 'molde' de la foto que crearemos como Prefab")]
    public GameObject fotoPrefab;    

    [Header("Base de Datos")]
    [Tooltip("Arrastra aquí TODOS tus archivos PhotoEntryData")]
    public List<PhotoEntryData> todasLasFotos; 

    public void AbrirCerrarBitacora()
    {
        if (panelBitacoraGeneral != null)
        {
            bool estadoActual = panelBitacoraGeneral.activeSelf;
            panelBitacoraGeneral.SetActive(!estadoActual); // Prende o apaga

            // Si se acaba de abrir, actualiza las fotos
            if (!estadoActual) 
            {
                ActualizarGaleria();
            }
        }
    }

    void ActualizarGaleria()
    {
        // 1. Limpiamos la cuadrícula borrando las fotos anteriores para no duplicarlas
        foreach (Transform hijo in contenedorGrid)
        {
            Destroy(hijo.gameObject);
        }

        // 2. Revisamos nuestra base de datos una por una
        foreach (PhotoEntryData dato in todasLasFotos)
        {
            // 3. Si el jugador ya descubrió este elemento...
            if (dato.isDiscovered)
            {
                // Creamos una copia del "molde" dentro de la cuadrícula
                GameObject nuevaFoto = Instantiate(fotoPrefab, contenedorGrid);
                nuevaFoto.transform.localScale = Vector3.one;
                
                // Le ponemos la imagen correspondiente (ej. la foto del pájaro)
                Image imagenUI = nuevaFoto.GetComponent<Image>();
                if (imagenUI != null)
                {
                    imagenUI.sprite = dato.photo;
                }
            }
        }
    }
}