using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    private Dictionary<CharacterIconController, CharacterIconController> m_Characters = new();

    [SerializeField] private Transform m_Root = default;

    public void HideAll()
    {
        foreach (var character in m_Characters)
            character.Value.Hide();
    }

    public void ActivateCharacter(CharacterIconController request)
    {
        if (request != null)
        {
            var character = GetOrCreate(request);
            character.ShowAnimated();
        }
    }

    private CharacterIconController GetOrCreate(CharacterIconController request)
    {
        if (!m_Characters.TryGetValue(request, out var character))
        {
            character = Instantiate(request, m_Root);
            m_Characters.Add(request, character);
        }

        return character;
    }
}
