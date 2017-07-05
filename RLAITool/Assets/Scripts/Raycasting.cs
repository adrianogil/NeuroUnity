using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycasting : MonoBehaviour {

    public Vector3[] direction;

	void Update () {
        for (int i = 0; i < direction.Length; i++)
        {
            Physics.Raycast(transform.position, transform.TransformDirection(direction[i]), Mathf.Infinity);
        }
	}

    void OnDrawGizmos()
    {
        if(direction != null)
            for (int i = 0; i < direction.Length; i++)
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(direction[i]), Color.red);
            }
    }
}
