using System.Collections;
using System.Collections.Generic;
using PID_Controller;
using UnityEngine;


public interface IPidSettingsProvider
{
    PidSettings TranslationSettings { get; }
    PidSettings RotationSettings { get; }
}

[CreateAssetMenu(fileName = "PidSettingsProvider", menuName = "ScriptableObjects/PidSettingsProvider")]
public class RigidBodySettingsProvider : ScriptableObject, IPidSettingsProvider
{
    [SerializeField] private PidSettings m_globalTranslationPidSettings;
    [SerializeField] private PidSettings m_globalRotationPidSettings;


    public PidSettings TranslationSettings => m_globalTranslationPidSettings;
    
    public PidSettings RotationSettings => m_globalRotationPidSettings;
}
