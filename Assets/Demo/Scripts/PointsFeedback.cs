using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointsFeedback : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _lifeTime = 3f;
    private int _value;

    public int Value
    {
        get { return _value; }
        set
        {
            _text.text =(value > 0 ? "+": "-") + value;
            _value = value;
        }
    }

    private void Awake()
    {
        Destroy(gameObject,_lifeTime);
    }
}
