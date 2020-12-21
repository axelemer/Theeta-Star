using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CharacterMovement : MonoBehaviour
{
    public LayerMask LayerFloor;
    public float speed;
    public float rotationSpeed;
    public float stopDistance;
    public bool useStopDistance;
    private Vector3 pointToGo;
    public bool onMove;

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
                onMove = true;
            }
            else
            {
                onMove = false;
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
        RaycastHit hit;
        Vector3 pos = transform.localPosition + Vector3.up * 10;
        Vector3 raydir = (transform.localPosition - Vector3.up * 50) - transform.localPosition;
        if (Physics.Raycast(pos, raydir, out hit, Mathf.Infinity, LayerFloor, QueryTriggerInteraction.Collide))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.5f, transform.position.z);
        }
        //transform.position = Vector3.MoveTowards(transform.position, new Vector3(pointToGo.x, transform.position.y, pointToGo.z), speed * Time.deltaTime);
        Vector3 dir = (pointToGo - transform.position).normalized;
        transform.position += new Vector3(dir.x, transform.position.y, dir.z) * speed * Time.deltaTime;
    }

    public void SetPoint(Vector3 pointToGo)
    {
        this.pointToGo = pointToGo;
    }
}
