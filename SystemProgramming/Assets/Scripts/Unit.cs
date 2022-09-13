using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    [SerializeField] private int _health = 80;

    private bool isHealing = false;

    private float timeHealing;
    public void ReceiveHealing()
    {
        Debug.Log(_health);
        if (_health != 100 )
        {
            Debug.Log(_health);
            if (isHealing == false)
            {
                
                isHealing = true;
                StartCoroutine(Healing());
                
            }
        }
    }



    IEnumerator Healing()
    {
        timeHealing = 3;
        bool isT = true;
        while (isT)
        {
            _health += 5;
            timeHealing -= 0.5f;
            Debug.Log(_health);
            yield return new WaitForSeconds(0.5f);
            if (timeHealing <= 0)
            {
                isT = false;
                if (_health > 100) _health = 100;
            }

        }

        isHealing = true;
    }
}
