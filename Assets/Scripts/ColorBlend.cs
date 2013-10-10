using UnityEngine;
using System.Collections;
using System.Linq;

public class ColorBlend : MonoBehaviour {
	public int kNumber = 1;
	public float modulation;
	public float modRadius;
	
	public ParticleData target;
	public ParticleData modulator;
	
	private KNN _targetKnn;

	// Use this for initialization
	void Start () {
		_targetKnn = new KNN();
	}
	
	// Update is called once per frame
	void Update () {
		ParticleSystem.Particle[] targetParticles;
		var nTargetParticles = target.GetParticles(out targetParticles);
		ParticleSystem.Particle[] modParticles;
		var nModParticles = modulator.GetParticles(out modParticles);
		if (nTargetParticles <= 0 || nModParticles <= 0)
			return;
		
		var points = targetParticles.Where((p, j) => j < nTargetParticles).Select(p => p.position).ToArray();
		_targetKnn.build(points, Enumerable.Range(0, points.Length).ToArray());
		
		var startColor = target.shuriken.startColor;
		for (var iTarget = 0; iTarget < nTargetParticles; iTarget++) {
			var color = targetParticles[iTarget].color;
			targetParticles[iTarget].color = new Color32(
				(byte)(color.r + 0.11f * startColor.r), 
				(byte)(color.g + 0.11f * startColor.g),
				(byte)(color.b + 0.11f * startColor.b),
				(byte)(color.a + 0.11f * startColor.a));
		}
		
		var sqrModRadius = modRadius * modRadius;
		for (int iMod = 0; iMod < nModParticles; iMod++) {
			var pMod = modParticles[iMod];
			var targetIndices = _targetKnn.knearest(pMod.position, kNumber);
			var color = pMod.color;
			foreach (var iTarget in targetIndices) {
				var pTarget = targetParticles[iTarget];
				if (sqrModRadius < (pTarget.position - pMod.position).sqrMagnitude)
					continue;
				pTarget.color = color;
				targetParticles[iTarget] = pTarget;
			}
		}
		
		target.SetParticles(targetParticles, nTargetParticles);
	}
}
