using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button m_ToNextLevel = default;
    [SerializeField] private GameObject m_LevelFinished = default;
    [SerializeField] private PhraseController m_EndingController = default;

    [SerializeField] private Button m_HideObjective = default;
    [SerializeField] private Button m_ShowObjective = default;
    [SerializeField] private GameObject m_ObjectiveRoot = default;
    [SerializeField] private PhraseController m_ObjectiveController = default;

    [SerializeField] private GameObject m_PhoneButton = default;

    public Action OnNextLevel;

    private void Awake()
    {
        m_LevelFinished.SetActive(false);
        m_ObjectiveRoot.SetActive(false);
        m_PhoneButton.SetActive(false);
        m_ShowObjective.gameObject.SetActive(false);
        m_ToNextLevel.onClick.AddListener(() => OnNextLevel?.Invoke());

        m_HideObjective.onClick.AddListener(() => m_ObjectiveRoot.SetActive(false));
        m_ShowObjective.onClick.AddListener(() => m_ObjectiveRoot.SetActive(true));
    }

    public void ShowObjective(Phrase objectivePhrase)
    {
        m_ShowObjective.gameObject.SetActive(true);
        m_PhoneButton.SetActive(true);
        m_ObjectiveRoot.SetActive(true);
        m_ObjectiveController.Init(objectivePhrase);
    }

    public void LevelCompleted(Phrase endingPhrase)
    {
        m_ShowObjective.gameObject.SetActive(false);
        m_LevelFinished.SetActive(true);
        m_ObjectiveRoot.SetActive(false);
        m_PhoneButton.SetActive(false);
        m_EndingController.Init(endingPhrase);
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
}
