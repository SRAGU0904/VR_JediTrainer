using System;
using System.Collections.Generic;
using System.Linq;
using EzySlice;
using JetBrains.Annotations;
using Slicing;
using UnityEngine;

namespace Slicing {

	[RequireComponent(typeof(DefaultSlicingBehaviour))]
	class SlicingKnife : MonoBehaviour {

		public bool addColider = true;
		public bool addRigidbodies = true;
		public float pushForceMagnitude = 1f;
		public float pushInitialInSeconds = 1f;
		public int sliceCountLimit = 4;
		private DefaultSlicingBehaviour _defaultBehaviour;

		private void Start() {
			_defaultBehaviour = GetComponent<DefaultSlicingBehaviour>();
		}

		[CanBeNull]
		public GameObject[] Slice(GameObject objectToSlice, Plane plane) {
			int sliceCount = SliceCounter.GetSliceCount(objectToSlice);
			sliceCount++;
			if (sliceCount >= sliceCountLimit) {
				Destroy(objectToSlice);
				return null;
			}

			GameObject[] hulls = GetBehaviour(objectToSlice).CreateHulls(objectToSlice, plane.center, plane.normal);
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
				pushHulls(hulls, plane.normal, pushForceMagnitude, pushInitialInSeconds);
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

		private static Vector3 GetWorkaroundShift(GameObject original, GameObject[] hull) {
			// Very simple approximation
			Vector3 meanHullPos = hull.Select(go => go.transform.position).Aggregate((a, b) => a + b) / hull.Length;
			return meanHullPos - original.transform.position;
		}

		public ISlicingBehaviour GetBehaviour(GameObject go) {
			ISlicingBehaviour[] instances = go.GetComponents<ISlicingBehaviour>();
			if (instances.Length > 1) {
				Debug.LogError($"More than one ISlicingBehaviour component on {go.name}!");
			}
			return instances.FirstOrDefault() ?? _defaultBehaviour;
			
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