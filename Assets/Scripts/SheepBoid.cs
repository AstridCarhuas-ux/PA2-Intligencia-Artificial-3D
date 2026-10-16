using System.Collections.Generic;
using UnityEngine;
using AI.SteeringBehaviours;

public class SheepBoid : MonoBehaviour, IAgent
{
    [Header("Parámetros del Agente")]
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float maxForce = 8f;

    public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
    public float MaxForce { get => maxForce; set => maxForce = value; }
    public Vector3 Position { get; set; }
    public Vector3 Velocity { get; set; }

    public bool IsInPen { get; private set; } = false;

    private ShepherdGameManager _manager;

    public void Setup(ShepherdGameManager manager, Vector3 initialPosition)
    {
        _manager = manager;
        Position = initialPosition;
        transform.position = Position;
    }

    void Update()
    {
        if (IsInPen)
        {
            Velocity = Vector3.Lerp(Velocity, Vector3.zero, Time.deltaTime * 5f);
            Position += Velocity * Time.deltaTime;
            transform.position = Position;
            return;
        }

        Vector3 totalForce = Vector3.zero;

        float distToPen = Vector3.Distance(new Vector3(Position.x, 0, Position.z), new Vector3(_manager.penTransform.position.x, 0, _manager.penTransform.position.z));
        if (distToPen <= _manager.penArriveRadius + 1.5f)
        {
            totalForce += SteeringBehaviours.Arrive(this, _manager.penTransform.position, _manager.penArriveRadius) * 2.0f;
            if (distToPen <= _manager.penArriveRadius)
            {
                IsInPen = true;
            }
        }
        else
        {
            float distToPlayer = Vector3.Distance(new Vector3(Position.x, 0, Position.z), new Vector3(_manager.playerTransform.position.x, 0, _manager.playerTransform.position.z));
            if (distToPlayer <= _manager.fleeRadius)
            {
                totalForce += SteeringBehaviours.Flee(this, _manager.playerTransform.position) * 2.5f;
            }

            Vector3 cohesion = SteeringBehaviours.Cohesion(this, _manager.Boids, _manager.neighborRadius);
            Vector3 separation = SteeringBehaviours.Separate(this, _manager.Boids, 1.5f);
            Vector3 alignment = SteeringBehaviours.Alignment(this, _manager.Boids, _manager.neighborRadius);

            totalForce += (cohesion * _manager.cohesionWeight) + (separation * _manager.separationWeight) + (alignment * _manager.alignmentWeight);

            totalForce += SteeringBehaviours.InsideCircle(this, Vector3.zero, _manager.worldRadius) * 2.0f;
        }

        Velocity += totalForce * Time.deltaTime;
        Velocity = Vector3.ClampMagnitude(Velocity, MaxSpeed);

        Velocity = new Vector3(Velocity.x, 0f, Velocity.z);
        Position += Velocity * Time.deltaTime;

        if (Velocity.sqrMagnitude > 0.001f)
        {
            transform.forward = Velocity.normalized;
        }

        transform.position = Position;
    }
}