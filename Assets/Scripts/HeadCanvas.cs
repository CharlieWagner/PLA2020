using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadCanvas : MonoBehaviour
{
    [SerializeField]
    private Text _FPSDisplay;
    [SerializeField]
    private Image _FPSGauge;
    
    
    private void Update()
    {
        float framerate = (int)(1f / Time.unscaledDeltaTime);

        _FPSDisplay.text = framerate + " fps";
        _FPSGauge.fillAmount = framerate / 120;
    }
}
