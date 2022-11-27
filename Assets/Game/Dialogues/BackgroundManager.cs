using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    private Dictionary<BackgroundController, BackgroundController> m_Backgrounds = new Dictionary<BackgroundController, BackgroundController>();

    [SerializeField] private Transform m_Root = default;

    public void ActivateBackground(BackgroundController request)
    {
        if (request != null)
        {
            var back = GetOrCreate(request);
            back.ShowAnimated();
        }
    }

    private BackgroundController GetOrCreate(BackgroundController request)
    {
        if (!m_Backgrounds.TryGetValue(request, out var back))
        {
            back = Instantiate(request, m_Root);
            m_Backgrounds.Add(request, back);
        }

        return back;
    }
}
