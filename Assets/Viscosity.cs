using UnityEngine;
using System.Collections;
using System.Linq;

public class Viscosity : MonoBehaviour {
	public ParticleData target;
	public float modulation;
	public float modRadius;
	public int kNumber;
	
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
		
		var sqrModRadius = modRadius * modRadius;
		for (int iMe = 0; iMe < nTargetParticles; iMe++) {
			var pMe = targetParticles[iMe];
			var particleIndices = _knn.knearest(pMe.position, kNumber);
			var count = 0;
			var sumVelocity = Vector3.zero;
			foreach (var iNeighbor in particleIndices) {
				if (iMe == iNeighbor)
					continue;
				var pNeighbor = targetParticles[iNeighbor];
				if (sqrModRadius < (pMe.position - pNeighbor.position).sqrMagnitude)
					continue;
				sumVelocity += pNeighbor.velocity;
				count++;
			}

			if (count == 0)
				continue;
			pMe.velocity = (1f - modulation) * pMe.velocity + (modulation / count) * sumVelocity;
			targetParticles[iMe] = pMe;
		}
		
		target.SetParticles(targetParticles, nTargetParticles);
	}
}
