using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bot_CheckTargetInSight2 : MonoBehaviour
{
    [SerializeField] float distance, angle;
    [SerializeField] Transform target;
    [SerializeField] bool displayGizmos;

    private void OnDrawGizmos()
    {
        if (!displayGizmos) return;

        float xOffset = Mathf.Sin(Mathf.Deg2Rad * (angle / 2)) * distance;
        float zOffset = Mathf.Cos(Mathf.Deg2Rad * (angle / 2)) * distance;

        Vector3 basePos = transform.position;
        Vector3 midPos = basePos + transform.forward * distance;
        Vector3 rightPos = basePos + transform.forward * zOffset + transform.right * xOffset;
        Vector3 leftPos = basePos + transform.forward * zOffset + transform.right * -xOffset;
        Vector3 targetPos = new Vector3(target.position.x, basePos.y, target.position.z);

        float AB = Vector3.Distance(midPos, basePos);
        float AC = Vector3.Distance(basePos, targetPos);
        float BC = Vector3.Distance(midPos, targetPos);

        float cosO = (AB * AB + AC * AC - BC * BC) / (2 * AB * AC);
        float resultAngleInRadian = Mathf.Acos(cosO);
        float resultAngleInDegree = Mathf.Rad2Deg * resultAngleInRadian;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(basePos, distance);
        Gizmos.DrawLine(basePos, midPos);
        Gizmos.DrawLine(leftPos, basePos);
        Gizmos.DrawLine(rightPos, basePos);
        Gizmos.DrawLine(leftPos, midPos);
        Gizmos.DrawLine(rightPos, midPos);
    }

    public bool IsCanSeeTarget()
    {
        if (Vector3.Distance(transform.position, target.position) < distance)
        {
            Vector3 basePos = transform.position;
            Vector3 midPos = basePos + transform.forward * distance;
            Vector3 targetPos = new Vector3(target.position.x, basePos.y, target.position.z);

            float AB = Vector3.Distance(midPos, basePos);
            float AC = Vector3.Distance(basePos, targetPos);
            float BC = Vector3.Distance(midPos, targetPos);

            float cosO = (AB * AB + AC * AC - BC * BC) / (2 * AB * AC);
            float resultAngleInRadian = Mathf.Acos(cosO);
            float resultAngleInDegree = Mathf.Rad2Deg * resultAngleInRadian;

            Vector3 rayCastDirection = (target.position - basePos).normalized;
            RaycastHit hit;
            Physics.Raycast(basePos, rayCastDirection, out hit);

            if (displayGizmos)
            {
                Vector3 tPos = new Vector3(target.position.x, basePos.y, target.position.z);
                Vector3 direction = (tPos - basePos).normalized;
                Debug.DrawRay(basePos, direction * distance, Color.black);
            }

            bool isCanSeeTarget = false;
            if(hit.collider != null)
                if(hit.collider.gameObject == target.gameObject)
                    isCanSeeTarget = true;

            bool isInSightAngle = resultAngleInDegree < angle / 2;

            return isCanSeeTarget && isInSightAngle;
        }
        return false;
    }
}
