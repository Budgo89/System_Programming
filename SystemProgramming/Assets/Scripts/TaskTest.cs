using System;
using System.Threading;
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
            _ = Task1(cancelToken);
            _ = Task2(cancelToken, 60);
        }

    }

    async Task Task1(CancellationToken cancelToken)
    {
        if (cancelToken.IsCancellationRequested)
        {
            return;
        }

        await Task.Delay(1000);
        Debug.Log("Task1 отработал");
    }
    async Task Task2(CancellationToken cancelToken, int times)
    {

        while (times > 0)
        {
            if (cancelToken.IsCancellationRequested)
            {
                return;
            }
            times--;
            
            await Task.Yield();
        }
        Debug.Log("Task2 отработал");
    }


    public void Dispose()
    {
        cancelTokenSource?.Cancel();
        cancelTokenSource?.Dispose();
    }
}
