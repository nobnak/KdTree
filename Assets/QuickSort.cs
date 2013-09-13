using System.Collections.Generic;

namespace Nobnak.Extension {
	public static class Sort {
		public static void QuickSort<T>(this T[] list, IComparer<T> comp) {
			QuickSort(list, comp, 0, list.Length - 1);
		}
		
		private static void QuickSort<T>(this T[] list, IComparer<T> comp, int left, int right) {
			if (right <= left)
				return;
			
			var i = left;
			var j = right;
			var pivot = med3(list[i], list[i + (j-i)/2], list[j], comp);
			while (true) {
				while (comp.Compare(list[i++], pivot) < 0) {}
				while (comp.Compare(pivot, list[j--]) < 0) {}
				if (j <= i)
					break;
				var tmp = list[i]; list[i] = list[j]; list[j] = tmp;
				i++; j--;
			}
			QuickSort(list, comp, left, i - 1);
			QuickSort(list, comp, j + 1, right);
		}
		
		public static T med3<T>(T x, T y, T z, IComparer<T> comp) {
			if (comp.Compare(x, y) < 0) {
				if (comp.Compare(y, z) < 0)
					return y;
				else if (comp.Compare(x, z) < 0)
					return z;
				return x;					
			}
			if (comp.Compare(x, z) < 0)
				return x;
			else if (comp.Compare(y, z) < 0)
				return z;
			return y;
		}
	}
}
