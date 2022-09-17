using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Lesson2 : MonoBehaviour
{
    private NativeArray<int> array;

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

}


