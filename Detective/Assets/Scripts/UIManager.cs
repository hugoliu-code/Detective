using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Slider slider;
    public void UpdateSlider(float input)
    {
        slider.value = input;
    }


}
