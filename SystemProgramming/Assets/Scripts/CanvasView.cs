using UnityEngine;
using UnityEngine.UI;

public class CanvasView : MonoBehaviour
{
    [SerializeField] private Button _buttonHealing;
    [SerializeField] private Button _buttonTask1;

    public Button ButtonHealing => _buttonHealing;
    public Button ButtonTask1 => _buttonTask1;
}
