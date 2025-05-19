using System;
using System.Linq;
using EzySlice;
using JetBrains.Annotations;
using Slicing;
using UnityEngine;

namespace Slicing {

	public class DefaultSlicingBehaviour : MonoBehaviour, ISlicingBehaviour {
		
		public bool addColider = true;
		public bool addRigidbodies = true;
		
		[CanBeNull]
		public GameObject[] CreateHulls(GameObject objectToSlice, Vector3 planeCenter, Vector3 planeNormal) {
			if (!objectToSlice) {
				Debug.LogError("Object to slice is null!");
				return null;
			}

			Debug.Log("Slicing object " + objectToSlice.name);
			Debug.Log("Plane center: " + planeCenter);
			Debug.Log("Plane normal: " + planeNormal);
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
	}

}