using System;
using System.Collections.Generic;

namespace ArrayExtension
{
	public static class ArrayExtension
	{
		public static T[] Resize<T>(this T[] array, int size)
        {
			T[] newArray = new T[size];
			Array.Copy(array, newArray, array.Length);
			//array.CopyTo(newArray, 0);
			return newArray;
        }

		public static T[] Reverse<T>(this T[] array)
        {
			List<T> newList = new List<T>();
			newList.AddRange(array);
			newList.Reverse();
			return newList.ToArray();
        }
	}
}
