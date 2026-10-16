using UnityEngine;

namespace AI.SteeringBehaviours
{
    public interface IAgent
    {
        float MaxSpeed { get; set; }
        float MaxForce { get; set; }
        Vector3 Position { get; set; }
        Vector3 Velocity { get; set; }
    }
}