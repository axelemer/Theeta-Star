using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CharacterMovement : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float stopDistance;
    public bool useStopDistance;
    private Vector3 pointToGo;

    private void Start()
    {
        this.pointToGo = transform.position;
    }

    private void Update()
    {
        if (useStopDistance)
        {
            if (Vector3.Distance(transform.position, this.pointToGo) > stopDistance)
            {
                Move();
            }
        }
        else
        {
            Move();
        }
    }

    private void Move()
    {
        Quaternion quat = Quaternion.LookRotation(this.pointToGo - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, quat, rotationSpeed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, this.pointToGo, speed * Time.deltaTime);
    }

    public void SetPoint(Vector3 pointToGo)
    {
        this.pointToGo = pointToGo;
    }
}
