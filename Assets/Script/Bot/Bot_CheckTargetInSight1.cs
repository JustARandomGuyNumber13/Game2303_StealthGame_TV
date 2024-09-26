using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

/*
 *  Can't detect target if viewAngle is over 170 degrees    // CANCEL THIS SCIPT
 */
public class Bot_CheckTargetInSight1 : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform inSightCheckObject;  // A child Square with isTrigger collider
    [SerializeField] float viewDistance, viewAngle;
    [SerializeField] bool isDisplayIndicator;
    
    Vector3 basePos, leftPos, rightPos;
    bool isInSightAngle, isCanSeePlayer;

    private void OnDrawGizmos()
    {
        GetViewPoints();
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(basePos, rightPos); 
        Gizmos.DrawLine(basePos, leftPos);  
        Gizmos.DrawLine(rightPos, leftPos);  
        Gizmos.DrawWireSphere(basePos, viewDistance);
    }

    public bool IsCanSeeTarget()
    {
        isCanSeePlayer = true; isInSightAngle = false;
        if (Vector3.Distance(transform.position, target.position) < viewDistance)
        {
            GetViewPoints();
            Vector3 pPos = new Vector3(target.position.x, basePos.y, target.position.z);
            Vector3 direction = (pPos - basePos).normalized;
            RaycastHit[] hit = Physics.RaycastAll(basePos, direction, viewDistance);
            
            Debug.DrawRay(basePos, direction * viewDistance, Color.red);

            foreach (RaycastHit ob in hit)
            {
                GameObject other = ob.collider.gameObject;
                if (other == inSightCheckObject.gameObject) // Check if target is in sight area
                    isInSightAngle = true;
                else
                    if (Vector3.Distance(transform.position, target.position) > Vector3.Distance(basePos, other.transform.position))  // Check if any object's blocking the view
                    isCanSeePlayer = false;
            }
            Vector3 f = transform.forward * viewDistance;
            print(Vector3.Distance(f, target.position) < Vector3.Distance(f, rightPos));

        }
        return isInSightAngle && isCanSeePlayer;
    }
    private void GetViewPoints()
    {
        float angle = Mathf.Deg2Rad * (viewAngle / 2);  // Normal triangle's sin and cos math to find offset lengths (x, y)
        float xOffset = Mathf.Sin(angle) * viewDistance;
        float zOffset = Mathf.Cos(angle) * viewDistance;

        basePos = transform.position;
        rightPos = basePos + transform.forward * zOffset + transform.right * xOffset;
        leftPos = basePos + transform.forward * zOffset + transform.right * -xOffset;

        Vector3 location = basePos + transform.forward * zOffset;    // Calculate size and position for sight check object *should do it once, unless for debug
        if(inSightCheckObject.position != location)
            inSightCheckObject.position = location;
        if(inSightCheckObject.localScale.x != xOffset * 2)
            inSightCheckObject.localScale = new Vector3(xOffset * 2, 0.1f, 0.1f);
    }
}