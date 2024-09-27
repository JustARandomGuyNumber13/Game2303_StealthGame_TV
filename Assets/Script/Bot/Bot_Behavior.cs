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
    [SerializeField] bool isCase1;
    [SerializeField] float rotateSpeed;

    [Header("Case 2: Look at player")]
    [SerializeField] bool isCase2;
    [SerializeField] float rotateToPlayerDirectionSpeed;

    [Header("Case 3: Chase player")]
    [SerializeField] bool isCase3;
    [SerializeField] float chaseSpeed;
    [SerializeField] float chaseRotateSpeed;

    [Header("Case 4: All + Shoot target")]
    [SerializeField] bool isCase4;
    [SerializeField] float shootChaseSpeed, shootChaseRotateSpeed, shootDelay;
    [SerializeField] GameObject projectile;
    float shootDelayCount;

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
            if (isCase1) count++;
            if (isCase2) count++;
            if (isCase3) count++;
            if (isCase4) count++;

            if (count <= 1)
            {
                isTargetInSight = sightCheck.IsCanSeeTarget();

                if (displayInSightIndicator) DisplayInSightCheck();
                if (isCase4) shootDelayCount -= Time.deltaTime;

                if (isCase1) RotateObject();
                else if (isTargetInSight)
                {
                    if (isCase2) LookAtTarget(rotateToPlayerDirectionSpeed);
                    else if (isCase3)
                    {
                        LookAtTarget(chaseRotateSpeed);
                        ChaseTarget(chaseSpeed);
                    }
                    else if (isCase4)
                    {
                        LookAtTarget(shootChaseRotateSpeed);
                        ChaseTarget(shootChaseSpeed);
                        ShootTarget();
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
    private void ChaseTarget(float speed)
    {
        agent.SetDestination(target.position);
        agent.speed = speed;
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
    private void ShootTarget()
    {
        if (projectile == null) return;
        if (shootDelayCount <= 0)
        {
            Instantiate(projectile, myTransform.position + transform.forward * 1, Quaternion.LookRotation(target.position - transform.position));
            shootDelayCount = shootDelay;
        }
    }
}