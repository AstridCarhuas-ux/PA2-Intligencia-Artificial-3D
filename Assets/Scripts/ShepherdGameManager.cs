using System.Collections.Generic;
using UnityEngine;
using AI.SteeringBehaviours;

public class ShepherdGameManager : MonoBehaviour
{
    [Header("Referencias de la Escena")]
    public GameObject sheepPrefab;
    public Transform playerTransform;
    public Transform penTransform;

    [Header("Configuración de Cantidad y Radios")]
    public int sheepCount = 10;
    public float spawnRadius = 8.0f;
    public float fleeRadius = 3.0f;
    public float penArriveRadius = 2.0f;
    public float worldRadius = 15.0f;
    public float neighborRadius = 5.0f;

    [Header("Pesos de Flocking")]
    public float cohesionWeight = 1.0f;
    public float separationWeight = 1.5f;
    public float alignmentWeight = 1.0f;

    public List<IAgent> Boids { get; private set; } = new List<IAgent>();
    private List<SheepBoid> sheepAgents = new List<SheepBoid>();

    void Start()
    {
        for (int i = 0; i < sheepCount; i++)
        {
            Vector2 randCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = new Vector3(randCircle.x, 0.5f, randCircle.y);

            GameObject sheepGO = Instantiate(sheepPrefab, spawnPos, Quaternion.identity);
            SheepBoid boid = sheepGO.GetComponent<SheepBoid>();

            Boids.Add(boid);
            sheepAgents.Add(boid);

            boid.Setup(this, spawnPos);
        }
    }

    void OnGUI()
    {
        int deliveredCount = 0;
        foreach (SheepBoid agent in sheepAgents)
        {
            if (agent.IsInPen) deliveredCount++;
        }

        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(20, 20, 400, 40), $"Ovejas en el Redil: {deliveredCount} / {sheepCount}", style);
    }
}