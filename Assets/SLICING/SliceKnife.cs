using System;
using System.Collections.Generic;
using System.Linq;
using EzySlice;
using JetBrains.Annotations;
using UnityEngine;

public class SliceKnife : MonoBehaviour {
	// TODO: make not static

	public static bool addColider = true;
	public static bool addRigidbodies = true;
	public static Material sliceMaterial = null;
	public static float pushForceMagnitude = 3f;
	public static float pushInitialInSeconds = 0.5f;
	public static int sliceCountLimit = 4;
	
	[CanBeNull]
	public static GameObject[] Slice(GameObject objectToSlice, Quadrilateral quadrilateral){
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
		Destroy(objectToSlice);
		pushHulls(hulls, quadrilateral.normal, pushForceMagnitude, pushInitialInSeconds);
		foreach (GameObject hull in hulls) {
			SliceCounter.SetSliceCount(hull, sliceCount);
			hull.tag = "SliceTarget";
		}
		return hulls;
	}

	private static void pushHulls(GameObject[] hulls, Vector3 planeNormal, float pushForceMagnitude,
		float pushInitialInSeconds) {
		float signum = 1f;
		foreach (GameObject hull in hulls) {
			Pushable.pushGameObject(hull, signum * pushForceMagnitude * planeNormal, pushInitialInSeconds);
			signum *= -1;
		}
	}
	
	[CanBeNull]
	private static GameObject[] createHulls(GameObject objectToSlice, Vector3 planeCenter, Vector3 planeNormal) {
		if (!objectToSlice) {
			Debug.LogError("Object to slice is null!");
			return null;
		}
		GameObject[] objects =  objectToSlice.SliceInstantiate(planeCenter, planeNormal);
		if (objects == null) {
			Debug.Log("Slicing unsuccessful");
			return null;
		}
		if (addColider) {
			MeshUtils.AddMeshCollider(objects[0]);
			MeshUtils.AddMeshCollider(objects[1]);
		}
		if (addRigidbodies) {
			AddRigidbody(objects[0]);
			AddRigidbody(objects[1]);
		}
		if (sliceMaterial) {
			UpdateMaterial(objects[0].GetComponent<MeshRenderer>(), ^1, sliceMaterial); 
			UpdateMaterial(objects[1].GetComponent<MeshRenderer>(), ^1, sliceMaterial);
		}
		return objects;
	}

	private static void UpdateMaterial(MeshRenderer mr, Index i, Material mat) {
		Material[] mats = mr.materials;
		mats[i] = mat;
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