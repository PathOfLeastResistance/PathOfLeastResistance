using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class UserSettingsPanel : MonoBehaviour
{
    private bool m_isPanelVisible = false;
    
    [SerializeField] private Transform m_root;
    [SerializeField] private Button m_SettingsButton;
    [SerializeField] private Button m_closeButton;
 
    [SerializeField] private Slider m_zoomSlider;
    [SerializeField] private Slider m_rotSlider;

    private const string MouseSens = "MouseSens";

    private const string ObjRotSens = "RotSens";
    
    public bool IsPanelVisible
    {
        get => m_isPanelVisible;
        set
        {
            m_isPanelVisible = value;
            m_root.gameObject.SetActive(m_isPanelVisible);
        }
    }
    private void Start()
    {
        IsPanelVisible = false;
        m_SettingsButton.onClick.AddListener(() => IsPanelVisible = true);
        m_closeButton.onClick.AddListener(() =>
            {
                IsPanelVisible = false;
                PlayerPrefs.SetFloat(MouseSens, m_zoomSlider.value);
                PlayerPrefs.SetFloat(ObjRotSens, m_rotSlider.value);
            }
        );

        //ZOOM
        m_zoomSlider.onValueChanged.AddListener(OnZoomSliderChange);
        if (PlayerPrefs.HasKey(MouseSens))
            m_zoomSlider.value = PlayerPrefs.GetFloat(MouseSens);
        else
            m_zoomSlider.value = SimpleCameraInteraction.Instance.WheelSensitive;
        
        
        //ROT
        m_rotSlider.onValueChanged.AddListener(OnRotSliderChange);
        if (PlayerPrefs.HasKey(ObjRotSens))
            m_rotSlider.value = PlayerPrefs.GetFloat(ObjRotSens);
        else
            m_rotSlider.value = RigidBodySceneElement.RotationSensitivity;
    }
    
    private void OnRotSliderChange(float value)
    {
        RigidBodySceneElement.RotationSensitivity = value;
    }

    private void OnZoomSliderChange(float sens)
    {
        SimpleCameraInteraction.Instance.WheelSensitive = sens;
    }
}
