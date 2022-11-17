using UnityEngine;
using Zenject;

public class CableView : MonoBehaviour
{
    [SerializeField] private LineRenderer m_lineRenderer;
    [SerializeField] private Transform m_colliderRoot;
    [SerializeField] private Transform m_cableStart;
    [SerializeField] private Transform m_cableEnd;

    private Vector3 m_from;
    private Vector3 m_to;

    public Vector3 From
    {
        get => m_from;
        set
        {
            m_from = value;
            RefreshView();
        }
    }

    public Vector3 To
    {
        get => m_to;
        set
        {
            m_to = value;
            RefreshView();
        }
    }

    private void RefreshView()
    {
        var lineFrom = new Vector3(m_from.x, m_from.z, -m_from.y);
        var lineTo = new Vector3(m_to.x, m_to.z, -m_to.y);

        m_lineRenderer.SetPositions(new Vector3[] { lineFrom, lineTo });

        var worldCenter = (m_from + m_to) / 2;

        m_colliderRoot.transform.position = worldCenter;
        m_colliderRoot.transform.LookAt(m_from, Vector3.up);
        m_colliderRoot.transform.localScale = new Vector3(1, 1, Vector3.Distance(m_from, m_to));

        m_cableStart.transform.position = m_from;
        m_cableEnd.transform.position = m_to;
        m_cableStart.transform.LookAt(worldCenter);
        m_cableEnd.transform.LookAt(worldCenter);
    }
}