using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

/// <summary>
/// Manages a large number of capsule GameObjects, moving them towards the player.
/// Supports both single-threaded and multithreaded movement calculations using Unity's Job System.
/// Designed to compare the performance impact of multithreading on capsule movement in a Unity scene.
/// </summary>
public class CapsuleManager : MonoBehaviour
{
    [SerializeField] private GameObject capsulePrefab;
    [SerializeField] private Transform player;
    [SerializeField] private float capsuleCount = 1000;
    [SerializeField] private bool useMultithreading;
    [SerializeField] private float capsuleMovespeed;
    [SerializeField] private List<GameObject> capsules;

    // Array of transforms for all capsules; used for multithreading
    private TransformAccessArray capsuleTransforms;
    // Cached player position for use in the multithreaded job
    private Vector3 playerPosition;

    void Start()
    {
        capsules = new List<GameObject>();
        SpawnCapsules();
    }

    void SpawnCapsules()
    {
        // Loop to spawn each capsule at a random position within a defined range
        for (int i = 0; i < capsuleCount; i++)
        {
            Vector3 position = new Vector3(Random.Range(-100, 100), 0.5f, Random.Range(-100, 100));
            GameObject capsule = Instantiate(capsulePrefab, position, Quaternion.identity);
            capsules.Add(capsule);
        }

        // Initialize the TransformAccessArray for multithreading
        Transform[] capsuleTransformArray = new Transform[capsules.Count];
        for (int i = 0; i < capsules.Count; i++)
        {
            capsuleTransformArray[i] = capsules[i].transform;
        }
        capsuleTransforms = new TransformAccessArray(capsuleTransformArray);
    }


    void Update()
    {
        // Check if multithreading should be used, and move capsules accordingly
        if (useMultithreading)
        {
            MoveWithMultiThreading();
        }
        else
        {
            MoveWithoutMultithreading();
        }
    }

    private void MoveWithoutMultithreading()
    {
        // Move each capsule towards the player's position without using multithreading
        foreach (var capsule in capsules)
        {
            Vector3 direction = (player.position - capsule.transform.position).normalized * capsuleMovespeed;
            capsule.transform.position += direction * Time.deltaTime;
        }
    }

    void OnDestroy()
    {
        // Ensure that the TransformAccessArray is disposed when the object is destroyed to prevent memory leaks
        capsuleTransforms.Dispose();
    }

    private void MoveWithMultiThreading()
    {
        // Cache the player's position to avoid accessing it repeatedly in the job
        playerPosition = player.position;

        // Create and configure a new job to move the capsules
        MoveJob moveJob = new MoveJob
        {
            playerPosition = playerPosition,
            deltaTime = Time.deltaTime,
            speed = capsuleMovespeed,
        };

        // Schedule the job using the TransformAccessArray, then wait for it to complete
        JobHandle jobHandle = moveJob.Schedule(capsuleTransforms);
        jobHandle.Complete(); // Ensure the job is completed before proceeding
    }


    [BurstCompile]
    struct MoveJob : IJobParallelForTransform
    {
        // Job data that will be used in parallel across multiple threads
        public Vector3 playerPosition;
        public float deltaTime;
        public float speed;

        public void Execute(int index,TransformAccess transform)
        {
            // Calculate the direction towards the player and move the capsule
            Vector3 direction = math.normalize(playerPosition - transform.position);
            transform.position += direction * deltaTime * speed;
        }
    }
}
