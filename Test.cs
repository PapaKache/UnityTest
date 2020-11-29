using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float v1 = Random.Range(-1f, 1f);
        float v2 = Random.Range(-1f, 1f);
        float v3 = Random.Range(-1f, 1f);
        float v4 = Random.Range(-1f, 1f);
        string s = "v1:" + v1.ToString() + ",";
        s += "v2:" + v2.ToString() + ",";
        s += "v3:" + v3.ToString() + ",";
        s += "v4:" + v4.ToString() + ",";
        Debug.Log(s);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
