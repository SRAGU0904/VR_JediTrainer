using System;
using System.Collections.Generic;
using System.Linq;
using EzySlice;
using JetBrains.Annotations;
using UnityEngine;

public class SliceKnife : MonoBehaviour {
	[CanBeNull]
	public static GameObject[] SliceWithCorners(GameObject objectToSlice, Vector3 BLCorner, Vector3 BRCorner,
		Vector3 TRCorner, Vector3 TLCorner, bool addColider = true, bool addRigidbodies = true, bool destroyOriginal = true, Material sliceMaterial = null) {
		Vector3 planeCenter = (BLCorner + BRCorner + TRCorner + TLCorner) / 4;
		Vector3 planeNormal = Vector3.Cross(TRCorner - BLCorner, TLCorner - BRCorner).normalized;
		return SliceWithNormals(objectToSlice, planeCenter, planeNormal, addColider, addRigidbodies, destroyOriginal, sliceMaterial);
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
				addMeshCollider(objects[0]);
				addMeshCollider(objects[1]);
			}
			if (addRigidbodies) {
				addRigidbody(objects[0]);
				addRigidbody(objects[1]);
			}
			if (sliceMaterial) {
				updateMaterial(objects[0].GetComponent<MeshRenderer>(), ^1, sliceMaterial); 
				updateMaterial(objects[1].GetComponent<MeshRenderer>(), ^1, sliceMaterial);
			}
			if (destroyOriginal) {
				Destroy(objectToSlice);
			}
		}
		return objects;
	}

	public static void updateMaterial(MeshRenderer mr, Index i, Material mat) {
		Material[] mats = mr.materials;
		mats[i] = mat;
		mr.SetMaterials(mats.ToList());
	}

	public static void addRigidbody(GameObject go) {
		Rigidbody rb = go.AddComponent<Rigidbody>();
		rb.useGravity = true;
		rb.isKinematic = false;
		rb.interpolation = RigidbodyInterpolation.Interpolate;
		rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
	}
	
	public static void addMeshCollider(GameObject go) {
		MeshCollider mc = go.AddComponent<MeshCollider>();
		mc.convex = true;
	}
	
	
}