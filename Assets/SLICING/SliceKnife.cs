using EzySlice;
using JetBrains.Annotations;
using UnityEngine;

public class SliceKnife : MonoBehaviour {
	[CanBeNull]
	public static GameObject[] SliceWithCorners(GameObject objectToSlice, Vector3 BLCorner, Vector3 BRCorner,
		Vector3 TRCorner, Vector3 TLCorner, bool addColider = true, bool addRigidbodies = true, bool destroyOriginal = true) {
		Vector3 planeCenter = (BLCorner + BRCorner + TRCorner + TLCorner) / 4;
		Vector3 planeNormal = Vector3.Cross(TRCorner - BLCorner, TLCorner - BRCorner).normalized;
		return SliceWithNormals(objectToSlice, planeCenter, planeNormal, addColider, addRigidbodies, destroyOriginal);
	}

	[CanBeNull]
	public static GameObject[] SliceWithNormals(GameObject objectToSlice, Vector3 planeCenter, Vector3 planeNormal, bool addColider = true, bool addRigidbodies = true, bool destroyOriginal = true) {
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
			if (destroyOriginal) {
				Destroy(objectToSlice);
			}
		}
		return objects;
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