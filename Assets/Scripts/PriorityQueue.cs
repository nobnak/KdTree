using UnityEngine;
using System.Collections.Generic;


public class PriorityQueue<T> : IEnumerable<T> where T : System.IComparable<T> {
	public List<T> queue { get; private set; }
	
	public PriorityQueue() {
		reset ();
	}
	
	public void reset() {
		queue = new List<T>();
	}
	
	public virtual void enqueue(T newNode) {
		int iNewNode = queue.Count;
		queue.Add(newNode);
		
		int iParent = parent(iNewNode);
		while (iNewNode != ROOT_INDEX && newNode.CompareTo(queue[iParent]) > 0) {
			T tmpNode = queue[iParent]; queue[iParent] = newNode; queue[iNewNode] = tmpNode;
			iNewNode = iParent; iParent = parent(iNewNode);
		}
	}
	
	public T dequeue() {
		int iParent = ROOT_INDEX;
		int n = queue.Count - 1;
		T result = queue[iParent];
		queue[iParent] = queue[n];
		queue.RemoveAt(n);
		
		int iChild = left (iParent);
		while (iChild < n) {
			int iRight = iChild + 1;
			if (iRight < n && queue[iRight].CompareTo(queue[iChild]) > 0)
				iChild = iRight;
			if (queue[iChild].CompareTo(queue[iParent]) > 0) {
				T tmpNode = queue[iChild]; queue[iChild] = queue[iParent]; queue[iParent] = tmpNode;
			}
			iParent = iChild; iChild = left (iParent);
		}
		return result;
	}
	
	public virtual void resize(int count) {
		while (queue.Count > count)
			dequeue();
	}
	public int count() { return queue.Count; }
	
	public T head() { return queue[0]; }
	
	public const int ROOT_INDEX = 0;
	public static int parent(int iChild) {
		return (iChild-1) >> 1;
	}
	public static int left(int iParent) {
		return 2*iParent+1;
	}

	#region IEnumerable[T] implementation
	public IEnumerator<T> GetEnumerator () {
		while (queue.Count > 0)
			yield return dequeue();
	}
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () {
		while (queue.Count > 0)
			yield return dequeue();
	}
	#endregion
}

public class FixedSizePriorityQueue<T> : IEnumerable<T> where T : System.IComparable<T> {
	private T[] _queue;
	private int _count;
	
	public FixedSizePriorityQueue(int size) {
		_queue = new T[size];
		_count = 0;
	}
	public void enqueue(T newNode) {
		var iInsert = 0;
		for (; iInsert < _count; iInsert++) {
			if (newNode.CompareTo(_queue[iInsert]) >= 0)
				break;
		}
		if (_count < _queue.Length) {
			var movementLength = _count - iInsert;
			if (movementLength > 0)
				System.Array.Copy(_queue, iInsert, _queue, iInsert+1, movementLength);
			_count++;
		} else if (iInsert-- == 0) {
			return;
		} else {
			var movementLength = iInsert;
			if (movementLength > 0)
				System.Array.Copy(_queue, 1, _queue, 0, movementLength);
		}
		_queue[iInsert] = newNode;
	}
	public T head() { 
		return _queue[0]; 
	}
	public void reset() { 
		_count = 0; 
	}
	public void resize(int size) {
		if (_queue.Length != size)
			_queue = new T[size];
	}

	#region IEnumerable[T] implementation
	public IEnumerator<T> GetEnumerator () {
		for (int i = 0; i < _count; i++)
			yield return _queue[i];
	}
	#endregion

	#region IEnumerable implementation
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () {
		for (int i = 0; i < _count; i++)
			yield return _queue[i];
	}
	#endregion
}