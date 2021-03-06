﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    public bool bReverse;
    public int iStartIndex;
    Vector3[] mPoints;
    Vector3 mDir;
    Vector3 mTa, mTb,mTx;
    int iCurrentIndex;
    int PSIZE = 0;
    int iTick = 0;
    Camera mCamera;
    Animator mAni;
    Vector3[] mPool;
    const int POOL_SIZE = 60;
    MOVE_DIR mMoveHeight;
    float fDeltaY;
    
    enum MOVE_DIR
    {
        UP = 1,
        DOWN = 2,
        HOR = 3,
    };
    void Start()
    {
        fDeltaY = 0;
        mMoveHeight = MOVE_DIR.HOR;
        GameObject obj = GameObject.Find("MainCamera");
        mCamera = obj.GetComponent<Camera>();
        mAni = GetComponent<Animator>();
        
        if (mCamera == null)
        {
            Debug.Log("ccan found canmear");
        }
        else
        {
            mCamera.transform.position = transform.position;
        }
        //mCamera = obj.GetComponent<Camera>();
        mTa = new Vector3();
        mTb = new Vector3();
        mTx = new Vector3();
        mPool = new Vector3[POOL_SIZE];
        
        PSIZE = FindChild();
        if (PSIZE <= 0)
        {
            Debug.Log("start track fail");
            return;
        }
        iCurrentIndex = iStartIndex % PSIZE;
        AdjustPointY();
        mDir = mPoints[(iCurrentIndex + 1) % PSIZE] - mPoints[iCurrentIndex % PSIZE];
        transform.position = mPoints[iCurrentIndex % PSIZE];
        transform.position += Vector3.up * 3;
        for (int i = 0; i < POOL_SIZE; i++)
        {
            mPool[i] = new Vector3();
            Vector3Copy(transform.position, ref mPool[i]);
            Debug.Log("src"+ transform.position.ToString() + "->" + mPool[i].ToString());

        };

    }
    /*fill points,maybe sort point*/

    public int FindChild()
    {
        int i = 0;
        GameObject father = GameObject.Find("Points");
        if (father == null)
            return -1;
        mPoints = new Vector3[father.transform.childCount];
        foreach (Transform child in father.transform)
        {
            //print(child.name);
            if (bReverse)
            {
                int t = father.transform.childCount - 1 - i++;      
                mPoints[t] = child.position;
            }
            else
            {
                mPoints[i++] = child.position;
            }
        }
        return father.transform.childCount;
    }

    class TransInfo
    {
        Transform mT;
        int iSuffix;
    };
    /*
    public int FindChildWithSort()
    {
        int i = 0;
        int cnt = transform.childCount;
        string s, sindex1;
        mPoints = new Vector3[cnt];
        TransInfo[] trans = new TransInfo[cnt];
        foreach (Transform child in transform)
        {
            print(child.name);
            trans[i].mT = child;
            s = child.name;
            sindex1 = s.Substring(1, s.Length - 1);
            trans[i].iSuffix = int.Parse(sindex1);
            //mPoints[i++] = child
        }

        TransInfo t;
        TransInfo a[] = trans;
        for (int i = 0; i < cnt; i++)
        {
            for (int j = 0; j < cnt; j++)
            {
                if (a[i].iSuffix < a[j].iSuffix)
                {
                    t = a[j];
                    a[j] = a[i];
                    a[i] = t;
                }
            }
        }

        for (int i = 0; i < cnt; i++)
        {
            mPoints[i] = a[i];
        }
        return cnt;
    }
    */
    float GetGroundHeight(Vector3 pos)
    {
        float height = 0;
        bool avail = false;
        Vector3 skypos = new Vector3(pos.x, pos.y, pos.z);
        skypos.y += skypos.y + 300;
        Vector3 dir = pos - skypos;
        Ray ray = new Ray(skypos, dir);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0xFFFF))
        {
            // Debug.Log("fuck in");
            if (hit.collider.gameObject.name.Equals("Terrain"))
            {
                height = hit.point.y;
                avail = true;
            }
        }
        if (avail == false)
            return -1;
        return height;
    }

    void AdjustPointY()
    {
        Vector3[] entry;
        entry = mPoints;
        for (int i = 0; i < PSIZE; i++)
        {
            entry[i].y = GetGroundHeight(entry[i]);
            if (entry[i].y < 0)
            {
                Debug.Log("invaild height!!!!!!!!!!!! check again:" + i.ToString());
            }
            Debug.Log("P" + (i + 1).ToString() + " :" + entry[i].ToString());
        }
    }

    /*return next posistion*/
    public Vector3 GetNext(Vector3 realpos)
    {
        Vector3[] entry;
        entry = mPoints;
        Vector3 current = entry[iCurrentIndex % PSIZE];
        Vector3 next = entry[(iCurrentIndex + 1) % PSIZE];
        Vector3 dir = next - current;
        dir = dir.normalized;
        Vector3 nextStep = dir * Time.deltaTime * 6;
        /*check point next*/
        //float distance = Vector3.Distance(realpos, next);
        float distance = GetXZDistance(realpos,next);
        Debug.DrawLine(current, next,Color.yellow);
        //Debug.Log("f:" + realpos.ToString() + " t:" + next.ToString()  + ",distance:" + distance.ToString());
        mDir = next - transform.position;
        if (distance < 2)
        {
            iCurrentIndex += 1;
            iCurrentIndex %= PSIZE;
            //mDir = mPoints[(iCurrentIndex + 1) % PSIZE] - mPoints[iCurrentIndex % PSIZE];
            if (mDir.y > 6)
            {
                mMoveHeight = MOVE_DIR.UP;
            }
            else if (mDir.y < 6)
            {
                mMoveHeight = MOVE_DIR.DOWN;
            }
            else
                mMoveHeight = MOVE_DIR.HOR;

        }
        return nextStep + realpos;
    }

    public Vector3 GetDirection()
    {
        //mDir = mPoints[(iCurrentIndex + 1) % PSIZE] - transform.position;
        return mDir;
    }


    float GetXZDistance(Vector3 a, Vector3 b)
    {
        mTa.x = a.x;
        mTa.z = a.z;
        mTa.y = 0;
        mTb.x = b.x;
        mTb.z = b.z;
        mTb.y = 0;
        return Vector3.Distance(mTa,mTb);
    }
    void TowardForce(Vector3 dir)
    {
        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = targetRotation;
    }
    void Toward(Vector3 dir)
    {
        //Vector3 dir = to - from;
        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);

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
    bool bCanMove = true;
    void Update()
    {
        Vector3 cur = transform.position;
        Vector3 next = GetNext(cur);
        Vector3 dir = GetDirection();
        
        
        mTx.x = dir.x;
        mTx.z = dir.z;
        mTx.y = 0;
        mTx = mTx.normalized;
        //Debug.Log(mTx);
        
        UpdatePool(next);
        if (bCanMove)
        {
            Vector3 t = transform.position - mPool[0];
            if (t.y < -2f )
            {
                if (fDeltaY < 3)
                     fDeltaY += Time.deltaTime * Mathf.Abs(t.y) * 2;
            }
            if (t.y > 0.5f )
            {
                if( fDeltaY > -2)
                    fDeltaY -= Time.deltaTime * t.y;
            }
            else
            {
                if (fDeltaY > 0.1f)
                {
                    fDeltaY -= Time.deltaTime;
                }
                else if (fDeltaY < -0.1f)
                {
                    fDeltaY += Time.deltaTime;
                }

            }
            //Debug.Log("delta y:" + fDeltaY.ToString());
            mPool[0].y = transform.position.y;



            // Debug.Log("ext y:" + deltay.ToString());
            if (mCamera != null)
            {
                mCamera.transform.position = mPool[0] + Vector3.up * (2.4f + fDeltaY);
                mCamera.transform.rotation = transform.rotation;
            }
            //transform.position = next;
            Toward(dir);
            mAni.SetFloat("Forward", 3, 0.1f, Time.deltaTime);
        }
        
        //TowardForce(dir);
        //Debug.DrawLine(mPoints[iCurrentIndex % PSIZE],mPoints[(iCurrentIndex+ 1) % PSIZE], Color.red);
    }
}