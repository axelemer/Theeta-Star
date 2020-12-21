using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{

    public bool isLeader;

    public LayerMask LayerBoid;
    public LayerMask LayerObst;
    public LayerMask LayerFloor;

    public List<Collider> friends;
    public List<Collider> obstacles;

    public float speed;
    public float rotSpeed;

    public Vector3 dir;

    private Vector3 _vectCohesion;
    private Vector3 _vectAlineacion;
    private Vector3 _vectSeparacion;
    private Vector3 _vectLeader;
    private Vector3 _vectAvoidance;
    private Vector3 _vectWander;

    private Vector3 _seek;

    public float radFlock;
    public float radSep;
    public float radObst;

    public float avoidWeight;
    public float leaderWeight;
    public float alineationWeight;
    public float separationWeight;
    public float cohesionWeight;

    public Transform BoidLeader;

    public float WanderThink;
    float currentTimeToWander;

    public float TimeToGetFriends = 1f;
    float currentTimeToGetFriends;

    public float TimeToGetObst = 1f;
    float currentTimeToGetObst;

    public Collider closerOb;

    public GameObject target;
    public Vector3 _dirToGo;

    void Update()
    {
        GetFriendsAndObstacles();
        Flock();
    }
    void Flock()
    {
        GetFriendsAndObstacles();
        closerOb = GetCloserOb();
        _vectCohesion = getCohesion() * cohesionWeight;
        _vectAlineacion = getAlin() * alineationWeight;
        _vectSeparacion = getSep() * separationWeight;
        _vectLeader = getLeader() * leaderWeight;
        _vectWander = getWander();
        _vectAvoidance = getObstacleAvoidance() * avoidWeight;
        _seek = Seek();


        dir = _vectAvoidance;

        dir += isLeader ? _seek : _vectLeader + _vectWander;
        if(_vectAlineacion.magnitude > 0)
        {
            dir += _vectAlineacion;
        }
        if (_vectCohesion.magnitude > 0)
        {
            dir += _vectCohesion;
        }
        if (_vectSeparacion.magnitude > 0)
        {
            dir += _vectSeparacion;
        }
        if (Vector3.Distance(transform.position, target.transform.position) > 1.5f)
        {
            RaycastHit hit;
            Vector3 pos = transform.localPosition + Vector3.up * 10;
            Vector3 raydir = (transform.localPosition - Vector3.up * 50) - transform.localPosition;
            if (Physics.Raycast(pos, raydir, out hit, Mathf.Infinity, LayerFloor, QueryTriggerInteraction.Collide))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y + 0.5f, transform.position.z);
            }
            transform.forward = Vector3.Slerp(transform.forward, dir, rotSpeed * Time.deltaTime);
            transform.position += transform.forward * Time.deltaTime * speed;

        }
    }

    Vector3 Seek()
    {
        Vector3 seek;
        //Calculamos hacia donde deberia estar mirando
        _dirToGo = target.transform.position - transform.position;
        //Vamos modificando el foward hacia la direccion
        seek = Vector3.Lerp(transform.forward, _dirToGo, rotSpeed * Time.deltaTime);
        //Hacemos que avance hacia adelante
        //transform.position += transform.forward * speed * Time.deltaTime;

        return seek;
    }

    void GetFriendsAndObstacles()
    {
        friends.Clear();
        friends.AddRange(Physics.OverlapSphere(transform.position, radFlock, LayerBoid));
        obstacles.Clear();
        obstacles.AddRange(Physics.OverlapSphere(transform.position, radObst, LayerObst));

    }
    Collider GetCloserOb()
    {
        if (obstacles.Count > 0)
        {
            Collider closer = null;
            float dist = 99999;
            foreach (var item in obstacles)
            {
                var newDist = Vector3.Distance(item.transform.position, transform.position);
                if (newDist < dist)
                {
                    dist = newDist;
                    closer = item;
                }
            }
            return closer;
        }
        else
            return null;
    }
    Vector3 getWander()
    {
        Vector3 wander = _vectWander;

        if (isLeader && currentTimeToWander >= WanderThink)
        {
            wander = Vector3.Slerp(transform.forward, new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2)), rotSpeed);
            currentTimeToWander = 0;
        }
        else
        {
            currentTimeToWander += Time.deltaTime;
        }

        return wander;
    }

    Vector3 getAlin()
    {
        Vector3 al = new Vector3();
        foreach (var item in friends)
            al += item.transform.forward;
        return al /= friends.Count;
    }

    Vector3 getSep()
    {
        Vector3 sep = new Vector3();
        foreach (var item in friends)
        {
            Vector3 f = new Vector3();
            f = transform.position - item.transform.position;
            float mag = radSep - f.magnitude;
            f.Normalize();
            f *= mag;
            sep += f;
        }
        return sep /= friends.Count;
    }

    Vector3 getCohesion()
    {
        Vector3 coh = new Vector3();
        foreach (var item in friends)
            coh += item.transform.position - transform.position;
        return coh /= friends.Count;
    }
    Vector3 getObstacleAvoidance()
    {
        if (closerOb)
            return transform.position - closerOb.transform.position;
        else return Vector3.zero;
    }
    Vector3 getLeader()
    {
        if (!isLeader)
            return BoidLeader.transform.position - transform.position;
        else
            return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        if (_vectAvoidance != Vector3.zero)
            Gizmos.DrawLine(transform.position, _vectAvoidance);
    }

}

