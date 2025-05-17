using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

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

	public Quadrilateral Expanded(float multiplier) {
		Vector3 _center = center;
		return new Quadrilateral(
			(bottomLeft - _center) * multiplier + _center,
			(bottomRight - _center) * multiplier + _center,
			(topRight - _center) * multiplier + _center,
			(topLeft - _center) * multiplier + _center
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
		GameObjectDestroyer.DestroyGameObject(quadGo);
	}
}

	
