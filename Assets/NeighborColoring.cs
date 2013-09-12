using UnityEngine;
using System.Collections;
using System.Linq;

public class NeighborColoring : MonoBehaviour {
	public int kNumber = 10;
	
	public ParticleData target;
	public Transform center;
	
	private KNN _knn;

	// Use this for initialization
	void Start () {
		_knn = new KNN();
	}
	
	// Update is called once per frame
	void Update () {
		ParticleSystem.Particle[] targetParticles;
		var nTargetParticles = target.GetParticles(out targetParticles);
		if (nTargetParticles <= 0)
			return;
		
		var points = targetParticles.Where((p, j) => j < nTargetParticles).Select(p => p.position).ToArray();
		_knn.build(points, Enumerable.Range(0, points.Length).ToArray());
		
		var targetIndices = _knn.knearest(center.position, kNumber);
		for (int i = 0; i < nTargetParticles; i++)
			targetParticles[i].color = Color.white;
		foreach (var iTarget in targetIndices)
			targetParticles[iTarget].color = Color.green;
		
		target.SetParticles(targetParticles, nTargetParticles);
	}
}
