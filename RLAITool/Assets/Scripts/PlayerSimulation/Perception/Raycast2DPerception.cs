using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class Raycast2DPerception : IPerception
{
    private Transform mPlayer;
    private int mNumberOfRays;
	private double mRaycastDistance;
    private double[] mFeatureVector;
    private double[] mRaycastValues;

    private IObjectRecognition mObjectRecognition;

    public Raycast2DPerception(Transform playerTransform, int numberOfRays, double raycastDistance, IObjectRecognition objRecognition)
    {
        mPlayer = playerTransform;
        mNumberOfRays = numberOfRays;
		mRaycastDistance = raycastDistance;
        mObjectRecognition = objRecognition;

        mFeatureVector = new double[mObjectRecognition.GetNumberOfObjects() * mNumberOfRays];
        mRaycastValues = new double[mObjectRecognition.GetNumberOfObjects()];
    }


    public double[] GetFeatureVector()
    {
        for (int i = 0; i < mNumberOfRays; i++)
        {
            double[] raycastValues = GetRaycastAtAngle(i * (180f / mNumberOfRays));

            for (int p = 0; p < mObjectRecognition.GetNumberOfObjects(); p++)
            {
                mFeatureVector[i * mObjectRecognition.GetNumberOfObjects() + p] = raycastValues[p];
            }
        }

        return mFeatureVector;
    }

    private double[] GetRaycastAtAngle(double angle)
    {
        Debug.Log("GilLog - Raycast2DPerception::GetRaycastAtAngle - angle " + angle + " ");

        Vector3 direction = new Vector3(0, -1, 0);
        Quaternion rotate = Quaternion.AngleAxis((float)angle, new Vector3(0, 0, 1));
        direction = rotate * direction;

		Vector3 position = mPlayer.position;
		double sqrDistance = (mRaycastDistance * mRaycastDistance);

		RaycastHit2D hit = Physics2D.Raycast(position, direction, (float)mRaycastDistance,
		                                     1,//(1 << LayerMask.NameToLayer(Layers.Platforms_Default)) |
                                             1);//(1 << LayerMask.NameToLayer(Layers.Collectibles)));

        Debug.DrawRay(mPlayer.position, direction * 10);

        for (int r = 0; r < mRaycastValues.Length; r++)
        {
            mRaycastValues[r] = -1;
        }

        if (hit.collider != null && hit.collider.gameObject != null)
        {
			int objectId = mObjectRecognition.GetId(hit.collider.gameObject);

			//Debug.Log(hit.collider.gameObject.name);

			if (objectId >= 0 && objectId < mRaycastValues.Length)
			{
				double diff = (position - hit.collider.gameObject.transform.position).sqrMagnitude;
				diff = (double)Mathf.Clamp((float)diff, 0, (float)sqrDistance);

				mRaycastValues[objectId] = (sqrDistance - diff) / sqrDistance;
				//Debug.Log(objectId);
				//Debug.Log(mRaycastValues[objectId]);
			}
        }

        return mRaycastValues;
    }


    public int GetFeatureSize()
    {
        return mObjectRecognition.GetNumberOfObjects() * mNumberOfRays;
    }

    public void DebugDraw()
    {
        for (int i = 0; i < mNumberOfRays; i++)
        {
            float angle = i * (180f / mNumberOfRays);

            Vector3 direction = new Vector3(0, -1, 0);
            Quaternion rotate = Quaternion.AngleAxis((float)angle, new Vector3(0, 0, 1));
            direction = rotate * direction;
            Debug.DrawRay(mPlayer.position, direction * 10);
        }

    }
}
