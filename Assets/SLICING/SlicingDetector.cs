using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlicingDetector : MonoBehaviour {
	public VelocityTracker velocityTracker;
	public float angVelocityThreshold = 5f;
	public Transform slicerBeginEffector;
	public Transform slicerEndEffector;
	public SliceMeshBuilder sliceMeshBuilder;
	public GameObject objectToSlice;
	public Material sliceMaterial = null;
	
	private bool currentlySlicing = false;
	private bool previouslySlicing = false;
	private List<Vector3[]> sliceLineList;

	private void Start() {
		sliceLineList = new List<Vector3[]>();
	}
	
	void FixedUpdate() {
		if (checkSlicing()) {
			Vector3[] sliceLine = new []{slicerBeginEffector.position, slicerEndEffector.position};
			sliceLineList.Add(sliceLine);
		}
	}
	
	private void onSlicingStart() {
		sliceLineList.Clear();
		Debug.Log("Slicing!");
	}

	private void onSlicingStop() {
		Debug.Log("Slicing stopped!");
		sliceMeshBuilder.SetMesh(sliceLineList);
		if (sliceLineList.Count > 0) {
			Vector3 BL = sliceLineList.First()[0];
			Vector3 BR = sliceLineList.Last()[0];
			Vector3 TL = sliceLineList.First()[1];
			Vector3 TR = sliceLineList.Last()[1];
			if (SliceKnife.SliceWithCorners(objectToSlice, BL, BR, TR, TL, true, true, true, sliceMaterial) != null) {
				Debug.Log("Sliced!");
			}
		}
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
	
	// private void OnDrawGizmos() {
	// 	if (slicingSamplePoints != null && slicingSamplePoints.Count >= 2) {
	// 		Gizmos.color = Color.red;
	// 		for (int i = 0; i < slicingSamplePoints.Count - 1; i++) {
	// 			Gizmos.DrawLine(slicingSamplePoints[i], slicingSamplePoints[i + 1]);
	// 		}
	// 	}
	// }
	
}