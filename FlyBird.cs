using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyBird : MonoBehaviour
{
    // Start is called before the first frame update
    public int iFlyDistanceMax;
    public int iRunSpeed;
    Vector3[] mPoints;
    Vector3 mCurrentPos;
    Vector3 mCurrentPoint;
    Vector3 mNextPoint;
    int iCurrentIndex;
    const int POINT_SIZE = 3;
    Animator mAni;
    int iTick = 0;
    void Start()
    {

        //Debug.Log("start");
        mPoints = new Vector3[POINT_SIZE];
        Vector3 random;
        random = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.1f, 0.1f), Random.Range(-1f, 1f));
        //Debug.Log("radom:" + random.ToString());
        mPoints[0] = transform.position;
        mPoints[1] = transform.position + random * iFlyDistanceMax;
        random = new Vector3(Random.Range(2f, 3f), Random.Range(-0.1f, -0.1f), Random.Range(2f, 3f));
        mPoints[2] = transform.position + random * iFlyDistanceMax;
        mCurrentPos = mPoints[0];
        mCurrentPoint = mPoints[0];
        mNextPoint = mPoints[1];
        iCurrentIndex = 0;
        mAni = GetComponent<Animator>();
    }

    void MoveNext()
    {
        Vector3 dir = mNextPoint - mCurrentPoint;
        dir = dir.normalized;
        Vector3 nextStep = mCurrentPos + dir * Time.deltaTime * iRunSpeed;
        float distance = Vector3.Distance(nextStep, mNextPoint);
        if (distance < Time.deltaTime * (iRunSpeed + 1))
        {
            iCurrentIndex += 1;
            iCurrentIndex %= POINT_SIZE;
            int NextIndex = iCurrentIndex + 1;
            NextIndex %= POINT_SIZE;
            mNextPoint = mPoints[NextIndex];
            //string s = "Current pos:" + iCurrentIndex.ToString();
            //Debug.Log(s);
            //s = "Next pos:" + NextIndex.ToString();
            //Debug.Log(s);
            mCurrentPoint = mPoints[iCurrentIndex];
        }

        Vector3 Orgin = transform.position;
        Vector3 Target = mNextPoint;
        Vector3 Dir = Target - Orgin;

        //Quaternion targetRotation = Quaternion.LookRotation(Dir);
        //this.transform.rotation = targetRotation;

        mCurrentPos = nextStep;
        transform.position = nextStep;
    }

    // Update is called once per frame
    void Update()
    {
        if (iTick ++ % 100 == 0)
        {
            int t = Random.Range(0,2);
            mAni.SetInteger("state", t%2);
        }
        MoveNext();
        
        /*
        if (mAni.IsPlaying("walk") == false)
        {
             mAni.Play("walk");
        }
        */
    }
}