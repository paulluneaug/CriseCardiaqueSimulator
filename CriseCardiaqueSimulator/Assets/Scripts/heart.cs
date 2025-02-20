using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Heart : MonoBehaviour
{
    [SerializeField, FormerlySerializedAs("curve")] private AnimationCurve m_curve;
    [SerializeField] private Renderer m_renderer;

    [SerializeField] private string m_propertyName = "_heartbeat";

    [NonSerialized] private MaterialPropertyBlock m_materialPropertyBlock;
    [NonSerialized] private int m_propertyID;

    void Start()
    {
        m_materialPropertyBlock = new MaterialPropertyBlock();
        m_propertyID = Shader.PropertyToID(m_propertyName);
    }

    void Update()
    {
        //transform.localScale = new Vector3(transform.localScale.x, curve.Evaluate(Time.time), transform.localScale.z);

        m_renderer.GetPropertyBlock(m_materialPropertyBlock);

        m_materialPropertyBlock.SetFloat(m_propertyID, m_curve.Evaluate(Time.time));

        m_renderer.SetPropertyBlock(m_materialPropertyBlock);

    }
}
