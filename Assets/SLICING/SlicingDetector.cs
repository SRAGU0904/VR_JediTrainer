using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;

public class SlicingDetector : MonoBehaviour {
	public VelocityTracker velocityTracker;
	public float angVelocityThreshold = 5f;
	public Transform slicerBeginEffector;
	public Transform slicerEndEffector;
	public float aoeExpandFactor = 3f;
	public SliceCrack debugCrack;
	
	private bool _currentlySlicing = false;
	private bool _previouslySlicing = false;
	

	[CanBeNull] private Tuple<Vector3, Vector3> sliceFirst = null;
	
	private void onSlicingStart() {
		sliceFirst = new Tuple<Vector3, Vector3>(slicerBeginEffector.position, slicerEndEffector.position);
	}

	private void triggerSlicing(Quadrilateral quad) {
		quad = quad.ExpandedTop(aoeExpandFactor);
		// Debug:
		quad.Render();
		Plane quadPlane = quad.plane;

		foreach (GameObject objectToSlice in quad.GetCollisions("SliceTarget")) {
			Plane overridenPlane = SliceCrack.SnapCrackedGameObject(objectToSlice, quadPlane);
			if (overridenPlane is null) {
				continue;
			} 
			SliceKnife.Slice(objectToSlice, quad);
		}
	}
		
	private void onSlicingStop() {
		Debug.Log("Slicing stopped!");
		if (sliceFirst is null) {
			Debug.LogWarning("Slicing stopped but no points recorded!");
			return;
		}
		Tuple<Vector3, Vector3> sliceLast = new Tuple<Vector3, Vector3>(slicerBeginEffector.position, slicerEndEffector.position);
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
		Vector3 angVel = velocityTracker.GetAngularVelocityEstimate();
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