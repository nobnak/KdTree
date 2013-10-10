using UnityEngine;
using System.Collections;
using System.Linq;

public class ColorRegistration : MonoBehaviour {
	public MeshFilter model;
	public ParticleData smoke;
	public float coloringDist;
	
	public Vector3[] positions;
	public Vector3[] normals;
	public Color[] colors;
	
	private KdTree _tree;
	private Bounds _bounds;

	// Use this for initialization
	void Start () {
		var mesh = model.sharedMesh;
		positions = mesh.vertices;
		normals = mesh.normals;
		var mat = model.renderer.sharedMaterial;
		var tex = (Texture2D)mat.mainTexture;
		colors = System.Array.ConvertAll(mesh.uv, (uv) => tex.GetPixelBilinear(uv.x, uv.y));
		
		_tree = new KdTree();
		_tree.build(positions, Enumerable.Range(0, mesh.vertexCount).ToArray());
		
		_bounds = model.renderer.bounds;
		_bounds.Expand(2f * coloringDist);
	}
	
	// Update is called once per frame
	void Update () {
		ParticleSystem.Particle[] particles;
		var particleCount = smoke.GetParticles(out particles);
		var sqrDist = coloringDist * coloringDist;
		var startColor = smoke.shuriken.startColor;
				
		for (var i = 0; i < particleCount; i++) {
			var particle = particles[i];
			particle.color = startColor;
			if (_bounds.Contains(particle.position)) {
				var positionLocal = model.transform.InverseTransformPoint(particle.position);
				var iNearest = _tree.nearest(positionLocal);
				if (iNearest >= 0) {
					var toMesh = positions[iNearest] - positionLocal;
					if (toMesh.sqrMagnitude < sqrDist)
						particle.color = colors[iNearest];
				}
			}
			particles[i] = particle;
		}
		smoke.SetParticles(particles, particleCount);
	}
}
