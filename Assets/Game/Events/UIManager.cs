using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button m_ToNextLevel = default;
    [SerializeField] private GameObject m_LevelFinished = default;
    [SerializeField] private PhraseController m_EndingController = default;
    [SerializeField] private Phrase m_EndingPhrase = default;

    [SerializeField] private Button m_HideObjective = default;
    [SerializeField] private Button m_ShowObjective = default;
    [SerializeField] private GameObject m_ObjectiveRoot = default;
    [SerializeField] private PhraseController m_ObjectiveController = default;
    [SerializeField] private Phrase m_ObjectivePhrase = default;

    public Action OnNextLevel;

    private void Awake()
    {
        m_LevelFinished.SetActive(false);
        m_ObjectiveRoot.SetActive(false);
        m_ShowObjective.gameObject.SetActive(false);
        m_ToNextLevel.onClick.AddListener(() => OnNextLevel?.Invoke());

        m_HideObjective.onClick.AddListener(() => m_ObjectiveRoot.SetActive(false));
        m_ShowObjective.onClick.AddListener(() => m_ObjectiveRoot.SetActive(true));
    }

    public void LevelCompleted()
    {
        m_ShowObjective.gameObject.SetActive(false);
        m_LevelFinished.SetActive(true);
        m_ObjectiveRoot.SetActive(false);
        m_EndingController.Init(m_EndingPhrase);
    }

    public void LevelAbandoned()
    {
        m_LevelFinished.SetActive(false);
    }

    private void OnDestroy()
    {
        m_ToNextLevel.onClick.RemoveAllListeners();
        m_HideObjective.onClick.RemoveAllListeners();
        m_ShowObjective.onClick.RemoveAllListeners();
    }

    public void ShowObjective()
    {
        m_ShowObjective.gameObject.SetActive(true);
        m_ObjectiveController.Init(m_ObjectivePhrase);
        m_ObjectiveRoot.SetActive(true);
    }
}
