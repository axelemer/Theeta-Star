using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boid : MonoBehaviour {

	public List<Collider> flock;
	public LayerMask flockingMask;
	public float radioFlock;
	public float speed;
	public float rotSpeed;

	public Vector3 dir;

	private Vector3 _vectCohesion;
	private Vector3 _vectAlineacion;
	private Vector3 _vectSeparacion;
	private Vector3 _vectLider;

	public float escCohesion;
	public float escAlineacion;
	public float escSeparacion;
	public float escLider;

	public float offsetLider;

	public Transform lider;

	void Start () {
	
	}
	
	void Update ()
	{
		flock.Clear();
		flock.AddRange(Physics.OverlapSphere(transform.position, radioFlock, flockingMask));
		flock.Remove(GetComponent<Collider>());
		_vectCohesion = CalcularCohesion() * escCohesion;
		_vectAlineacion = CalcularAlineacion() * escAlineacion;
		_vectSeparacion = CalcularSeparacion() * escSeparacion;
		_vectLider = CalcularLider() * escLider;
		dir = _vectCohesion + _vectAlineacion + _vectSeparacion + _vectLider;

		transform.forward = Vector3.Slerp(transform.forward, dir, rotSpeed * Time.deltaTime);
		transform.position += transform.forward * Time.deltaTime *  _vectLider.magnitude;
	}


	Vector3 CalcularCohesion()
	{
		Vector3 c = new Vector3();

		for (int i = 0; i < flock.Count; i++)
		{
			c += flock[i].transform.position - transform.position;
		}
		c /= flock.Count;
		return c;
	}

	Vector3 CalcularAlineacion()
	{
		Vector3 a = new Vector3();

		for (int i = 0; i < flock.Count; i++)
		{
			a += flock[i].transform.forward;
		}
		a /= flock.Count;

		return a;
	}

	Vector3 CalcularSeparacion()
	{
		Vector3 s = new Vector3();

		for (int i = 0; i < flock.Count; i++)
		{
			Vector3 v = transform.position - flock[i].transform.position;
			float m = radioFlock - v.magnitude;
			v.Normalize();
			v *= m;
			s += v;
		}

		s /= flock.Count;
		return s;
	}

	Vector3 CalcularLider()
	{
		Vector3 l = (lider.position - lider.forward * offsetLider) - transform.position;
		return l;
	}


	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, _vectCohesion);

		Gizmos.color = Color.blue;
		Gizmos.DrawRay(transform.position, _vectAlineacion);

		Gizmos.color = Color.green;
		Gizmos.DrawRay(transform.position, _vectSeparacion);

		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(transform.position, _vectLider);
	}
}
