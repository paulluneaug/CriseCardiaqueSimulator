using UnityEngine;
using System.Collections;

public class Heart : MonoBehaviour
{
    public AnimationCurve curve;
    Renderer rend;
    [SerializeField] Material material;

    void Start()
    {
        if (material == null)
        {
            Debug.LogWarning("NULL");
        }
        else
        {
            material = GetComponent<Renderer>().material;
        }
    }

    void Update()
    {
        material.SetFloat("_heartbeat", curve.Evaluate(Time.time));
    }
}
