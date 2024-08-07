using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;

public class CapsuleMovement : MonoBehaviour
{
    [SerializeField] private Transform player;
    private NativeArray<float3> capsulePositions;
    private NativeArray<float3> playerPosition;

    void Start()
    {
        // Get all capsules
        GameObject[] capsules = GameObject.FindGameObjectsWithTag("Capsule");
        capsulePositions = new NativeArray<float3>(capsules.Length, Allocator.Persistent);

        for (int i = 0; i < capsules.Length; i++)
        {
            capsulePositions[i] = capsules[i].transform.position;
        }

        playerPosition = new NativeArray<float3>(1, Allocator.Persistent);
        playerPosition[0] = player.position;
    }

    void Update()
    { 

        // Schedule the job
        MoveJob moveJob = new MoveJob
        {
            capsulePositions = capsulePositions,
            playerPosition = playerPosition[0],
            deltaTime = Time.deltaTime
        };

        JobHandle jobHandle = moveJob.Schedule(capsulePositions.Length, 100);
        jobHandle.Complete();

        // Apply the positions back to the capsules
        for (int i = 0; i < capsulePositions.Length; i++)
        {
            transform.position = capsulePositions[i];
        }
    }

    void OnDestroy()
    {
        capsulePositions.Dispose();
        playerPosition.Dispose();
    }

    [BurstCompile]
    struct MoveJob : IJobParallelFor
    {
        public NativeArray<float3> capsulePositions;
        public float3 playerPosition;
        public float deltaTime;

        public void Execute(int index)
        {
            float3 direction = math.normalize(playerPosition - capsulePositions[index]);
            capsulePositions[index] += direction * deltaTime;
        }
    }
}
