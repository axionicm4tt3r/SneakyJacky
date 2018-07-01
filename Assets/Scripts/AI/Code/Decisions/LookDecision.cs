using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Look")]
public class LookDecision : Decision
{

    public override bool Decide(StateController controller)
    {
        bool targetVisible = Look(controller);
        return targetVisible;
    }

    private bool Look(StateController controller)
    {
        Transform playerTransform;
        Debug.DrawRay(controller.eyes.position, controller.eyes.forward.normalized * controller.enemyStats.fieldOfVisionDistance, Color.green);

        if (controller.CanSeePlayer(out playerTransform))
        {
            controller.chaseTarget = playerTransform;
            controller.targetLastKnownPosition = playerTransform.position;
            controller.AlertNearbyEnemies();
            return true;
        }
        else
        {
            return false;
        }
    }
}