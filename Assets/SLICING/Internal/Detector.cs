using System;
using Slicing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;


namespace Slicing {

	[RequireComponent(typeof(VelocityTracker))]
	[RequireComponent(typeof(SlicingKnife))]
	class SlicingDetector : MonoBehaviour {
		public float angVelocityThreshold = 7f;
		public Transform slicerBeginEffector;
		public Transform slicerEndEffector;
		public float aoeExpandFactor = 5f;
		public bool debug = false;

		private bool _currentlySlicing = false;
		private bool _previouslySlicing = false;
		private VelocityTracker _velocityTracker;
		private SlicingKnife _slicingKnife;

		private void Start() {
			_velocityTracker = GetComponent<VelocityTracker>();
			_slicingKnife = GetComponent<SlicingKnife>();
		}

		[CanBeNull] private Tuple<Vector3, Vector3> sliceFirst = null;

		private void onSlicingStart() {
			sliceFirst = new Tuple<Vector3, Vector3>(slicerBeginEffector.position, slicerEndEffector.position);
		}

		private void triggerSlicing(Quadrilateral quad) {
			quad = quad.ExpandedTop(aoeExpandFactor);
			if (debug)
			{
				quad.Render();
			}
			Plane quadPlane = quad.plane;

			foreach (GameObject objectToSlice in quad.GetCollisions("SliceTarget")) {
				Plane overridenPlane = SlicingCrack.SnapCrackedGameObject(objectToSlice, quadPlane);
				if (overridenPlane is null) {
					continue;
				}
				_slicingKnife.Slice(objectToSlice, quad);
			}
		}

		private void onSlicingStop() {
			Debug.Log("Slicing stopped!");
			if (sliceFirst is null) {
				Debug.LogWarning("Slicing stopped but no points recorded!");
				return;
			}

			Tuple<Vector3, Vector3> sliceLast =
				new Tuple<Vector3, Vector3>(slicerBeginEffector.position, slicerEndEffector.position);
			Quadrilateral quad = new Quadrilateral(
				sliceFirst.Item1,
				sliceLast.Item1,
				sliceLast.Item2,
				sliceFirst.Item2
			);
			triggerSlicing(quad);
			sliceFirst = null;
		}

		private bool checkSlicing() {
			Vector3 angVel = _velocityTracker.GetAngularVelocityEstimate();
			_currentlySlicing = (angVel.magnitude > angVelocityThreshold);
			if (_currentlySlicing && !_previouslySlicing) {
				onSlicingStart();
			}
			else if (!_currentlySlicing && _previouslySlicing) {
				onSlicingStop();
			}

			_previouslySlicing = _currentlySlicing;
			return _currentlySlicing;
		}

		private void Update() {
			checkSlicing();
		}

	}
}