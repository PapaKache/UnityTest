using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    Vector3[] mPoints;
    Vector3 mDir;
    Vector3 mTa, mTb;
    int iCurrentIndex;
    int PSIZE = 0;
    void Start()
    {
        mTa = new Vector3();
        mTb = new Vector3();
        PSIZE = FindChild();
        if (PSIZE <= 0)
        {
            Debug.Log("start track fail");
            return;
        }
        iCurrentIndex = 70;
        AdjustPointY();
        mDir = mPoints[(iCurrentIndex + 1) % PSIZE] - mPoints[iCurrentIndex % PSIZE];
        transform.position = mPoints[iCurrentIndex % PSIZE];
        transform.position += Vector3.up * 3;
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
            mPoints[i++] = child.position;
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
        for (int i = 0; i < PSIZE; i++)
        {
            mPoints[i].y = GetGroundHeight(mPoints[i]);
            if (mPoints[i].y < 0)
            {
                Debug.Log("invaild height!!!!!!!!!!!! check again:" + i.ToString());
            }
            Debug.Log("P" + (i + 1).ToString() + " :" + mPoints[i].ToString());
        }
    }

    /*return next posistion*/
    public Vector3 GetNext(Vector3 realpos)
    {
        Vector3 current = mPoints[iCurrentIndex % PSIZE];
        Vector3 next = mPoints[(iCurrentIndex + 1) % PSIZE];
        Vector3 dir = next - current;
        dir = dir.normalized;
        Vector3 nextStep = dir * Time.deltaTime * 6;
        /*check point next*/
        //float distance = Vector3.Distance(realpos, next);
        float distance = GetXZDistance(realpos,next);
        Debug.DrawLine(current, next,Color.yellow);
        //Debug.Log("a:" + realpos.ToString() + " b:" + next.ToString()  + ",distance:" + distance.ToString());
        if (distance < Time.deltaTime * 8)
        {
            iCurrentIndex += 1;
            iCurrentIndex %= PSIZE;
            mDir = mPoints[(iCurrentIndex + 1) % PSIZE] - mPoints[iCurrentIndex % PSIZE];
        }
        return nextStep + realpos;
    }

    public Vector3 GetDirection()
    {
        return mDir;
    }


    float GetXZDistance(Vector3 a, Vector3 b)
    {
        mTa.x = a.x;
        mTa.z = a.z;
        mTb.x = b.x;
        mTb.z = b.z;
        return Vector3.Distance(mTa,mTb);
    }
    void TowardForce(Vector3 dir)
    {
        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = targetRotation;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 cur = transform.position;
        Vector3 next = GetNext(cur);
        Vector3 dir = GetDirection();
        transform.position = next;
        TowardForce(dir);
        //Debug.DrawLine(mPoints[iCurrentIndex % PSIZE],mPoints[(iCurrentIndex+ 1) % PSIZE], Color.red);
    }
}