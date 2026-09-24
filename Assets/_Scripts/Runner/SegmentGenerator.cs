using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SegmentGenerator : MonoBehaviour
{
    public static SegmentGenerator Instance { get; private set; }

    // Stores map "segments" for infinite generation
    public GameObject[] segments;

    [Tooltip("Assign the player's Transform here — used to drive both generation and deletion by Z position.")]
    public Transform player;

    [Tooltip("Always keep at least this much unexplored track generated ahead of the player. " +
             "Should comfortably cover the most distance the player could travel in one frame at max speed.")]
    public float aheadBufferDistance = 160f; // e.g. 8 segments' worth at 20 units each

    // Parallel lists: activeSegments[i] started at startZs[i]. Never removed from directly —
    // deleted segments are set to null so indices stay stable and monotonic.
    private readonly List<GameObject> activeSegments = new List<GameObject>();
    private readonly List<float> startZs = new List<float>();

    // How many segment-starts the player has already crossed
    private int enteredCount = 0;

    // Handles coordinates data
    [SerializeField] private int zPos = 20;
    [SerializeField] private int segmentNum;
    public int segmentLength = 20;

    private void Awake()
    {
        Instance = this;
    }

    // Starts with a 3x segment generation
    private void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            SpawnSegment();
        }
    }

    private void Update()
    {
        MaintainAheadBuffer();
        CheckSegmentCrossings();
    }

    // Keeps spawning segments as long as the generated track doesn't extend
    // far enough past the player. Naturally scales with speed: a faster
    // player closes the buffer sooner, so this fires more often on its own —
    // no timer needed.
    private void MaintainAheadBuffer()
    {
        if (player == null) return;

        while (zPos - player.position.z < aheadBufferDistance)
        {
            SpawnSegment();
        }
    }

    private void SpawnSegment()
    {
        segmentNum = Random.Range(0, segments.Length);
        GameObject newSegment = Instantiate(segments[segmentNum], new Vector3(0, 0, zPos), Quaternion.identity);
        activeSegments.Add(newSegment);
        startZs.Add(zPos);
        zPos += segmentLength;
    }

    // Checks the player's current Z against the next uncrossed segment start.
    // Uses a while loop (not if) in case the player crosses more than one
    // segment boundary in a single frame at very high speed.
    private void CheckSegmentCrossings()
    {
        if (player == null) return;

        while (enteredCount < startZs.Count && player.position.z >= startZs[enteredCount])
        {
            int targetIndex = enteredCount - 2;
            if (targetIndex >= 0 && targetIndex < activeSegments.Count && activeSegments[targetIndex] != null)
            {
                Destroy(activeSegments[targetIndex]);
                activeSegments[targetIndex] = null;
            }

            enteredCount++;
        }
    }
}