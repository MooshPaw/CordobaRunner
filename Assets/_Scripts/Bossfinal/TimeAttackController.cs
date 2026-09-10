using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// El "boss final" no es una pelea: es una carrera contra el tiempo hasta el
// punto donde asoma el periquito, esquivando ProjectileHazard que se lanzan
// cada vez m\u00e1s seguido mientras menos tiempo queda.
public class TimeAttackController : MonoBehaviour
{
    public float maxTime;
    public Slider timeSlider;

    public Transform[] launchPoints;
    public ProjectileHazard hazardPrefab;
    public float timeBetweenLaunchesEarly;
    public float timeBetweenLaunchesMid;
    public float timeBetweenLaunchesLate;

    public GameObject levelExitOnSuccess;

    [HideInInspector]
    public bool isRunning;

    private float timeRemaining;
    private float launchCounter;

    private void Awake()
    {
        timeSlider.maxValue = maxTime;
    }

    public void StartSequence()
    {
        timeRemaining = maxTime;
        timeSlider.value = timeRemaining;
        isRunning = true;

        launchCounter = timeBetweenLaunchesEarly;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;
        timeSlider.value = timeRemaining;

        if (timeRemaining <= 0f)
        {
            FailSequence();
            return;
        }

        launchCounter -= Time.deltaTime;
        if (launchCounter <= 0f)
        {
            LaunchHazard();
            ResetLaunchCounter();
        }
    }

    private void LaunchHazard()
    {
        if (launchPoints.Length == 0) return;

        Transform point = launchPoints[Random.Range(0, launchPoints.Length)];
        Instantiate(hazardPrefab, point.position, point.rotation);
    }

    private void ResetLaunchCounter()
    {
        float remainingFraction = timeRemaining / maxTime;

        if (remainingFraction > .5f)
        {
            launchCounter = timeBetweenLaunchesEarly;
        }
        else if (remainingFraction > .25f)
        {
            launchCounter = timeBetweenLaunchesMid;
        }
        else
        {
            launchCounter = timeBetweenLaunchesLate;
        }
    }

    private void FailSequence()
    {
        isRunning = false;
        LevelManager.instance.Respawn();
    }

    // Llamado desde TimeBonusPickup
    public void AddTime(float seconds)
    {
        if (!isRunning) return;

        timeRemaining = Mathf.Min(timeRemaining + seconds, maxTime);
        timeSlider.value = timeRemaining;
    }

    // Llamado desde SequenceGoal
    public void ReachGoal()
    {
        if (!isRunning) return;

        StartCoroutine(SucceedSequenceCo());
    }

    private IEnumerator SucceedSequenceCo()
    {
        isRunning = false;

        yield return new WaitForSeconds(1f);

        levelExitOnSuccess.SetActive(true);
        gameObject.SetActive(false);
    }
}
