using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LineView : MonoBehaviour
{
    [SerializeField] private TMP_Text _position;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _points;

    public TMP_Text Position => _position;
    public TMP_Text Name => _name;
    public TMP_Text points => _points;

}
