using UnityEngine;
using UnityEditor;
using System.Collections;

public static class MeshGenerator {
	[MenuItem("Custom/GeneratePlaneMesh")]
	public static void GeneratePlaneMesh() {
		var nGrids = 100;
		var nVerticesOnLine = nGrids + 1;

		var vertices = new Vector3[nVerticesOnLine * nVerticesOnLine];
		for (var y = 0; y <= nGrids; y++) {
			var baseIndex = y * nVerticesOnLine;
			for (var x = 0; x <= nGrids; x++) {
				var index = baseIndex + x;
				vertices[index] = new Vector3(x, y, 0f);
			}
		}
		
		var rUv = 1f / nGrids;
		var uvs = System.Array.ConvertAll(vertices, (v) => new Vector2(Mathf.Clamp01(v.x * rUv), Mathf.Clamp01(v.y * rUv)));
		vertices = System.Array.ConvertAll(uvs, (uv) => (Vector3)(uv - 0.5f * Vector2.one));
		
		var triangles = new int[6 * nGrids * nGrids];
		for (var y = 0; y < nGrids; y++) {
			var baseVertexIndex = y * nVerticesOnLine;
			var baseTriangleIndex = 6 * y * nGrids;
			for (var x = 0; x < nGrids; x++) {
				var vertexIndex = x + baseVertexIndex;
				var triangleIndex = 6 * x + baseTriangleIndex;
				triangles[triangleIndex] 	= vertexIndex;
				triangles[triangleIndex+1]	= vertexIndex + (nVerticesOnLine + 1);
				triangles[triangleIndex+2]	= vertexIndex + 1;
				triangles[triangleIndex+3]	= vertexIndex;
				triangles[triangleIndex+4]	= vertexIndex + nVerticesOnLine;
				triangles[triangleIndex+5]	= vertexIndex + (nVerticesOnLine + 1);
			}
		}
		
		var mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		
		AssetDatabase.CreateAsset(mesh, string.Format("Assets/Models/Generated/Plane{0}x{0}.asset", nGrids));
	}
}