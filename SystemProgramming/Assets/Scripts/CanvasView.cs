using UnityEngine;
using UnityEngine.UI;

public class CanvasView : MonoBehaviour
{
    [SerializeField] private Button _buttonHealing;
    [SerializeField] private Button _buttonTask1;
    [SerializeField] private Button _buttonTask2;
    [SerializeField] private Button _buttonTask3;

    public Button ButtonHealing => _buttonHealing;
    public Button ButtonTask1 => _buttonTask1;
    public Button ButtonTask2 => _buttonTask2;
    public Button ButtonTask3 => _buttonTask3;
}
