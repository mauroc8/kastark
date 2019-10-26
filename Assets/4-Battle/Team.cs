using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Team : MonoBehaviour
{
    [SerializeField] List<Creature> _creatures;
    
    public List<Creature> Creatures => _creatures;
    public bool IsAlive => _creatures.Any(creature => creature.IsAlive);
}