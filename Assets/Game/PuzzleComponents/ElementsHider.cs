using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElementsHider : MonoBehaviour
{
    private List<Renderer> m_renderersToHide;
    private List<Collider> m_collidersToDisable;

    private string m_hiddenTag = "Hidden";

    public void Hide()
    {
        var rootTransformsToHideElements = transform.GetComponentsInChildren<Transform>().Where(t => t.gameObject.CompareTag(m_hiddenTag)).ToList();

        m_renderersToHide = rootTransformsToHideElements.SelectMany(c => c.GetComponentsInChildren<Renderer>()).ToList();
        m_collidersToDisable = rootTransformsToHideElements.SelectMany(c => c.GetComponentsInChildren<Collider>()).ToList();

        foreach (var rendererToHide in m_renderersToHide)
            rendererToHide.enabled = false;

        foreach (var colliderToDisable in m_collidersToDisable)
            colliderToDisable.enabled = false;
    }
}