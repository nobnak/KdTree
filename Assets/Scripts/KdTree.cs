using UnityEngine;
using System.Collections.Generic;

public class KdTree {
	public Vector3[] points { get; private set; }
	public int[] ids { get; private set; }
	private Point _root;
	
	public void build(Vector3[] points, int[] ids) {
		this.points = points;
		this.ids = ids;
		_root = build(0, points.Length, 0);
	}
	private Point build(int offset, int length, int depth) {
		if (length == 0)
			return null;
		int axis = depth % 3;
		System.Array.Sort<Vector3, int>(points, ids, offset, length, COMPS[axis]);
		int mid = length >> 1;
		return new Point(){ mid = offset + mid,
			smaller = build(offset, mid, depth+1), 
			larger = build(offset+mid+1, length - (mid+1), depth+1) };
	}
	
	public int nearest(Vector3 point) {
		return ids[nearest(point, _root, 0)];
	}
	private int nearest(Vector3 point, Point p, int depth) {
		if (p == null)
			return -1;
		var axis = depth % 3;
		int leaf;
		var dist2mid = points[p.mid][axis] - point[axis];
		if (dist2mid > 0) {
			leaf = nearest(point, p.smaller, depth+1);
			var sqDist2leaf = sqDist (point, leaf);
			if (sqDist2leaf > dist2mid * dist2mid)
				leaf = closer(point, leaf, nearest(point, p.larger, depth+1));
		} else {
			leaf = nearest(point, p.larger, depth+1);
			var sqDist2leaf = sqDist(point, leaf);
			if (sqDist2leaf > dist2mid * dist2mid)
				leaf = closer(point, leaf, nearest(point, p.smaller, depth+1));
		}
		return closer(point, leaf, p.mid);
	}
	private float sqDist(Vector3 point, int index) {
		if (index == -1)
			return float.PositiveInfinity;
		var dist = point - points[index];
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

	private static readonly IComparer<Vector3>[] COMPS;
	static KdTree() {
		COMPS = new IComparer<Vector3>[]{ new AxisComparer(0), new AxisComparer(1), new AxisComparer(2)	};
	}
	private class AxisComparer : IComparer<Vector3> {
		private int _axis;
		public AxisComparer(int axis) {
			_axis = axis;
		}
		public int Compare(Vector3 p0, Vector3 p1) {
			return p0[_axis] > p1[_axis] ? +1 : (p0[_axis] < p1[_axis] ? -1 : 0);
		}
	}
}