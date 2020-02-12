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
        List<T> Lerp<T>(List<T> from, List<T> to, float t, Func<T, T, float, T> lerp)
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

        public static
        List<A> MapAtIndex<A>(this List<A> self, int index, Func<A, A> map)
        {
            var list = self.ConvertAll(a => a);

            if (index >= 0 && index < list.Count)
                list[index] = map(list[index]);

            return list;
        }

        public static
        List<B> Map<A, B>(this List<A> self, Func<A, B> map)
        {
            return self.ConvertAll(a => map(a));
        }

        public static
        List<C> Map2<A, B, C>(this List<A> self, List<B> other, Func<A, B, C> map)
        {
            var selfCount = self.Count;
            var otherCount = other.Count;
            var minCount = selfCount < otherCount ? selfCount : otherCount;

            var list = new List<C>(minCount);

            for (int i = 0; i < minCount; i++)
            {
                list.Add(map(self[i], other[i]));
            }

            return list;
        }

        public static
        List<A> Filter<A>(this List<A> self, Func<A, bool> filter)
        {
            var list = new List<A>();

            foreach (var item in self)
            {
                if (filter(item))
                    list.Add(item);
            }

            return list;
        }

        public static
        List<B> FilterMap<A, B>(this List<A> self, Func<A, Maybe<B>> filterMap)
        {
            return self
                .Map(filterMap)
                .Filter(x => x.ToBool())
                .Map(x => x.WithDefault(default(B)));
        }
    }
}

