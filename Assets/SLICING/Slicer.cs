using UnityEngine;

public class Slicer : MonoBehaviour
{
    public Transform slicerBegin;
    public Transform slicerEnd;
    public Transform fakeSword;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fakeSword.position = (slicerBegin.position + slicerEnd.position) / 2;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
