using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/AlertedByAlly")]
public class AlertedByAllyDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        if (controller.enemyAI.isAlerted || controller.enemyAI.wasAttacked)
        {
            controller.enemyAI.isAlerted = false;
            return true;
        }

        return false;
    }
}