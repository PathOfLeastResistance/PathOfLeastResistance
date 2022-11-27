using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button m_ToNextLevel = default;
    [SerializeField] private GameObject m_LevelFinished = default;
    [SerializeField] private PhraseController m_EndingController = default;
    [SerializeField] private Phrase m_EndingPhrase = default;

    public Action OnNextLevel;

    private void Awake()
    {
        m_LevelFinished.SetActive(false);
        m_ToNextLevel.onClick.AddListener(() => OnNextLevel?.Invoke());
    }

    public void LevelCompleted()
    {
        m_LevelFinished.SetActive(true);
        m_EndingController.Init(m_EndingPhrase);
    }

    public void LevelAbandoned()
    {
        m_LevelFinished.SetActive(false);
    }

    private void OnDestroy()
    {
        m_ToNextLevel.onClick.RemoveAllListeners();
    }
}
