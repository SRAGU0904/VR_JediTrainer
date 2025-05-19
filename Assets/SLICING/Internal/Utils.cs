using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Plane {
	public Vector3 center { get; }
	public Vector3 normal { get; }
	public Plane(Vector3 center, Vector3 normal) {
		this.center = center;
		this.normal = normal;
	}

	public GameObject Render() {
		GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		plane.transform.position = center;
		plane.transform.rotation = Quaternion.LookRotation(normal);
		plane.transform.localScale = new Vector3(10, 10, 10);
		return plane;
	}
}

public class Quadrilateral {
	private Vector3 bottomLeft { get; }
	private Vector3 bottomRight { get; }
	private Vector3 topRight { get; }
	private Vector3 topLeft { get; }
	
	public Quadrilateral(Vector3 bottomLeft, Vector3 bottomRight, Vector3 topRight, Vector3 topLeft) {
		this.bottomLeft = bottomLeft;
		this.bottomRight = bottomRight;
		this.topRight = topRight;
		this.topLeft = topLeft;
	}
	
	public Vector3 center => (bottomLeft + bottomRight + topRight + topLeft) / 4;
	public Vector3 normal => Vector3.Cross(topRight - bottomLeft, topLeft - bottomRight).normalized;
	public Plane plane => new Plane(center, normal);

	public Mesh CreateMesh() {
		Vector3[] vertices = new Vector3[] { center, bottomLeft, bottomRight, topRight, topLeft };
		int[] triangles = new int[] {
			0,1,2,
			0,2,3,
			0,3,4,
			0,4,1
		};
		Mesh result = new Mesh();
		result.SetVertices(vertices);
		result.SetTriangles(triangles, 0);
		result.RecalculateNormals();
		return result;
	}

	public Quadrilateral ExpandedTop(float multiplier) {
		return new Quadrilateral(
			bottomLeft,
			bottomRight,
			(topRight - bottomRight) * multiplier + bottomRight,
			(topLeft - bottomLeft) * multiplier + bottomLeft
		);
	}

	public void Render() {
		GameObject quadGo = new GameObject("Quadrilateral");
		MeshFilter mf = quadGo.AddComponent<MeshFilter>();
		mf.mesh = CreateMesh();
		MeshRenderer mr = quadGo.AddComponent<MeshRenderer>();
		mr.material = new Material(Shader.Find("Sprites/Default"));
		mr.material.color = new Color(0.5f, 0.5f, 0.5f, 0.1f);
		
	}

	public IEnumerable<GameObject> GetCollisions(string tagFilter) {
		GameObject quadGo = MeshUtils.CreateGameObject(CreateMesh());
		MeshCollider mc = MeshUtils.AddMeshCollider(quadGo, false);
		Collider[] hitColliders = Physics.OverlapBox(center, mc.bounds.extents, Quaternion.identity);
		HashSet<GameObject> seen = new HashSet<GameObject>();
		quadGo.SetActive(false);
		if (hitColliders is null) {
			yield break;
		}
		foreach (Collider hitCollider in hitColliders) {
			GameObject go = hitCollider.gameObject;
			if (seen.Contains(go)) {
				continue;
			}
			seen.Add(go);
			if (go.activeSelf && go.CompareTag(tagFilter) && go.gameObject != quadGo) {
				yield return go;
			}
		}
		UnityEngine.Object.Destroy(quadGo);
	}
}

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
	
}


	
