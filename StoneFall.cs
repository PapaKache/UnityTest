using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class StoneFall : MonoBehaviour
{
    int iTick = 0;
    Vector3 []mPool;
    Vector3 mInit;
    const int POOL_SIZE = 10; 
    mPool = new Vector3[POOL_SIZE];
    void Start()
    {
        mInit = transform.position;
        for (int i = 0; i < POOL_SIZE; i++)
        {
            mPool[i] = new Vector3();
            Vector3Copy(transform.position, ref mPool[i]);
            Debug.Log("src"+ transform.position.ToString() + "->" + mPool[i].ToString());

        };
    }
    
    void Vector3Copy(Vector3 src, ref Vector3 dst)
    {
        dst.x = src.x;
        dst.y = src.y;
        dst.z = src.z;
    }
    
    void UpdatePool(Vector3 n)
    {
        for (int i = 0; i < POOL_SIZE - 1;i++)
        {
            //mPool[i] = mPool[i + 1];
            Vector3Copy(mPool[i + 1], ref mPool[i]);
        }
       // mPool[POOL_SIZE - 1] = n;
        Vector3Copy(n, ref mPool[POOL_SIZE - 1]);
       
    }
    // Update is called once per frame
    void Update()
    {
       if ((iTick + 1)% POOL_SIZE * 3)
       {
           float distance = Vector3.Distance(mPool[0],transform.position);
           if (distance < 2)
           {
               transform.position = mInit;
               Debug.Log("move to init postion");
               //move origin
           }
       }
       UpdatePool(transform.position);
       iTick++;
    }
}