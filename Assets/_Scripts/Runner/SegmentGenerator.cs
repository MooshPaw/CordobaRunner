using System.Collections;
using UnityEngine;

public class SegmentGenerator : MonoBehaviour
{
    // Stores map "segments" for infinite generation
    public GameObject[] segments;
    
    // Handles coordinates data
    [SerializeField] private int zPos = 50;
    [SerializeField] private bool creatingSegment = false;
    [SerializeField] private int segmentNum;

    private void Start()
    {
        creatingSegment = false;
        StartCoroutine(SegmentGen());
    }

    IEnumerator SegmentGen()
    {
        segmentNum = Random.Range(0, 3);
        Instantiate(segments[segmentNum], new Vector3(0, 0, zPos), Quaternion.identity);
        zPos += 50;
        yield return new WaitForSeconds(3);
        creatingSegment = true;
    }
}
