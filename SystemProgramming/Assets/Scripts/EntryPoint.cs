using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] private CanvasView _canvasView;
    [SerializeField] private Unit _unit;


    public void Start()
    {
        _canvasView.ButtonHealing.onClick.AddListener(Healing);
    }

    private void Healing()
    {
        _unit.ReceiveHealing();
    }
}
