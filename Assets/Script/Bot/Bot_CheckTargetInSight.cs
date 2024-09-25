using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Bot_CheckTargetInSight : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform inSightCheckObject;  // A child Square with isTrigger collider
    [SerializeField] GameObject inSightIndicatorObject;   // Yellow ! indicate when player's in sight
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
            print("Is in sight angle: " + isInSightAngle + ", is can see player: " + isCanSeePlayer);
        }
        //print("Is in sight angle: " + isInSightAngle + ", is can see player: " + isCanSeePlayer);

        if (isDisplayIndicator) DisplayInSightCheck();
        return isInSightAngle && isCanSeePlayer;
    }
    private void DisplayInSightCheck()  // Display "!" when object is clearly looking at target
    { 
        if (isInSightAngle && isCanSeePlayer)   
        {
            if (!inSightIndicatorObject.activeInHierarchy)
                inSightIndicatorObject.SetActive(true);
        }
        else
        {
            if (inSightIndicatorObject.activeInHierarchy)
                inSightIndicatorObject.SetActive(false);
        }
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