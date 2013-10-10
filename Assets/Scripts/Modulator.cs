using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class Modulator : MonoBehaviour {
	public int kNumber = 10;
	public float modulation;
	public float modRadius;
	
	public ParticleData target;
	public ParticleData modulator;
	
	private KNN _modulatorKnn;

	// Use this for initialization
	void Start () {
		_modulatorKnn = new KNN();
	}
	
	// Update is called once per frame
	void Update () {
		ParticleSystem.Particle[] targetParticles;
		var nTargetParticles = target.GetParticles(out targetParticles);
		
		ParticleSystem.Particle[] modParticles;
		var nModParticles = modulator.GetParticles(out modParticles);
		if (nTargetParticles <= 0 || nModParticles <= 0)
			return;
		
		var points = modParticles.Where((p, j) => j < nModParticles).Select(p => p.position).ToArray();
		_modulatorKnn.build(points, Enumerable.Range(0, points.Length).ToArray());
		
		var sqrModRadius = modRadius * modRadius;
		for (int iTarget = 0; iTarget < nTargetParticles; iTarget++) {
			var pTarget = targetParticles[iTarget];
			var modIndices = _modulatorKnn.knearest(pTarget.position, kNumber);
			var count = 0;
			var sumVelocity = Vector3.zero;
			foreach (var iMod in modIndices) {
				var pMod = modParticles[iMod];
				if (sqrModRadius < (pTarget.position - pMod.position).sqrMagnitude)
					continue;
				sumVelocity += modParticles[iMod].velocity;
				count++;
			}
#if true
			if (count == 0)
				continue;
			pTarget.velocity = (1f - modulation) * pTarget.velocity + modulation * sumVelocity / count;
			targetParticles[iTarget] = pTarget;
#endif
		}
		
		target.SetParticles(targetParticles, nTargetParticles);
	}
}
