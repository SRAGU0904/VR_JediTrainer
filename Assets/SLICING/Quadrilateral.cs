using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Quadrilateral {

	public Vector3 bottomLeft { get; private set; }
	public Vector3 bottomRight { get; private set; }
	public Vector3 topRight { get; private set; }
	public Vector3 topLeft { get; private set; }
	
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

	public IEnumerable<GameObject> GetCollisions(string tagFilter) {
		GameObject quadGo = MeshUtils.CreateGameObject(CreateMesh());
		MeshCollider mc = MeshUtils.AddMeshCollider(quadGo);
		Collider[] hitColliders = Physics.OverlapBox(center, mc.bounds.extents, Quaternion.identity);
		quadGo.SetActive(false);
		foreach (Collider hitCollider in hitColliders) {
			if (hitCollider.gameObject.CompareTag(tagFilter) && hitCollider.gameObject != quadGo) {
				yield return hitCollider.gameObject;
			}
		}
		GameObjectDestroyer.DestroyGameObject(quadGo);
	}
}

	
