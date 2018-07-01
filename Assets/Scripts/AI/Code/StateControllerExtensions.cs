using UnityEngine;
using UnityEngine.AI;

public static class StateControllerExtensions
{
    public static bool CanSeePlayer(this StateController controller, out Transform playerTransform)
    {
        RaycastHit hit;
        Vector3 rayDirection = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerCamera).transform.position - controller.eyes.transform.position;

        if ((Vector3.Angle(rayDirection, controller.eyes.transform.forward)) <= controller.enemyStats.fieldOfVisionAngle * 0.5f)
        {
            var layerMask = LayerMask.GetMask("Default", Helpers.Layers.Player);
            if (Physics.Raycast(controller.eyes.transform.position, rayDirection, out hit, controller.enemyStats.fieldOfVisionDistance, layerMask))
            {
                if (hit.transform.tag == Helpers.Tags.Player || hit.transform.tag == Helpers.Tags.PlayerCamera)
                {
                    playerTransform = hit.transform;
                    return true;
                }
            }
        }

        playerTransform = null;
        return false;
    }

    public static void AlertNearbyEnemies(this StateController controller)
    {
        //Gizmos.DrawSphere(controller.transform.position, controller.enemyStats.alertAlliesRadius);

        var enemyLayer = LayerMask.GetMask(Helpers.Layers.Enemy);
        var enemyColliders = Physics.OverlapSphere(controller.transform.position, controller.enemyStats.alertAlliesRadius, enemyLayer);

        foreach (Collider collider in enemyColliders)
        {
            StateController enemyController = collider.gameObject.GetComponent<StateController>();
            if (enemyController == controller)
                continue;

            //Debug.Log($"EnemyController: {enemyController.gameObject.name} / CurrentController: {controller.gameObject.name}");
            enemyController.chaseTarget = controller.chaseTarget;
            enemyController.targetLastKnownPosition = controller.targetLastKnownPosition;
            enemyController.enemyAI.isAlerted = true;
        }

    }
}