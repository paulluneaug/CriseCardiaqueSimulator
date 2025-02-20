using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Heart : MonoBehaviour
{
    [SerializeField] private AnimationCurve m_curve;
    [SerializeField] private Renderer m_renderer;
    [SerializeField] private BPMFileReader m_bpmReader;

    [SerializeField] private string m_propertyName = "_heartbeat";

    [NonSerialized] private MaterialPropertyBlock m_materialPropertyBlock;
    [NonSerialized] private int m_propertyID;

    [NonSerialized] private float m_time;

    void Start()
    {
        m_materialPropertyBlock = new MaterialPropertyBlock();
        m_propertyID = Shader.PropertyToID(m_propertyName);

        m_time = 0.0f;
    }

    void Update()
    {
        m_time += Time.deltaTime * ((float)m_bpmReader.CurrentBPM / 60.0f);

        m_renderer.GetPropertyBlock(m_materialPropertyBlock);

        m_materialPropertyBlock.SetFloat(m_propertyID, m_curve.Evaluate(m_time));

        m_renderer.SetPropertyBlock(m_materialPropertyBlock);

    }
}
