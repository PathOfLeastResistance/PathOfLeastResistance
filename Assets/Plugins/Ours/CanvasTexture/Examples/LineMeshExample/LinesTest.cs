using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using ViJApps.CanvasTexture.Utils;
using Mesh = UnityEngine.Mesh;

namespace ViJApps.CanvasTexture.Examples
{
    [ExecuteInEditMode]
    public class LinesTest : MonoBehaviour
    {    
        private List<Transform> m_PositivePoints;
        private List<Transform> m_negativePoints;

        [SerializeField] private Transform m_PositivePrefab;
        [SerializeField] private Transform m_negativePrefab;
        
        [SerializeField] private MeshFilter m_meshFilter;
        [SerializeField] private MeshFilter m_meshFilter2;
        [SerializeField] private LineJoinType m_joinType;
        [SerializeField] private LineEndingType m_endType;
        [SerializeField] private float m_thickness = 1f;
        [SerializeField] private float m_miter = 1f;

        [Range(0f,1f)]
        [SerializeField] private float m_lineOffset = 1f;

        void Update()
        {
            m_PositivePoints = m_PositivePrefab.GetComponentsInChildren<Transform>().ToList();
            m_negativePoints = m_negativePrefab.GetComponentsInChildren<Transform>().ToList();
            
            var positivePoints = m_PositivePoints.Where(t => t!=m_PositivePrefab).Select(c =>
            {
                Vector3 position;
                return new float2((position = c.position).x, position.y);
            }). ToList();
            
            var negativePoints = m_negativePoints.Where(t=> t!=m_negativePrefab).Select(c=> 
            {
                Vector3 position;
                return new float2((position = c.position).x, position.y);
            }).ToList();
            
            // m_meshFilter.mesh = MeshTools.CreatePolyLine(points.ToList(), m_thickness, m_endType, m_joinType, m_miter, mesh: m_meshFilter.mesh);
            (m_meshFilter.sharedMesh, m_meshFilter2.sharedMesh) = MeshTools.CreatePolygon( positivePoints, negativePoints, m_thickness, m_lineOffset, m_joinType, m_miter, m_meshFilter.sharedMesh, m_meshFilter2.sharedMesh);
        }
    }
}
