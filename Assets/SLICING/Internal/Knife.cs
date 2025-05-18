using System;
using System.Collections.Generic;
using System.Linq;
using EzySlice;
using JetBrains.Annotations;
using UnityEngine;

namespace Slicing {

	class SlicingKnife : MonoBehaviour {

		public bool addColider = true;
		public bool addRigidbodies = true;
		public float pushForceMagnitude = 1f;
		public float pushInitialInSeconds = 1f;
		public int sliceCountLimit = 4;

		[CanBeNull]
		public GameObject[] Slice(GameObject objectToSlice, Quadrilateral quadrilateral) {
			int sliceCount = SliceCounter.GetSliceCount(objectToSlice);
			sliceCount++;
			if (sliceCount >= sliceCountLimit) {
				Destroy(objectToSlice);
				return null;
			}

			GameObject[] hulls = createHulls(objectToSlice, quadrilateral.center, quadrilateral.normal);
			if (hulls is null) {
				return null;
			}

			Vector3 workaroundShift = GetWorkaroundShift(objectToSlice, hulls);
			Destroy(objectToSlice);
			foreach (GameObject hull in hulls) {
				SliceCounter.SetSliceCount(hull, sliceCount);
				hull.tag = "SliceTarget";
				hull.layer = LayerMask.NameToLayer("Hulls");
				hull.transform.position -= workaroundShift;
			}

			if (addRigidbodies) {
				pushHulls(hulls, quadrilateral.normal, pushForceMagnitude, pushInitialInSeconds);
			}

			return hulls;
		}

		private void pushHulls(GameObject[] hulls, Vector3 planeNormal, float pushForceMagnitude,
			float pushInitialInSeconds) {
			float signum = 1f;
			foreach (GameObject hull in hulls) {
				Pushable.pushGameObject(hull, signum * pushForceMagnitude * planeNormal, pushInitialInSeconds);
				signum *= -1;
			}
		}

		[CanBeNull]
		private GameObject[] createHulls(GameObject objectToSlice, Vector3 planeCenter, Vector3 planeNormal) {
			if (!objectToSlice) {
				Debug.LogError("Object to slice is null!");
				return null;
			}

			GameObject[] objects = objectToSlice.SliceInstantiate(planeCenter, planeNormal);
			if (objects == null) {
				Debug.Log("Slicing unsuccessful");
				return null;
			}

			if (addColider) {
				IgnoreHullCollisions.EnsureIgnored();
				MeshUtils.AddMeshCollider(objects[0]);
				MeshUtils.AddMeshCollider(objects[1]);
			}

			if (addRigidbodies) {
				AddRigidbody(objects[0]);
				AddRigidbody(objects[1]);
			}

			UpdateMaterial(objects[0].GetComponent<MeshRenderer>(), 0, ^1);
			UpdateMaterial(objects[1].GetComponent<MeshRenderer>(), 0, ^1);
			return objects;
		}

		private static void UpdateMaterial(MeshRenderer mr, Index source, Index target) {
			Material[] mats = mr.materials;
			mats[target] = mats[source];
			mr.SetMaterials(mats.ToList());
		}

		private static void AddRigidbody(GameObject go) {
			Rigidbody rb = go.AddComponent<Rigidbody>();
			rb.useGravity = true;
			rb.isKinematic = false;
			rb.interpolation = RigidbodyInterpolation.Interpolate;
			rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
		}

		private static Vector3 GetWorkaroundShift(GameObject original, GameObject[] hull) {
			// Very simple approximation
			Vector3 meanHullPos = hull.Select(go => go.transform.position).Aggregate((a, b) => a + b) / hull.Length;
			return meanHullPos - original.transform.position;
		}
	}

	class IgnoreHullCollisions {
		public static bool initialized = false;

		public static void EnsureIgnored() {
			if (initialized) {
				return;
			}

			Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Hulls"), LayerMask.NameToLayer("Hulls"));
			initialized = true;
		}
	}

}