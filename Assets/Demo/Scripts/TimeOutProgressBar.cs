using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeOutProgressBar : MonoBehaviour
{

    public event Action<TimeOutProgressBar> TimeOut; 
    [SerializeField] private Image _progressImage;
    [SerializeField] private float _maxTimeOut;

    public float CurrentTimeOut { get;private set; }

    public float MaxTimeOut
    {
        get { return _maxTimeOut; }
        set { _maxTimeOut = value; }
    }

    public float Normalized
    {
        get { return _progressImage.fillAmount;}
        set { _progressImage.fillAmount = value; } }

    private void OnEnable()
    {
        CurrentTimeOut = MaxTimeOut;
    }

    private void Update()
    {
        CurrentTimeOut =Mathf.Clamp(CurrentTimeOut - Time.deltaTime,0,Mathf.Infinity);

        Normalized = CurrentTimeOut / MaxTimeOut;

        if(Normalized<=0)
        {
            TimeOut?.Invoke(this);
            gameObject.SetActive(false);
        }
    }


}