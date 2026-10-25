using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private void Awake()
    {
        instance = this;

        currentCrystals = PlayerPrefs.GetInt("Crystals");
        currentCoins = PlayerPrefs.GetInt("Coins");

    }

    public float waitBeforeRespawning;

    [HideInInspector]
    public bool respawning;

    private PlayerController player;

    [HideInInspector]
    public Vector3 respawnPoint;

    private CameraController cam;

    [HideInInspector]
    public float levelTimer;

    public int currentCoins, coinThreshold = 100, currentCrystals;

    public float waitToEndLevel = 1f;

    [HideInInspector]
    public bool levelComplete;

    public bool reloadOnRespawn;

    // Start is called before the first frame update
    void Start()
    {


        player = FindObjectOfType<PlayerController>();
        respawnPoint = player.transform.position;


        cam = FindObjectOfType<CameraController>();



        UIController.instance.SetCoinText(currentCoins.ToString());
        UIController.instance.SetCrystalText(currentCrystals.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (!levelComplete)
        {
            levelTimer += Time.deltaTime;
            UIController.instance.SetTimeText(levelTimer.ToString("0"));
        } else
        {
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.Euler(0f, 180f, 0f), 10f * Time.deltaTime);
        }

#if UNITY_EDITOR
        if(Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame)
        {
            PlayerPrefs.DeleteAll();
        }
#endif
    }

    public void Respawn()
    {
        if(!respawning)
        {
            respawning = true;

            StartCoroutine(RespawnCo());
        }
    }

    public IEnumerator RespawnCo()
    {
        Debug.Log($"[LevelManager] RespawnCo starting. respawnPoint={respawnPoint} reloadOnRespawn={reloadOnRespawn}");

        player.gameObject.SetActive(false);
        UIController.instance.FadeToBlack();

        AudioManager.instance.PlaySFX(10);

        yield return new WaitForSeconds(waitBeforeRespawning);

        if (reloadOnRespawn)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {

            Debug.Log($"[LevelManager] Repositioning player to respawnPoint={respawnPoint}");

            player.transform.position = respawnPoint;

            Debug.Log($"[LevelManager] Player position after set: {player.transform.position}");

            cam.SnapToTarget();

            player.gameObject.SetActive(true);

            respawning = false;

            UIController.instance.FadeFromBlack();

            PlayerHealthController.instance.FillHealth();
        }
    }

    public void GetCoin()
    {
        currentCoins++;

        if(currentCoins >= coinThreshold)
        {
            GetCrystal();

            currentCoins -= coinThreshold;
        }

        UIController.instance.SetCoinText(currentCoins.ToString());

        PlayerPrefs.SetInt("Coins", currentCoins);
    }

    public void GetCrystal()
    {
        currentCrystals++;

        UIController.instance.SetCrystalText(currentCrystals.ToString());

        PlayerPrefs.SetInt("Crystals", currentCrystals);
    }

    // Only marks the level as complete and kicks the player's end-of-level animation.
    // The fade-out and scene load are owned by LevelExit/FadeScreen instead, gated on
    // the Fade Out animation's completion rather than a fixed timer.
    public void EndLevel()
    {
        levelComplete = true;

        if (HasParameter(player.anim, "endLevel"))
        {
            player.anim.SetTrigger("endLevel");
        }
    }

    // Evita el error "Parameter 'x' does not exist" si el Animator todavia
    // no tiene ese trigger agregado (ej. antes de que exista la animacion
    // de fin de nivel).
    private bool HasParameter(Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }

        return false;
    }
}
