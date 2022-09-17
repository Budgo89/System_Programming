using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using static Lesson2;

public class Lesson2 : MonoBehaviour, IDisposable
{
    [SerializeField] private int _speed = 2;
    [SerializeField] private GameObject _gameObject;
    private TransformAccessArray _transformAccess;
    private bool isTask3 = false;
    private float angles = 1;

    private NativeArray<int> array;

    public NativeArray<Vector3> Positions;
    public NativeArray<Vector3> Velocities;
    public NativeArray<Vector3> FinalPositions;

    public void StartMyJob()
    {
        array = new NativeArray<int>(new int[] { 1, 6, 8, 10, 3 }, Allocator.Persistent);
        MyJob MyJob1 = new MyJob()
        {
            arrays = array
        };

        JobHandle jobHandle = MyJob1.Schedule();
        jobHandle.Complete();
        for (int i = 0; i < array.Length; i++)
        {
            Debug.Log(array[i]);
        }
        array.Dispose();
    }

    public void StartMyIJobParallelFor()
    {
        Positions = new NativeArray<Vector3>(
            new Vector3[] { new Vector3(1, 1, 1), new Vector3(2, 2, 2), new Vector3(3, 3, 3) }, Allocator.Persistent);
        Velocities = new NativeArray<Vector3>(
            new Vector3[] { new Vector3(-1, -1,- 1), new Vector3(-2, -2, -2), new Vector3(-3, -3, -3) }, Allocator.Persistent);
        FinalPositions = new NativeArray<Vector3>(new Vector3[Positions.Length],Allocator.Persistent);
        MyIJobParallelFor MyIJobParallelFor1 = new MyIJobParallelFor()
        {
            Positions = Positions,
            Velocities = Velocities,
            FinalPositions = FinalPositions,
        };
        
        if (Positions.Length == Velocities.Length)
        {
            JobHandle jobHandle;
            jobHandle = MyIJobParallelFor1.Schedule(Positions.Length,0);
            jobHandle.Complete();
        }
        
        foreach (var finalPosition in FinalPositions)
        {
            Debug.Log(finalPosition);
        }
        Positions.Dispose();
        Velocities.Dispose();
        FinalPositions.Dispose();
    }

    public void StartMyJobForTransform()
    {
        _transformAccess = new TransformAccessArray(AddTransform(_gameObject));
        isTask3 = !isTask3;
    }

    private void Update()
    {
        if (isTask3)
        {
            angles = angles + (_speed * Time.deltaTime);
            if (angles > 360)
            {
                angles -=360;
            }

            MyJobForTransform myJobForTransform = new MyJobForTransform()
            {
                angles = angles,
            };
            JobHandle jobHandle;
            jobHandle = myJobForTransform.Schedule(_transformAccess);
            jobHandle.Complete();
        }
    }

    private Transform[] AddTransform(GameObject gameObject)
    {
        return new Transform[] { gameObject.transform };
    }

    public struct MyJob : IJob
    {
        public NativeArray<int> arrays;
        public void Execute()
        {
            for (int i = 0; i < arrays.Length; i++)
            {
                arrays[i] += 1;
                if (arrays[i] > 10)
                {
                    arrays[i] = 10;
                }
            }
        }
    }

    public struct MyIJobParallelFor : IJobParallelFor
    {
        public NativeArray<Vector3> Positions;
        public NativeArray<Vector3> Velocities;
        public NativeArray<Vector3> FinalPositions;

        public void Execute(int index)
        {
            FinalPositions[index] = Positions[index] + Velocities[index];
        }
    }

    public struct MyJobForTransform : IJobParallelForTransform
    {
        public float angles;
        public void Execute(int index, TransformAccess transform)
        {
            transform.localRotation = Quaternion.AngleAxis(angles, Vector3.up);
        }
    }

    public void Dispose()
    {
        array.Dispose();
        Positions.Dispose();
        Velocities.Dispose();
        FinalPositions.Dispose();
        _transformAccess.Dispose();
    }
}


