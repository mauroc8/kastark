using UnityEngine;

public struct Query
{
    private GameObject host;

    public static
    Query From(Component component, string query)
    {
        return new Query
        {
            host =
                Node.Query(component, query)
        };
    }

    public static
    Query From(GameObject gameObject, string query)
    {
        return new Query
        {
            host =
                Node.Query(gameObject, query)
        };
    }

    public static
    Query From(Component component)
    {
        return new Query
        {
            host =
                component.gameObject
        };
    }

    public static
    Query From(GameObject gameObject)
    {
        return new Query
        {
            host =
                gameObject
        };
    }

    public GameObject Get()
    {
        return host;
    }

    public C Get<C>() where C : Component
    {
        var c = host.GetComponentInChildren<C>(true);

        if (c == null)
        {
            Debug.LogError($"The object {host.name} does not have a child of type {typeof(C)}");
            return default(C);
        }

        return c;
    }

    public C[] GetAll<C>() where C : Component
    {
        return host.GetComponentsInChildren<C>(true);
    }

    public Query Child(string query)
    {
        return new Query
        {
            host =
                Node.Query(this.host, query)
        };
    }

    public Query Child<C>() where C : Component
    {
        var c = host.GetComponentInChildren<C>(true);

        if (c == null)
        {
            Debug.LogError($"The component {typeof(C)} is not a child of {host.name}.");
            return this;
        }

        return new Query
        {
            host =
                c.gameObject
        };
    }
}

