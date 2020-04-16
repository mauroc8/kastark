using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public static class Node
{
    public static
    TComponent Component<TComponent>(Component component) where TComponent : Component, new()
    {
        return Component<TComponent>(component.gameObject);
    }

    public static
    TComponent Component<TComponent>(GameObject gameObject) where TComponent : Component, new()
    {
        var component = gameObject.GetComponent<TComponent>();

        if (component != null)
            return component;

        Debug.LogError($"Missing component of type {typeof(TComponent)} " +
            $"in script attached to '{component.name}'.");

        try
        {
            return gameObject.AddComponent<TComponent>() as TComponent;
        }
        catch (Exception e)
        {
            Debug.LogException(e);

            return new TComponent();
        }
    }

    public static
    GameObject Query(Component component, string query)
    {
        return Query(component.transform, query);
    }

    public static
    GameObject Query(GameObject gameObject, string query)
    {
        return Query(gameObject.transform, query);
    }

    public static
    GameObject Query(Transform transform, string query)
    {
        foreach (var subQuery in query.Split(','))
            foreach (var gameObject in _Query(transform, subQuery))
                return gameObject;

        Debug.LogWarning($"'{query}' did not match any GameObject child of '{transform.name}'.");

        return new GameObject("query failed: " + query);
    }

    public static
    Optional<GameObject> TryGetQuery(Transform transform, string query)
    {
        foreach (var subQuery in query.Split(','))
            foreach (var gameObject in _Query(transform, subQuery))
                return Optional.Some(gameObject);

        return Optional.None<GameObject>();
    }

    public static
    List<GameObject> QueryAll(this Component self, string query)
    {
        var list = new List<GameObject> { };

        foreach (var subQuery in query.Split(','))
            foreach (var gameObject in _Query(self.transform, subQuery))
                list.Add(gameObject);

        return list.Distinct().ToList();
    }

    static
    IEnumerable<GameObject> _Query(Transform transform, string query)
    {
        var name = query.Trim().Split(' ')[0];

        var tag = name.StartsWith(".") ? name.Substring(1) : null;

        var queryTail = query.Trim().Substring(name.Length).Trim();

        // Breadth-first search over all gameObjects in scene.

        var queue = new List<(Transform, string)> { (transform, queryTail) };

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
