using UnityEngine;

using System;
using System.Collections;

public class ObjectPerception
{
	public Collider2D collider;
	public Vector3 position;
	public Vector3 size;
	public int nearestOrder = 1;
}

public class CircleRaycast2DPerception : IPerception {

	private Transform mPlayer;
    private int mNumberOfGameElements;
	private double mRaycastRadius;
    private double[] mFeatureVector;
    private double[] mAttributesValues;

    private IObjectRecognition mObjectRecognition;

    private int mNumberOfCategories;
    private int mFeatureVectorSize;
    private int mNumberOfHits;
    private int mTotalNumberOfAttributes;

    public Collider2D[] mColliders2D;

    public CircleRaycast2DPerception(Transform playerTransform, 
    								 int numberOfGameElements, 
    								 double raycastRadius, 
    								 IObjectRecognition objRecognition)
    {
        mPlayer = playerTransform;
        mNumberOfGameElements = numberOfGameElements;
		mRaycastRadius = raycastRadius;
        mObjectRecognition = objRecognition;

        mNumberOfCategories = mObjectRecognition.GetNumberOfObjects();
        mTotalNumberOfAttributes = mNumberOfCategories + 4; // + diffX, diffY, collider width and height

        mFeatureVectorSize = mTotalNumberOfAttributes * mNumberOfGameElements;

        mFeatureVector = new double[mFeatureVectorSize];
        mAttributesValues = new double[mFeatureVectorSize];

        mNumberOfHits = numberOfGameElements*2;
        mColliders2D = new Collider2D[mNumberOfHits];
    }


    public double[] GetFeatureVector()
    {
        // Initialize attributes
        for (int r = 0; r < mFeatureVector.Length; r++)
        {
            mFeatureVector[r] = -1;
        }

    	ObjectPerception[] gameElementsPerception = GetObjectPerception();

        for (int i = 0; i < mNumberOfGameElements && i < gameElementsPerception.Length; i++)
        {
            double[] raycastValues = GetAttributesFromHit(gameElementsPerception[i]);

            for (int p = 0; p < mTotalNumberOfAttributes; p++)
            {
                mFeatureVector[i * mTotalNumberOfAttributes + p] = raycastValues[p];
            }
        }

        return mFeatureVector;
    }

    private ObjectPerception[] GetObjectPerception()
    {
    	ObjectPerception[] objects = new ObjectPerception[mNumberOfHits];

    	int numberOfResults = Physics2D.OverlapCircleNonAlloc(mPlayer.position, 
                                                              (float) mRaycastRadius,
                                                              mColliders2D, 
                                                              (1 << LayerMask.NameToLayer(Layers.Platforms_Default)) |
                                                              (1 << LayerMask.NameToLayer(Layers.Collectibles))
                                                           );

    	for (int i = 0; i < numberOfResults; i++)
    	{
            if (mColliders2D[i] != null && mColliders2D[i].gameObject != null)
            {
        		objects[i] = new ObjectPerception()
        		{ 
        			collider = mColliders2D[i],
        			position = mColliders2D[i].gameObject.transform.position,
        			size = mColliders2D[i].bounds.size
        		};
            }
    	}

    	return objects;
    } 

    private double[] GetAttributesFromHit(ObjectPerception objectPerception)
    {
        // Initialize attributes
        for (int r = 0; r < mAttributesValues.Length; r++)
        {
            mAttributesValues[r] = -1;
        }

        if (objectPerception == null)
        {
            return mAttributesValues;
        }

        Debug.DrawRay(mPlayer.position, objectPerception.position - mPlayer.position); 

        if (objectPerception.collider != null && objectPerception.collider.gameObject != null)
        {
			int objectId = mObjectRecognition.GetId(objectPerception.collider.gameObject);

			//Debug.Log(hit.collider.gameObject.name);

			if (objectId >= 0 && objectId < mNumberOfCategories)
			{
                mAttributesValues[objectId] = 1;

                double diffX = (double) (objectPerception.position.x - mPlayer.position.x);
                double diffY = (double) (objectPerception.position.y - mPlayer.position.y);
                double colliderWidth = (double) (objectPerception.size.x);
                double colliderHeight = (double) (objectPerception.size.y);

				
				mAttributesValues[mNumberOfCategories] = diffX;
                mAttributesValues[mNumberOfCategories+1] = diffY;
                mAttributesValues[mNumberOfCategories+2] = colliderWidth;
                mAttributesValues[mNumberOfCategories+3] = colliderHeight;
			}
        }

        return mAttributesValues;
    }


    public int GetFeatureSize()
    {
        return mFeatureVectorSize;
    }
}
