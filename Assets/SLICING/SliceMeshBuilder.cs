using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SliceMeshBuilder : MonoBehaviour
{
	public bool lowRes = false;
	
	public void SetMesh(List<Vector3[]> lines)
	{
		if (lines.Count == 0) {
			return;
		}	
		if (lowRes) {
			List<Vector3[]> newLines = new List<Vector3[]>();
			newLines.Add(lines.First());
			newLines.Add(lines.Last());
			lines = newLines;
		}
		Mesh mesh = BuildMeshFromLines(lines);
		GetComponent<MeshFilter>().mesh = mesh;
		GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
	}
	
	private Mesh BuildMeshFromLines(List<Vector3[]> lines)
	{
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();

		for (int i = 0; i < lines.Count - 1; i++)
		{
			Vector3 a1 = lines[i][0];
			Vector3 a2 = lines[i][1];
			Vector3 b1 = lines[i + 1][0];
			Vector3 b2 = lines[i + 1][1];

			int baseIndex = vertices.Count;

			vertices.Add(a1); // 0
			vertices.Add(a2); // 1
			vertices.Add(b1); // 2
			vertices.Add(b2); // 3

			// First triangle
			triangles.Add(baseIndex + 0);
			triangles.Add(baseIndex + 2);
			triangles.Add(baseIndex + 1);

			// Second triangle
			triangles.Add(baseIndex + 1);
			triangles.Add(baseIndex + 2);
			triangles.Add(baseIndex + 3);
		}

		Mesh mesh = new Mesh();
		mesh.SetVertices(vertices);
		mesh.SetTriangles(triangles, 0);
		mesh.RecalculateNormals();
		return mesh;
	}
}