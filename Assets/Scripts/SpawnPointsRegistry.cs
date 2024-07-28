using System.Collections.Generic;
using UnityEngine;

public class SpawnPointsRegistry : MonoBehaviour
{
    public static SpawnPointsRegistry Instance { get; private set; }

    private readonly List<Vector3> _spawnPoints = new();

    private void Awake()
    {
        Instance = this;
        PopulateSpawnPoints();
    }

    public Vector3 GetRandomSpawnPoint()
    {
        int index = Random.Range(0, _spawnPoints.Count);
        return _spawnPoints[index];
    }

    private void PopulateSpawnPoints()
    {
        foreach (Transform child in transform) _spawnPoints.Add(child.position);
    }
}
