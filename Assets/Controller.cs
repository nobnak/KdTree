using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class Controller : MonoBehaviour {
	public int kNumber = 10;
	public Transform target;
	
	private ParticleSystem _system;
	private ParticleSystem.Particle[] _particles;
	private KNN _kNN;
	private List<int> _lines;

	// Use this for initialization
	void Start () {
		_system = GetComponentInChildren<ParticleSystem>();
		_particles = new ParticleSystem.Particle[0];
		_kNN = new KNN();
		_lines = new List<int>();
	}
	
	// Update is called once per frame
	void Update () {
		if (_particles.Length < _system.particleCount)
			_particles = new ParticleSystem.Particle[(int)(_system.particleCount * 1.5)];
		var nParticles = _system.GetParticles(_particles);
		if (nParticles <= 0)
			return;

		var points = _particles.Where((p, j) => j < nParticles).Select(p => p.position).ToArray();
		_kNN.build(points, Enumerable.Range(0, points.Length).ToArray());
		
		_lines.Clear();
		for (int iSelf = 0; iSelf < nParticles; iSelf++) {
			var iNeighbors = _kNN.knearest(_particles[iSelf].position, kNumber);
			foreach (var iNgh in iNeighbors) {
				if (iNgh <= iSelf)
					continue;
				_lines.Add(iSelf);
				_lines.Add(iNgh);
			}
		}
		
		for (int i = 0; i < nParticles; i++) {
			_particles[i].color = Color.white;
		}
		var pointIndices = _kNN.knearest(target.position, kNumber);
		foreach (int iPoint in pointIndices) {
			_particles[iPoint].color = Color.green;
		}
		
		//Debug.Log("nParticle=" + nParticles);
		//benchmarkKNN(points, nSearchings, kNumber);
		//benchmarkKdTree(points, nSearchings);
		//testKNN(points);
		//testKdTree(points, nIterations);
		//testPriorityQueue();
		
		_system.SetParticles(_particles, nParticles);
	}
	

	public void testKNN(Vector3[] points, int nIterations) {
		var _knn = new KNN();
		var clone = new Vector3[points.Length]; points.CopyTo(clone, 0);
		_knn.build(clone, Enumerable.Range(0, points.Length).ToArray());
		for (int j = 0; j < nIterations; j++) {
			var randPos = 10f * new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f);
			var sortedIncides = Enumerable.Range(0, points.Length).ToArray();
			System.Array.Sort(System.Array.ConvertAll(points, p => (p - randPos).sqrMagnitude), sortedIncides);		
			
			var iNearests = _knn.knearest(randPos, points.Length < 4 ? points.Length : 4);
			var knnIsCorrect = true;
			for (int i = 0; i < iNearests.Length; i++) {
				if (sortedIncides[i] != iNearests[i]) {
					knnIsCorrect = false;
					break;
				}
			}
			if (!knnIsCorrect) {
				Debug.Log("[KNN] Something wrong");
				
			}
		}
	}	
	public void benchmarkKNN(Vector3[] points, int nIterations, int kNumber) {
		var _knn = new KNN();
		var clone = new Vector3[points.Length]; points.CopyTo(clone, 0);
		_knn.build(clone, Enumerable.Range(0, points.Length).ToArray());
		var scale = 10f;
		var randPosList = (from i in Enumerable.Range(0, nIterations) 
			select new Vector3(scale * (Random.value - 0.5f), scale * (Random.value - 0.5f), scale * (Random.value - 0.5f))).ToArray();		

		for (int j = 0; j < nIterations; j++)
			_knn.knearest(randPosList[j], kNumber);
	}

	
	public void testKdTree(Vector3[] points, int nIterations) {
		var kdTree = new KdTree();
		var clone = new Vector3[points.Length]; points.CopyTo(clone, 0);
		kdTree.build(clone, Enumerable.Range(0, points.Length).ToArray());
		for (int i = 0; i < nIterations; i++) {
			var randPos = 10f * new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f);
			var sortedIncides = Enumerable.Range(0, points.Length).ToArray();
			System.Array.Sort(System.Array.ConvertAll(points, p => (p - randPos).sqrMagnitude), sortedIncides);		
			
			var iNearest = kdTree.nearest(randPos);
			if (iNearest != sortedIncides[0])
				Debug.Log("[Kd-tree] Something wrong");
		}
	}	
	public void benchmarkKdTree(Vector3[] points, int nIterations) {
		var kdTree = new KdTree();
		var clone = new Vector3[points.Length]; points.CopyTo(clone, 0);
		kdTree.build(clone, Enumerable.Range(0, points.Length).ToArray());
		var scale = 10f;
		var randPosList = (from i in Enumerable.Range(0, nIterations) 
			select new Vector3(scale * (Random.value - 0.5f), scale * (Random.value - 0.5f), scale * (Random.value - 0.5f))).ToArray();

		for (int i = 0; i < nIterations; i++)
			kdTree.nearest(randPosList[i]);
	}
	
	
		
	public void testPriorityQueue() {
		var priQueue = new PriorityQueue<SimpleNode>();
		var randPosList = new List<SimpleNode>();
		for (int i = 0; i < 10; i++) {
			var node = new SimpleNode(){ priority = Random.value * 100f };
			randPosList.Add(node);
			priQueue.enqueue(node);
		}
		randPosList.Sort();
		for (int i = randPosList.Count-1; i >= 0; i--) {
			var node = randPosList[i];
			randPosList.RemoveAt(i);
			if (node.id != priQueue.dequeue().id)
				Debug.Log("[PriorityQueue] Something wrong");
		}
	}
	
	public class SimpleNode : System.IComparable<SimpleNode> {
		public int id;
		public float priority;
		
		public SimpleNode() {
			id = idAccum++;
		}
		public int CompareTo(SimpleNode friend) {
			return priority > friend.priority ? 1 : (priority < friend.priority ? -1 : 0);
		}
		public static int idAccum = 0;
	}
}


