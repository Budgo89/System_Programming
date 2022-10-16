using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private RenderTexture _renderTexture;

    public Camera Camera => _camera;
    public RenderTexture RenderTexture => _renderTexture;
}
