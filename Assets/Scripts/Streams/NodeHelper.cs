using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

namespace NodeExtensions
{
    public static class NodeHelper
    {
        public static
        Ctx GetContext<Ctx>(this MonoBehaviour self) where Ctx : MonoBehaviour, StreamContext, new()
        {
            Ctx context = self.GetComponentInParent<Ctx>();

            if (context == null)
            {
                Debug.LogError($"Missing context of type {typeof(Ctx)} in script attached to '{self.name}'.");

                try
                {
                    context = self.gameObject.AddComponent<Ctx>() as Ctx;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);

                    context = new Ctx();
                }
            }

            return context;
        }

        public static
        GameObject GetQuery(this MonoBehaviour self, string query)
        {
            foreach (var subQuery in query.Split(','))
                foreach (var gameObject in _Query(self.transform.root, subQuery))
                    return gameObject;

            Debug.LogWarning($"'{query}' did not match any GameObject inside {self.transform.root.name}.");

            return null;
        }

        public static
        List<GameObject> GetQueryAll(this MonoBehaviour self, string query)
        {
            var list = new List<GameObject> { };

            foreach (var subQuery in query.Split(','))
                foreach (var gameObject in _Query(self.transform.root, subQuery))
                    list.Add(gameObject);

            return list.Distinct().ToList();
        }

        static
        IEnumerable<GameObject> _Query(Transform root, string query)
        {
            var name = query.Trim().Split(' ')[0];

            var tag = name.StartsWith(".") ? name.Substring(1) : null;

            var queryTail = query.Trim().Substring(name.Length).Trim();

            // Breadth-first search over all gameObjects in scene.

            var queue = new List<(Transform, string)> { (root, queryTail) };

            while (queue.Count > 0)
            {
                var (node, tail) = queue[0];

                queue.RemoveAt(0);

                if (node.name == name || (tag != null && node.CompareTag(tag)))
                {
                    if (tail.Length == 0)
                    {
                        yield return node.gameObject;
                    }
                    else
                    {
                        name = tail.Trim().Split(' ')[0];

                        tag = name.StartsWith(".") ? name.Substring(1) : null;

                        tail = tail.Trim().Substring(name.Length).Trim();
                    }
                }

                foreach (Transform child in node)
                {
                    queue.Add((child, tail));
                }
            }
        }
    }
}
