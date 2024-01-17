using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    public TransitionSettings transition;
    public float startDelay;

    public void FuncLoadScene(string _sceneName)
    {
        TransitionManager.Instance().Transition(_sceneName, transition, startDelay);
    }
}
