using System;
using UnityEngine;



public class SliceCrack : MonoBehaviour {
	public float angleTolerance = 10;
	public float distTolerance = 0.5f;
	public GameObject crackObj;
	public Vector3 crackDir = Vector3.up;

	private bool angleMatch(Vector3 planeNormal) {
		Vector3 translatedNormal = crackObj.transform.InverseTransformDirection(planeNormal);
		// TODO: can be optimized by using CrossProduct (avoids arctan)
		float angle = Vector3.Angle(crackDir.normalized, translatedNormal);
		bool angleMatch = Math.Abs(angle - 90) < angleTolerance;
		Debug.Log($"angle: {angle}, angleMatch: {angleMatch}");
		return angleMatch;
	}

	private bool distanceMatch(Vector3 planeCenter, Vector3 planeNormal) {
		Vector3 displacement = planeCenter - crackObj.transform.position;
		Vector3 displacementProj = Vector3.ProjectOnPlane(displacement, planeNormal);
		Vector3 displacementOrtho = displacement - displacementProj;
		float dist = displacementOrtho.magnitude;
		bool distMatch = dist < distTolerance;
		Debug.Log($"dist: {dist}, distMatch: {distMatch}");
		return distMatch;
	}
	public bool CheckIfCracked(Plane planeInput) {
		return angleMatch(planeInput.normal) && distanceMatch(planeInput.center, planeInput.normal);
	}

	public static Plane SnapCrackedGameObject(GameObject go, Plane planeInput) {
		SliceCrack instance;
		if (go.TryGetComponent(out instance)) {
			if (!instance.CheckIfCracked(planeInput)) {
				// "Crackable" object and test failed - stop slicing by returning null
				return null;
			}
			// "Crackable" object and test succeeded - override slicing plane to "snap" to the crack
			return new Plane(go.transform.position, instance.crackDir.normalized);
		}
		// Not a "Crackable" object - pass the input through
		return planeInput;
	}

	private void OnDestroy() {
		Destroy(crackObj);
	}

	// public static void createDebugSphere(Vector3 pos, string name) {
	// 	GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
	// 	sphere.name = name;
	// 	sphere.transform.position = pos;
	// 	sphere.transform.localScale = Vector3.one * 0.1f;
	// }
	
}
