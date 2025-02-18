using UnityEngine;
using System.Collections;

public class Heart : MonoBehaviour
{
    public AnimationCurve curve;
    Renderer rend;
    Material material;
    void Start()
    {
        if (Shader.Find("Heart") == null){
            Debug.LogWarning("NULL");
        }
        else {
        material = new Material(Shader.Find("Heart"));
        }
        //rend = GetComponent<Renderer>();
        //rend.material.shader = Shader.Find("Heart");
    }

    void Update()
    {
        //transform.localScale = new Vector3(transform.localScale.x, curve.Evaluate(Time.time), transform.localScale.z);

        //GetComponent<Renderer>().material = material;
        //material.SetFloat("_heartbeat", 0);
    }
}
