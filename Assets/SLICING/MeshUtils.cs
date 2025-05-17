using System.Collections.Generic;
using UnityEngine;

public class MeshUtils : MonoBehaviour
{
	public static MeshCollider AddMeshCollider(GameObject go, bool convex = true) {
		MeshCollider mc = go.AddComponent<MeshCollider>();
		mc.convex = convex;
		return mc;
	}

	public static GameObject CreateGameObject(Mesh mesh) {
		GameObject result = new GameObject("MeshGameObject");
		MeshFilter mf = result.AddComponent<MeshFilter>();
		mf.mesh = mesh;
		return result;
	}
	
	// public static Vector3 GetCenterOfMass(IEnumerable<Rigidbody> bodies) {
	// 	Vector3 result = Vector3.zero;
	// 	float totalMass = 0;
	// 	foreach (Rigidbody body in bodies) {
	// 		float mass = body.mass;
	// 		result += mass * body.centerOfMass;
	// 		totalMass += mass;
	// 	}
	// 	return result / totalMass;
	// }
}
