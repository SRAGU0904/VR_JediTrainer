using Slicing;
using UnityEngine;
using UnityEngine.Assertions;

public class BallTutorialBehaviour : MonoBehaviour, ISlicingBehaviour
{
	public float NextStageDelay = 3f;
	public int myStage = -1;

	void OnValidate() {
		Assert.IsTrue(myStage >= 0, "Stage index not set!");;
	}
	
	public void OnSlicingFinished() {
		TutorialController.StageFinished(myStage, NextStageDelay);
	}

    public GameObject[] CreateHulls(GameObject objectToSlice, Vector3 planeCenter, Vector3 planeNormal) {
        GameObject[] slicedObjects = new GameObject[2];
        slicedObjects[0] = new GameObject("SlicedHull1");
        slicedObjects[1] = new GameObject("SlicedHull2");
        return slicedObjects;
    }

    public void PushHulls(GameObject[] hulls, Vector3 planeNormal) { }
}
