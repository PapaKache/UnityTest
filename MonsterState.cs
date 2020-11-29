using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
public class MonsterState : MonoBehaviour
{
    // Start is called before the first frame update
    public float fResetTime;
    public float fActiveRadius;
    public bool bCanNotMove;
    Animator mAnimator;
    Vector3 mInitPos;
    float iLastActionTime;
    public int iRunSpeed;
    Rigidbody mBody;
    StateInfo mInfo;
    const int ROTATE_MAX_TICK = 1000;
    const int MOVE_MAX_TICK = 1000;
    Vector3 mTa, mTb;
    Vector3 mCurrentPos;
    enum Action
    {
        IDLE = 9,
        WALK = 0,
        RUN = 1,
        JUMP = 2,
        EAT = 3,
        ATTACK = 4
    }


    public enum State
    {
        RUN,
        WALK,
        IDLE,   //Origin 
        FAR_IDLE,
        RETURN,
    };

    public class StateInfo
    {

        public Vector3 target;
        public Vector3 dir;
        public State state;
        public int rotateTick;
        public int moveTick;
        //public bool rotateFinish;
        public bool moveFinish;
        public float deltaHigh;
    }

    void Start()
    {
        mInfo = new StateInfo();
        mInfo.moveFinish = true;
        //mInfo.rotateFinish = true;
        mInfo.rotateTick = 0;
        mInfo.moveTick = 0;
        mInfo.state = State.IDLE;
        mInitPos = this.transform.position;
        mAnimator = GetComponent<Animator>();
        mBody = GetComponent<Rigidbody>();
        iLastActionTime = Time.time;
        mTa = new Vector3();
        mTb = new Vector3();
        mCurrentPos = new Vector3();
        //Debug.Log("Init:" + mInitPos.ToString());
    }

    float GetGroundHeight(Vector3 pos)
    {
        float height = 0;
        bool avail = false;
        Vector3 skypos = new Vector3(pos.x, pos.y, pos.z);
        skypos.y += skypos.y + 100;
        Vector3 dir = pos - skypos;
        Ray ray = new Ray(skypos, dir);
        //Debug.DrawLine(skypos,pos);
        //Debug.Log("fuck sky to ground:"+ skypos.ToString() + " -> "+ pos.ToString());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0xFFFF))
        {
            // Debug.Log("fuck in");
            if (hit.collider.gameObject.name.Equals("Terrain"))
            {
                height = hit.point.y;
                avail = true;
            }

            if (hit.collider.gameObject.tag.Equals("water"))
            {
                // Debug.Log("is water point");
                avail = false;
            }
        }
        if (avail == false)
            return -1;
        return height;
    }

    Vector3 GetRandomPosition(Vector3 initpos)
    {
        Vector3 random;
        Vector3 output = Vector3.zero;
        int tick = 0;
        bool r = false;
        do
        {
            r = false;
            random = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            if (random.magnitude <= 0)
                continue;

            random = random.normalized;
            output = initpos + random * fActiveRadius;
            float height = GetGroundHeight(output);
            //Debug.Log("pos hegiht:" + height.ToString());
            if (height < 0)
            {
                //Debug.Log("pos hegiht:" + height.ToString());
                r = true;
                Thread.Sleep(1);
            }
            output.y = height;
        } while (r == true);

        return output;
    }

    void RandomAction(StateInfo inf)
    {
        Vector3 current = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 target = new Vector3(transform.position.x,transform.position.y,transform.position.z);
        Vector3 dir = Vector3.one;
        float deltaHeight = 0;
        State state;
        if (bCanNotMove == true)
        {
            return;
        }
        if (!inf.moveFinish)
        //if (!inf.moveFinish || !inf.rotateFinish)
        {
            Debug.Log("is busy move/rotate");
            return;
        }
        //inf.rotateFinish = false;
        inf.moveFinish = false;
        int r = Random.Range(0, 4);
        if (inf.state == State.FAR_IDLE)
        {
            if (r <= 1)
            {
                state = State.FAR_IDLE;
                PlayAction(Action.IDLE);
               // inf.rotateFinish = true;
                inf.moveFinish = true;
            }
            else
            {
                state = State.RETURN;
                target = mInitPos;
                dir = target - transform.position;
                deltaHeight = target.y - transform.position.y;
                PlayAction(Action.WALK);
                TowardForce(current, target);
                print("return:" + target.ToString());
               
            }
        }
        else
        {

            if (r == 0)
            {
                state = State.IDLE;
                PlayAction(Action.IDLE);
               // inf.rotateFinish = true;
                inf.moveFinish = true;
            }
            else if (r <= 2)
            {
                target = GetRandomPosition(mInitPos);
                dir = target - transform.position;
                deltaHeight = target.y - transform.position.y;
                state = State.WALK;
                PlayAction(Action.WALK);
                TowardForce(current, target);
            }
            else
            {
                target = GetRandomPosition(mInitPos);
                dir = target - transform.position;
                deltaHeight = target.y - transform.position.y;
                state = State.RUN;
                PlayAction(Action.RUN);
                TowardForce(current, target);

            }
        }
        inf.target = target;
        inf.state = state;
        inf.dir = dir;
        inf.rotateTick = 0;
        inf.moveTick = 0;
        inf.deltaHigh = deltaHeight;
        //string str1 = string.Format("random {0:G}", state);
        // Debug.Log(str1);

    }

