using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    Animator mAni;
    float iLastTime;
    GameObject t;
    Vector3 mOrg, mTar;
    // Start is called before the first frame update
    void Start()
    {
        mAni = GetComponent<Animator>();
        iLastTime = Time.time;
        Debug.Log("play start");
        mAni.SetInteger("state",1);

        t = GameObject.Find("male_test");
        //bool r = Physics.(transform.position, t.transform.position + new Vector3(10,10,10));
        Rigidbody rig = GetComponent<Rigidbody>();
        Debug.Log("center:" + rig.worldCenterOfMass);
        Vector3 center = rig.worldCenterOfMass;
        center.x = 0;
        center.z = 0;
        center.y = 0;
        mOrg = transform.position + center;
        mTar = t.transform.position + center;
        Vector3 dir = mTar - mOrg;
        Ray ray = new Ray(mOrg , dir);
        float distance = Vector3.Distance(mOrg,mTar);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance * 2))
        {
            //print(hit.point);
            //print(hit.transform.position);
            print(hit.collider.gameObject);
        }
            Debug.Log("s:" + transform.position.ToString() + " ->t:" + t.transform.position );

    }

    // Update is called once per frame
    void Update()
        {
          Debug.DrawLine(mOrg, mTar, Color.yellow);
       // Vector3 dir = mTar - mOrg;
        //Ray ray = new Ray(mOrg, mTar);
       // float distance = Vector3.Distance(mOrg, mTar);
       // RaycastHit hit;
       // if (Physics.Raycast(ray, out hit, distance))
      //  /{
            //print(hit.point);
            //print(hit.transform.position);
        //    print(hit.collider.gameObject);
       // }
        //Debug.Log("s:" + transform.position.ToString() + " ->t:" + t.transform.position);
        /*
        if (Time.time - iLastTime > 3)
        {
            iLastTime = Time.time;
            int r = Random.Range(0, 3);
            mAni.SetInteger("state",r);
            Debug.Log("play :" + r);
        }
           )*/
    }
}
