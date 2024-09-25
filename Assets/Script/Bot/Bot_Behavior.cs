using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Bot_Behavior : MonoBehaviour
{
    [SerializeField] Bot_CheckTargetInSight sightCheck;
    [SerializeField] Transform target;

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

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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
        transform.eulerAngles += Vector3.up * rotateSpeed * Time.deltaTime;
    }
    private void LookAtTarget(float speed)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, speed * Time.deltaTime);
    }
    private void ChaseTarget()
    {
        print("Chasing");
        agent.SetDestination(target.position);
        agent.speed = chaseSpeed;
    }
}