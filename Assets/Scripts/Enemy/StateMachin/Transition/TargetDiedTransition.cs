using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDiedTransition : Transition
{
    private void Update()
    {
        if (Target==null)
        {
            NeedTransition = true;
        }
    }
}
