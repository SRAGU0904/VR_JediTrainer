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
	public Material sliceMaterial = null;
	public float pushForce = 5000f;
	public float pushInitialInSeconds = 0.5f;
	
	private bool currentlySlicing = false;
	private bool previouslySlicing = false;

	[CanBeNull] private Tuple<Vector3, Vector3> sliceFirst = null;
	
	private void onSlicingStart() {
		sliceFirst = new Tuple<Vector3, Vector3>(slicerBeginEffector.position, slicerEndEffector.position);
	}

	private void onSlicingStop() {
		Debug.Log("Slicing stopped!");
		if (sliceFirst is null) {
			Debug.LogWarning("Slicing stopped but no points recorded!");
		}
		if (sliceFirst is not null) {
			Tuple<Vector3, Vector3> sliceLast = new Tuple<Vector3, Vector3>(slicerBeginEffector.position, slicerEndEffector.position);
			Quadrilateral quad = new Quadrilateral(
				sliceFirst.Item1,
				sliceLast.Item1,
				sliceLast.Item2,
				sliceFirst.Item2
			);
			// quad = quad.Expanded(2f);
			// For debug purposes
			quad = quad.Expanded(100f);
			foreach (GameObject objectToSlice in quad.GetCollisions("SliceTarget")) {
				GameObject[]? newHulls = SliceKnife.SliceWithCorners(objectToSlice, quad, true, true, true, sliceMaterial);
				if (newHulls is not null) {
					Debug.Log($"Sliced! {objectToSlice.name}");
					float signum = 1;
					Pushable pushable = objectToSlice.GetComponent<Pushable>();;
					if (pushable is null) {
						Debug.LogWarning($"Sliced object {objectToSlice.name} does not have a Pushable component!");
					}
					foreach (GameObject hull in newHulls) {
						hull.tag = "SliceTarget";
						Pushable hullPushable = hull.AddComponent<Pushable>();
						pushable.CopyTo(hullPushable);
						hullPushable.Push(signum * pushForce * quad.normal, pushInitialInSeconds);
						Debug.Log($"Pushed {hull.name} with force {(pushForce * quad.normal).magnitude}");
						signum *= -1;
					}
				}
			}
			
		}
		sliceFirst = null;
	}

	private bool checkSlicing() {
		Vector3 angVel = velocityTracker.GetAngularVelocityEstimate();
		currentlySlicing = (angVel.magnitude > angVelocityThreshold);
		if (currentlySlicing && !previouslySlicing) {
			onSlicingStart();
		}
		else if (!currentlySlicing && previouslySlicing) {
			onSlicingStop();
		}

		previouslySlicing = currentlySlicing;
		return currentlySlicing;
	}
	
	private void Update() {
		checkSlicing();
	}

}