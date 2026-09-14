using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SegmentGenerator : MonoBehaviour
{
    // Stores map "segments" for infinite generation
    public GameObject[] segments;
    
    // Handles coordinates data
    [SerializeField] private int zPos = 20;
    [SerializeField] private bool creatingSegment = false;
    [SerializeField] private int segmentNum;
    
    // Starts with a 3x segment generation
    private void Start()
    {
        segmentNum = Random.Range(0, 3);
        Instantiate(segments[segmentNum], new Vector3(0, 0, zPos), Quaternion.identity);
        zPos += 20;
        segmentNum = Random.Range(0, 3);
        Instantiate(segments[segmentNum], new Vector3(0, 0, zPos), Quaternion.identity);
        zPos += 20;
        segmentNum = Random.Range(0, 3);
        Instantiate(segments[segmentNum], new Vector3(0, 0, zPos), Quaternion.identity);
        zPos += 20;
    }

    private void Update()
    {
        if (!creatingSegment)
        {
            creatingSegment = true;
            StartCoroutine(SegmentGen());   
        }
    }

    IEnumerator SegmentGen()
    {
        segmentNum = Random.Range(0, 3);
        Instantiate(segments[segmentNum], new Vector3(0, 0, zPos), Quaternion.identity);
        zPos += 20;
        yield return new WaitForSeconds(2);
        creatingSegment = false;
    }
}
