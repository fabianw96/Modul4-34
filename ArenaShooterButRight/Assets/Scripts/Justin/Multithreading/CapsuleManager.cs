using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;


public class CapsuleManager : MonoBehaviour
{
    [SerializeField] private GameObject capsulePrefab;
    [SerializeField] private Transform player;
    [SerializeField] private float capsuleCount = 1000;
    [SerializeField] private bool useMultithreading;
    [SerializeField] private float capsuleMovespeed;
    [SerializeField] private List<GameObject> capsules;
    TransformAccessArray capsuleTransforms;
    private Vector3 playerPosition;

    void Start()
    {
        capsules = new List<GameObject>();
        SpawnCapsules();
    }

    void SpawnCapsules()
    {
        for (int i = 0; i < capsuleCount; i++)
        {
            Vector3 position = new Vector3(Random.Range(-100, 100), 0.5f, Random.Range(-100, 100));
            GameObject capsule = Instantiate(capsulePrefab, position, Quaternion.identity);
            capsules.Add(capsule);
        }

        Transform[] capsuleTransformArray = new Transform[capsules.Count];
        for (int i = 0; i < capsules.Count; i++)
        {
            capsuleTransformArray[i] = capsules[i].transform;
        }
        capsuleTransforms = new TransformAccessArray(capsuleTransformArray);
    }


    void Update()
    {
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
        foreach (var capsule in capsules)
        {
            Vector3 direction = (player.position - capsule.transform.position).normalized * capsuleMovespeed;
            capsule.transform.position += direction * Time.deltaTime;
        }
    }

    void OnDestroy()
    {
        // TransformAccessArrays must be disposed manually.
        capsuleTransforms.Dispose();
    }

    private void MoveWithMultiThreading()
    {
        playerPosition = player.position;

        MoveJob moveJob = new MoveJob
        {
            playerPosition = playerPosition,
            deltaTime = Time.deltaTime,
            speed = capsuleMovespeed,
        };
        JobHandle jobHandle = moveJob.Schedule(capsuleTransforms);
        
        jobHandle.Complete();
    }


    [BurstCompile]
    struct MoveJob : IJobParallelForTransform
    {
        public Vector3 playerPosition;
        public float deltaTime;
        public float speed;

        public void Execute(int index,TransformAccess transform)
        {
            Vector3 direction = math.normalize(playerPosition - transform.position);
            transform.position += direction * deltaTime * speed;
        }
    }
}
