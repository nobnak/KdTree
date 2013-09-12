using UnityEngine;
using System.Collections;

[System.Serializable]
public class ParticleData {
	public ParticleSystem shuriken;
	
	private ParticleSystem.Particle[] _particles;
	
	public ParticleData() {
		_particles = new ParticleSystem.Particle[0];
	}
	
	public int GetParticles(out ParticleSystem.Particle[] particles) {
		if (_particles.Length < shuriken.particleCount)
			_particles = new ParticleSystem.Particle[(int)(shuriken.particleCount * 1.5)];
		var nParticles = shuriken.GetParticles(_particles);
		particles = _particles;
		return nParticles;
	}
	public void SetParticles(ParticleSystem.Particle[] particles, int nParticles) {
		shuriken.SetParticles(particles, nParticles);
	}
}