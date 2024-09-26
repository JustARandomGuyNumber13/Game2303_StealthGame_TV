using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Bot_Behavior : MonoBehaviour
{
    [SerializeField] Bot_CheckTargetInSight2 sightCheck;
    [SerializeField] GameObject inSightIndicatorObject;   // Yellow ! indicate when player's in sight
    [SerializeField] Transform target;

    [SerializeField] bool displayInSightIndicator;
    [Header("Choose one case only per test object")]
    [SerializeField] bool checkIfUnderstand;

    [Header("Case 1: Rotate this object")]
    [SerializeField] bool isRotate;
    [SerializeField] float rotateSpeed;

    [Header("Case 2: Look at player")]
    [SerializeField] bool isLookAtTarget;
    [SerializeField] float rotateToPlayerDirectionSpeed;

    [Header("Case 3: Chase player")]
    [SerializeField] bool isChaseTarget;
    [SerializeField] float chaseSpeed;
    [SerializeField] float chaseRotateSpeed;

    bool isTargetInSight;
    NavMeshAgent agent;
    Transform myTransform;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        myTransform = transform;
    }
    void FixedUpdate()
    {
        if (checkIfUnderstand)
        {
            int count = 0;
            if (isRotate) count++;
            if (isLookAtTarget) count++;
            if (isChaseTarget) count++;

            if (count <= 1)
            {
                isTargetInSight = sightCheck.IsCanSeeTarget();

                if (displayInSightIndicator) DisplayInSightCheck();
                if (isRotate) RotateObject();
                if (isTargetInSight)
                {
                    if (isLookAtTarget) LookAtTarget(rotateToPlayerDirectionSpeed);
                    if (isChaseTarget)
                    {
                        LookAtTarget(chaseRotateSpeed);
                        ChaseTarget(); 
                    }
                }
            }
        }
    }

    private void RotateObject()
    {
        myTransform.eulerAngles += Vector3.up * rotateSpeed * Time.deltaTime;
    }
    private void LookAtTarget(float speed)
    {
        Vector3 targetPos = target.position;
        Vector3 myPos = myTransform.position;
        Vector3 direction = new Vector3((targetPos.x - myPos.x), myPos.y, (targetPos.z - myPos.z));
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        myTransform.rotation = Quaternion.Slerp(myTransform.rotation, lookRotation, speed * Time.deltaTime);
    }
    private void ChaseTarget()
    {
        agent.SetDestination(target.position);
        agent.speed = chaseSpeed;
    }
    private void DisplayInSightCheck()  // Display "!" when object is clearly looking at target
    {
        if (isTargetInSight)
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
}