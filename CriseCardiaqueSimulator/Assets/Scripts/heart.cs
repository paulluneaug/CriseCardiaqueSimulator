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

            //rend = GetComponent<Renderer>();
            //rend.material.shader = material;
        }
    }

    void Update()
    {
        //transform.localScale = new Vector3(transform.localScale.x, curve.Evaluate(Time.time), transform.localScale.z);

        material = GetComponent<Renderer>().material;
        material.SetFloat("_heartbeat", curve.Evaluate(Time.time));
    }
}
