using Slicing;
using UnityEngine;

public class BallBehaviour : MonoBehaviour, ISlicingBehaviour
{

    public GameObject[] CreateHulls(GameObject objectToSlice, Vector3 planeCenter, Vector3 planeNormal) {
        GameObject[] slicedObjects = new GameObject[2];
        slicedObjects[0] = new GameObject("SlicedHull1");
        slicedObjects[1] = new GameObject("SlicedHull2");
        return slicedObjects;
    }

    public void PushHulls(GameObject[] hulls, Vector3 planeNormal) { }
}
