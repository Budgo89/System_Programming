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
