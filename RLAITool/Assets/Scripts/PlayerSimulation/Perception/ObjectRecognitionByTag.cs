using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class ObjectRecognitionByTag : IObjectRecognition
{
    private string[] mTags;

    public ObjectRecognitionByTag(string[] tags)
    {
        mTags = tags;
    }

    public int GetId(GameObject gameObject)
    {
        for (int i = 0; i < mTags.Length; i++)
        {
            if (gameObject.tag == mTags[i])
            {
                return i;
            }
        }

        return -1;
    }

    public int GetNumberOfObjects()
    {
        return mTags.Length;
    }
}
