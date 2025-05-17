using UnityEngine;

public class MeshUtils
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
}
