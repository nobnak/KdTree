using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nobnak.Extension;

public class KNN2 {
	public Entity[] points { get; private set; }
	private Point _root;
	private FixedSizePriorityQueue<PriorityPoint> _priQueue;
	
	public void build(Entity[] points) {
		this.points = points;
		_root = build(0, points.Length, 0);
		_priQueue = new FixedSizePriorityQueue<PriorityPoint>(0);
	}
	private Point build(int offset, int length, int depth) {
		if (length == 0)
			return null;
		int axis = depth % 3;
		System.Array.Sort(points, offset, length, COMPS[axis]);
		int mid = length >> 1;
		return new Point(){ mid = offset + mid,
			smaller = build(offset, mid, depth+1), 
			larger = build(offset+mid+1, length - (mid+1), depth+1) };
	}
	
	public int[] knearest(Vector3 point, int k) {
		_priQueue.reset();
		_priQueue.resize(k);
		_priQueue.enqueue(new PriorityPoint(-1, float.PositiveInfinity));
		knearest(point, _root, 0);
		return (from node in _priQueue.Reverse() where node.ipos >= 0 select points[node.ipos].id).ToArray();
	}
	private void knearest(Vector3 point, Point p, int depth) {
		if (p == null)
			return;
		var axis = depth % 3;
		var distOnAxis = points[p.mid].pos[axis] - point[axis];
		if (distOnAxis > 0) {
			knearest(point, p.smaller, depth+1);
			var sqDist2leaf = sqDist (point, _priQueue.head().ipos);
			if (sqDist2leaf > distOnAxis * distOnAxis)
				knearest(point, p.larger, depth+1);
		} else {
			knearest(point, p.larger, depth+1);
			var sqDist2leaf = sqDist(point, _priQueue.head().ipos);
			if (sqDist2leaf > distOnAxis * distOnAxis)
				knearest(point, p.smaller, depth+1);
		}
		_priQueue.enqueue(new PriorityPoint(p.mid, sqDist(point, p.mid)));
	}
	private float sqDist(Vector3 point, int index) {
		if (index == -1)
			return float.PositiveInfinity;
		var dist = point - points[index].pos;
		return dist.sqrMagnitude;
	}
	private int closer(Vector3 point, int i0, int i1) {
		if (i0 == -1)
			return i1;
		else if (i1 == -1)
			return i0;
		return sqDist(point, i0) < sqDist(point, i1) ? i0 : i1;
	}
	
	private class Point {
		public int mid;
		public Point smaller;
		public Point larger;
	}

	private static readonly IComparer<Entity>[] COMPS;
	static KNN2() {
		COMPS = new IComparer<Entity>[]{ new AxisComparer(0), new AxisComparer(1), new AxisComparer(2)	};
	}
	private class AxisComparer : IComparer<Entity> {
		private int _axis;
		public AxisComparer(int axis) {
			_axis = axis;
		}
		public int Compare(Entity p0, Entity p1) {
			return p0.pos[_axis] > p1.pos[_axis] ? +1 : (p0.pos[_axis] < p1.pos[_axis] ? -1 : 0);
		}
	}
	private class PriorityPoint : System.IComparable<PriorityPoint> {
		public readonly int ipos;
		public readonly float dist;
		
		public PriorityPoint(int ipos, float dist) {
			this.ipos = ipos;
			this.dist = dist;
		}

		public int CompareTo (PriorityPoint other) {
			return dist > other.dist ? +1 : (dist < other.dist ? -1 : 0);
		}
		
		public override string ToString ()
		{
			return string.Format("(i={0},d={1})", ipos, dist);
		}
	}
	
	public class Entity {
		public Vector3 pos;
		public int id;
		
		public Entity(Vector3 pos, int id) {
			this.pos = pos;
			this.id = id;
		}
	}
}