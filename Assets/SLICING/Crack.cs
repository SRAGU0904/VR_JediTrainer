using System;
using UnityEngine;

namespace Slicing {
	
	class SlicingCrack : MonoBehaviour {
		public float angleTolerance = 10;
		public float distTolerance = 0.5f;
		public GameObject swingTargetObj;
		public Vector3 swingTargetDir = Vector3.right;
		public Vector3 sliceNormalDir = Vector3.up;

		private bool angleMatch(Vector3 planeNormal) {
			Vector3 translatedNormal = swingTargetObj.transform.InverseTransformDirection(planeNormal);
			// TODO: can be optimized by using CrossProduct (avoids arctan)
			float angle = Vector3.Angle(swingTargetDir.normalized, translatedNormal);
			bool angleMatch = Math.Abs(angle - 90) < angleTolerance;
			Debug.Log($"angle: {angle}, angleMatch: {angleMatch}");
			return angleMatch;
		}

		private bool distanceMatch(Vector3 planeCenter, Vector3 planeNormal) {
			Vector3 displacement = planeCenter - swingTargetObj.transform.position;
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
			SlicingCrack instance;
			if (go.TryGetComponent(out instance)) {
				if (!instance.CheckIfCracked(planeInput)) {
					// "Crackable" object and test failed - stop slicing by returning null
					return null;
				}

				// "Crackable" object and test succeeded - override slicing plane to "snap" to the crack
				return instance.GetCrackPlane();
			}

			// Not a "Crackable" object - pass the input through
			return planeInput;
		}

		private Plane GetCrackPlane() {
			Vector3 center = swingTargetObj.transform.position;
			Vector3 normal = swingTargetObj.transform.TransformDirection(sliceNormalDir);
			Plane result = new Plane(center, normal);
			return result;
		}
		
		private void OnDestroy() {
			Destroy(swingTargetObj);
		}

	}
}
