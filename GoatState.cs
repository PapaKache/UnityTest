using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatState : StateMachineBehaviour
{
    public string m_parametersName = "GoatState";
    public int[] m_stateIDArray = { 0, 1, 2};

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    //官方注释翻译：在此状态机内的任何状态上调用OnStateEnter之前调用OnStateEnter
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_stateIDArray.Length <= 0)
        {
            animator.SetInteger(m_parametersName, 0);
        }
        else
        {
            int index = Random.Range(0, m_stateIDArray.Length);
            //Debug.Log(m_parametersName + "-->" + m_stateIDArray[index]);
            animator.SetInteger(m_parametersName, m_stateIDArray[index]);
        }
    }
}