    bool CheckReachRoatate(Vector3 from, Vector3 to, bool log = false)
    {
        int y1, y2;
        Vector3 dir = to - from;
        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        y1 = (int)(transform.rotation.y * 10);
        y2 = (int)(targetRotation.y * 10);
        if (log)
        {
            print("current:" + transform.rotation.ToString() + ",target:" + targetRotation.ToString());
        }
        if (y1 == y2)
        {
            return true;
        }
        return false;
    }
    void Toward(Vector3 from, Vector3 to)
    {
        Vector3 dir = to - from;
        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3);

    }

    void TowardForce(Vector3 from, Vector3 to)
    {
        Vector3 dir = to - from;
        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = targetRotation;
    }

    void MoveStep(Vector3 from, Vector3 to, float speed)
    {
        print("move form:" + from.ToString() + "->" + to.ToString());
        Vector3 dir = to - from;
        dir = dir.normalized;
        Vector3 nextStep = transform.position + dir * Time.deltaTime * speed;
        transform.position = nextStep;

    }

    void MoveStep(Vector3 dir, float speed)
    {
        // print("move form:" + from.ToString() + "->" + to.ToString());
        //Vector3 dir = to - from;
        dir = dir.normalized;
        Vector3 nextStep = transform.position + dir * Time.deltaTime * speed;
        transform.position = nextStep;

    }

    void PlayAction(Action a)
    {
        mAnimator.SetInteger("state", (int)(a));
        //Debug.Log("play action:" + a.ToString());
    }

   
    float GetDistance(Vector3 a, Vector3 b)
    {
        mTa.x = a.x;
        mTa.z = a.z;
        mTa.y = 0;

        mTb.x = b.x;
        mTb.z = b.z;
        mTb.y = 0;


        return Vector3.Distance(mTa,mTb);

    }
    void Sport(StateInfo inf)
    {
        bool test = true;
        float distance = GetDistance(transform.position, inf.target);
        float minDistance = Mathf.Abs(inf.deltaHigh * 2) + iRunSpeed;
        Debug.DrawLine(transform.position, inf.target, Color.red);
        mCurrentPos.x = transform.position.x;
        mCurrentPos.y = transform.position.y;
        mCurrentPos.z = transform.position.z;
        //Debug.DrawLine(mInitPos, inf.target, Color.red);
        /*
        if (inf.rotateFinish == false)
        {
            if (test == true)
            //if (CheckReachRoatate(mCurrentPos, inf.target) == true || inf.rotateTick++ > ROTATE_MAX_TICK)
            {
                inf.rotateFinish = true;
                if (inf.rotateTick > ROTATE_MAX_TICK)
                {
                    print("walk rtick timeout1,tick:" + inf.rotateTick.ToString());
                }
                else
                {
                    print("roate reache");
                }
                TowardForce(mCurrentPos, inf.target);
            }
            else
            {
                Toward(mCurrentPos, inf.target);
                print("walk roate");
                return;
            }
        }
        
        //if (distance < 2)
        {
            Debug.Log("min:" + minDistance.ToString() + ",distance:" + distance.ToString());
        }
        */
        if (distance < minDistance || inf.moveTick++ > MOVE_MAX_TICK)
        {
            State last = inf.state;
            if (inf.state != State.RETURN)
            {
                inf.state = State.FAR_IDLE;
            }
            else
            {
                inf.state = State.IDLE;
            }
            PlayAction(Action.IDLE);
            inf.moveFinish = true;
            print(last.ToString() + "->set far idle:" + inf.state.ToString());
        }
        else
        {
            // Debug.Log("move distance:" + distance.ToString() + " " + transform.position.ToString() + "->" + mNextPoint.ToString());
            //MoveStep(transform.position, inf.target, iRunSpeed);
            MoveStep(inf.dir, iRunSpeed);
            //print("walk move");
        }
    }

    void Update()
    {
        switch (mInfo.state)
        {
            case State.IDLE:
            case State.FAR_IDLE:
                if (Time.time - iLastActionTime > fResetTime)
                {

                    RandomAction(mInfo);
                    iLastActionTime = Time.time;
                    Debug.Log(mInfo.state.ToString());
                }

                break;
            default:
                Sport(mInfo);
                break;

        }
    }
}
