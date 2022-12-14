using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] private CanvasView _canvasView;
    [SerializeField] private Unit _unit;
    [SerializeField] private Lesson2 _lesson2;


    public void Start()
    {
        _canvasView.ButtonHealing.onClick.AddListener(Healing);
        _canvasView.ButtonTask1.onClick.AddListener(StartMyJob);
        _canvasView.ButtonTask2.onClick.AddListener(StartMyIJobParallelFor);
        _canvasView.ButtonTask3.onClick.AddListener(StartMyJobForTransform);
    }

    private void StartMyJobForTransform()
    {
        _lesson2.StartMyJobForTransform();
    }

    private void StartMyIJobParallelFor()
    {
        _lesson2.StartMyIJobParallelFor();
    }

    private void StartMyJob()
    {
        _lesson2.StartMyJob();
    }

    private void Healing()
    {
        _unit.ReceiveHealing();
    }
}
