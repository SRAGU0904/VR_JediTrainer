using System;
using System.Collections.Generic;
using System.Linq;
using EzySlice;
using JetBrains.Annotations;
using UnityEngine;

public class SliceKnife : MonoBehaviour {
	[CanBeNull]
	public static GameObject[] SliceWithCorners(GameObject objectToSlice, Quadrilateral quadrilateral, bool addColider = true, bool addRigidbodies = true, bool destroyOriginal = true, Material sliceMaterial = null) {
		return SliceWithNormals(objectToSlice, quadrilateral.center, quadrilateral.normal, addColider, addRigidbodies, destroyOriginal, sliceMaterial);
	}

	[CanBeNull]
	public static GameObject[] SliceWithNormals(GameObject objectToSlice, Vector3 planeCenter, Vector3 planeNormal, bool addColider = true, bool addRigidbodies = true, bool destroyOriginal = true, Material sliceMaterial = null) {
		if (!objectToSlice) {
			Debug.LogError("Object to slice is null!");
			return null;
		}
		GameObject[] objects =  objectToSlice.SliceInstantiate(planeCenter, planeNormal);
		if (objects != null) {
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
			if (destroyOriginal) {
				Destroy(objectToSlice);
			}
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