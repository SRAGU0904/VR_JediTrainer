using System;
using System.Collections.Generic;
using EzySlice;
using UnityEngine;

public class TEST_SlicingTest : MonoBehaviour {
	public Transform slicingPlaneBL;
	public Transform slicingPlaneBR;
	public Transform slicingPlaneTL;
	public Transform slicingPlaneTR;
	public SliceMeshBuilder sliceMeshBuilder;
	public GameObject objectToSlice;

	private void Start() {
		List<Vector3[]> lines = new List<Vector3[]>();
		lines.Add(new Vector3[] { slicingPlaneBL.position, slicingPlaneBR.position });
		lines.Add(new Vector3[] { slicingPlaneTL.position, slicingPlaneTR.position });
		sliceMeshBuilder.SetMesh(lines);
		GameObject[] result = SliceKnife.SliceWithCorners(objectToSlice, slicingPlaneBL.position, slicingPlaneBR.position, slicingPlaneTR.position, slicingPlaneTL.position);
		// Why not working?
		transform.position = (slicingPlaneBL.position + slicingPlaneBR.position + slicingPlaneTL.position + slicingPlaneTR.position) / 4;
	}
}
