using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/WalkAround")]
public class WalkAroundAction : Action
{
    public override void Act(StateController controller)
    {
        WalkAround(controller);
    }

    private void WalkAround(StateController controller)
    {
        if (controller.CheckIfCountDownElapsed(controller.enemyStats.walkAroundTimeDelay))
        {
            //Find a new point to walk to and resume the walk
            //controller.navMeshAgent.isStopped = false;
            //Debug.Log("Walking around...");
        }
    }
}