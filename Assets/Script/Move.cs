using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] 
public class Move : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    void Update()
    {
        transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * Time.deltaTime * moveSpeed);
    }
}
