
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/RespondToDamage")]
public class RespondToDamageDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        if (controller.enemyAI.wasAttacked)
        {
            controller.enemyAI.isAlerted = true;
            controller.enemyAI.wasAttacked = false;

            var playerTransform = GameObject.FindWithTag(Helpers.Tags.Player).transform;
            controller.chaseTarget = playerTransform;
            controller.targetLastKnownPosition = playerTransform.position;
            controller.AlertNearbyEnemies();
            return true;
        }

        //Add the delay 
        return false;
    }
}