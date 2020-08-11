using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Ability : UpdateAsStream
{
    public abstract bool IsAgressive { get; }
}
