using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class TaskTest : MonoBehaviour, IDisposable
{
    private CancellationTokenSource cancelTokenSource;

    void Start()
    {
        using (cancelTokenSource = new CancellationTokenSource())
        {
            CancellationToken cancelToken = cancelTokenSource.Token;
            Task task1 = Task.Run(() => Task1(cancelToken, 1));
            Task task2 = Task.Run(() => Task2(cancelToken, 60));
            var result  = WhatTaskFasterAsync(cancelTokenSource, task1, task2);
            Task.WaitAny(result);
            Debug.Log(result.Result);
        }

    }

    async Task Task1(CancellationToken cancelToken, float times)
    {
        while (times > 0)
        {
            if (cancelToken.IsCancellationRequested != true)
            {
                return;
            }

            times -= 0.1f;
            await Task.Delay(100);
            Debug.Log("Task1 отработал");
        }
    }
    async Task Task2(CancellationToken cancelToken, int times)
    {

        while (times > 0)
        {
            if (cancelToken.IsCancellationRequested != true)
            {
                return;
            }
            times--;
            
            await Task.Yield();
        }
        Debug.Log("Task2 отработал");
    }

    public static async Task<bool> WhatTaskFasterAsync(CancellationTokenSource cancelToken, Task task1, Task task2)
    {
        var a = Task.WaitAny(task1, task2);
        cancelToken.Cancel();
        return a == 0;
    }
    public void Dispose()
    {
        cancelTokenSource?.Cancel();
        cancelTokenSource?.Dispose();
    }
}
