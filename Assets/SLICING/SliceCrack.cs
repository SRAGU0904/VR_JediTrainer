using System;
using UnityEngine;

public class SliceCrack : MonoBehaviour {
	public float angleTolerance = 0.03f;
	public float distTolerance = 0.1f;
	public Vector3 crackDir = Vector3.up;

	private bool angleMatch(Vector3 planeNormal) {
		Vector3 translatedNormal = transform.InverseTransformDirection(planeNormal);
		// TODO: can be optimized by using CrossProduct (avoids arctan)
		float angle = Vector3.Angle(crackDir.normalized, translatedNormal);
		bool angleMatch = Math.Abs(angle - 90) < angleTolerance;
		Debug.Log($"angle: {angle}, angleMatch: {angleMatch}");
		return angleMatch;
	}

	private bool distanceMatch(Vector3 planeCenter) {
		Vector3 translatedCenter = transform.InverseTransformPoint(planeCenter);
		translatedCenter = Vector3.ProjectOnPlane(translatedCenter, crackDir);
		createDebugSphere(transform.TransformPoint(translatedCenter), "translatedCenter" );
		bool distMatch = translatedCenter.magnitude < distTolerance;
		Debug.Log($"translatedCenter.magnitude: {translatedCenter.magnitude}, distMatch: {distMatch}");
		return distMatch;
	}
	public bool Cracked(Vector3 planeCenter, Vector3 planeNormal) {
		return angleMatch(planeNormal) && distanceMatch(planeCenter);
	}

	public static void createDebugSphere(Vector3 pos, string name) {
		GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere.name = name;
		sphere.transform.position = pos;
		sphere.transform.localScale = Vector3.one * 0.1f;
	}
}
