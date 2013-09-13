using UnityEngine;
using System.Collections;
using System.Linq;

public class NeighborColoring : MonoBehaviour {
	public ParticleData target;
	public Center[] centers;
	
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
		
		for (int i = 0; i < nTargetParticles; i++)
			targetParticles[i].color = Color.white;
		foreach (var c in centers) {
			var targetIndices = _knn.knearest(c.pos.position, c.nFriends);
			foreach (var iTarget in targetIndices)
				targetParticles[iTarget].color = c.color;
		}
		
		target.SetParticles(targetParticles, nTargetParticles);
	}
}

[System.Serializable]
public class Center {
	public Transform pos;
	public int nFriends;
	public Color color;
}
