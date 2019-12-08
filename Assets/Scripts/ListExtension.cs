using System;
using System.Collections.Generic;
using System.Linq;

namespace ListExtensions
{
    public static class ListExtension
    {
        public static
        List<T> Repeat<T>(T value, int times) =>
            Enumerable.Repeat(value, times).ToList();

        public static
        List<T> Lerp<T>(this List<T> from, List<T> to, float t, Func<T, T, float, T> lerp)
        {
            var result = new List<T>();

            for (int i = 0; i < from.Count; i++)
                result.Add(lerp(from[i], to[i], t));

            return result;
        }

        public static
        bool MoveElementDown<T>(this List<T> list, int index)
        {
            if (index < 0 || index + 1 >= list.Count)
                return false;

            T value = list[index];
            list.RemoveAt(index);
            list.Insert(index + 1, value);

            return true;
        }

        public static
        bool MoveElementDown<T>(this List<T> list, T elem)
        {
            var index = list.IndexOf(elem);
            return list.MoveElementDown(index);
        }
    }
}

