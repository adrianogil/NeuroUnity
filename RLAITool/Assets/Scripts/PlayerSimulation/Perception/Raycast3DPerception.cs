using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class Raycast3DPerception : IPerception
{
    private Transform mPlayer;
    private int mNumberOfRays;
    private double mRaycastDistance;
    private double[] mFeatureVector;
    private double[] mRaycastValues;

    private IObjectRecognition mObjectRecognition;

    private int mNumberOfRaysLatitude, mNumberOfRaysLongitude;
    private float mInitialLatitudeAngle, mFinalLatitudeAngle;
    private float mInitialLongitudeAngle, mFinalLongitudeAngle;

    /// <summary>
    /// initialLatitudeAngle, finalLatitudeAngle - in radians
    /// initialLongitudeAngle, finalLongitudeAngle - in radians
    /// </summary>
    public Raycast3DPerception(Transform playerTransform, int numberOfRaysLatitude, int numberOfRaysLongitude, 
                               float initialLatitudeAngle, float finalLatitudeAngle,
                               float initialLongitudeAngle, float finalLongitudeAngle,
                               double raycastDistance, IObjectRecognition objRecognition)
    {
        mPlayer = playerTransform;

        mNumberOfRaysLatitude = numberOfRaysLatitude;
        mNumberOfRaysLongitude = numberOfRaysLongitude;

        mInitialLatitudeAngle = initialLatitudeAngle;
        mFinalLatitudeAngle = finalLatitudeAngle;

        mInitialLongitudeAngle = initialLongitudeAngle;
        mFinalLongitudeAngle = finalLongitudeAngle;

        mNumberOfRays = numberOfRaysLatitude * (numberOfRaysLongitude + 1);

        mRaycastDistance = raycastDistance;
        mObjectRecognition = objRecognition;

        mFeatureVector = new double[mObjectRecognition.GetNumberOfObjects() * mNumberOfRays];
        mRaycastValues = new double[mObjectRecognition.GetNumberOfObjects()];
    }


    public double[] GetFeatureVector()
    {
        for( int lat = 0; lat < mNumberOfRaysLatitude; lat++ )
        {
            for( int lon = 0; lon <= mNumberOfRaysLongitude; lon++ )
            {
                DebugDraw(lat, lon);
                double[] raycastValues = GetRaycastAt(lat, lon);

                for (int p = 0; p < mObjectRecognition.GetNumberOfObjects(); p++)
                {
                    mFeatureVector[(lat * (mNumberOfRaysLongitude+1) + lon) * mObjectRecognition.GetNumberOfObjects() + p] = raycastValues[p];
                }
        }

        for (int i = 0; i < mNumberOfRays; i++)
        {
            }
        }

        return mFeatureVector;
    }

    private double[] GetRaycastAt(int lat, int lon)
    {
        float _pi = Mathf.PI;
        float _2pi = _pi * 2f;

        float a1 = mInitialLatitudeAngle + (mFinalLatitudeAngle - mInitialLatitudeAngle) * _pi * (float)(lat+1) / (mNumberOfRaysLatitude+1);
        float sin1 = Mathf.Sin(a1);
        float cos1 = Mathf.Cos(a1);

        float a2 = mInitialLongitudeAngle + (mFinalLongitudeAngle - mInitialLongitudeAngle) * _2pi * (float)(lon == mNumberOfRaysLongitude ? 0 : lon) / mNumberOfRaysLongitude;
        float sin2 = Mathf.Sin(a2);
        float cos2 = Mathf.Cos(a2);
 
        Vector3 direction = new Vector3( sin1 * cos2, cos1, sin1 * sin2 );

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
        for( int lat = 0; lat < mNumberOfRaysLatitude; lat++ )
        {
            for( int lon = 0; lon <= mNumberOfRaysLongitude; lon++ )
            {
                DebugDraw(lat, lon);
            }
        }
    }

    private void DebugDraw(int lat, int lon)
    {
        // Debug.Log("GilLog - Raycast3DPerception::DebugDraw - lat " + lat + " lon " + lon + " ");

        float _pi = Mathf.PI;
        float _2pi = _pi * 2f;

        float a1 = mInitialLatitudeAngle + (mFinalLatitudeAngle - mInitialLatitudeAngle) * _pi * (float)(lat+1) / (mNumberOfRaysLatitude+1);
        float sin1 = Mathf.Sin(a1);
        float cos1 = Mathf.Cos(a1);

        float a2 = mInitialLongitudeAngle + (mFinalLongitudeAngle - mInitialLongitudeAngle) * _2pi * (float)(lon == mNumberOfRaysLongitude ? 0 : lon) / mNumberOfRaysLongitude;
        float sin2 = Mathf.Sin(a2);
        float cos2 = Mathf.Cos(a2);
 
        Vector3 direction = new Vector3( sin1 * cos2, cos1, sin1 * sin2 );

        Debug.DrawRay(mPlayer.position, 20f * direction, Color.green);
    }
}
