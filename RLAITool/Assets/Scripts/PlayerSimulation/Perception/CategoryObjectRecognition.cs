using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CategoryObject
{
	public string CategoryName;
	public string[] Tags;
	public string[] ObjectNames;
}

public class CategoryObjectRecognition : IObjectRecognition {

	private int mNumberOfCategories;
	private CategoryObject[] mCategories;

	public CategoryObjectRecognition(CategoryObject[] categories)
	{
		if (categories != null && categories.Length > 0)
		{
			mNumberOfCategories = categories.Length;

			mCategories = categories;
		}
	}

	public int GetNumberOfObjects()
	{
		return mNumberOfCategories;
	}

	public int GetId(GameObject gameObject)
	{
		for (int i = 0; i < mCategories.Length; i++)
        {
        	CategoryObject category = mCategories[i];

        	for (int t = 0; category.Tags != null && t < category.Tags.Length; t++)
        	{
	            if (gameObject.tag == category.Tags[t])
	            {
	                return i;
	            }
        	}

        	for (int n = 0; category.ObjectNames != null && n < category.ObjectNames.Length; n++)
        	{
	            if (gameObject.name == category.ObjectNames[n])
	            {
	                return i;
	            }
        	}
        }

        return -1;
	}
}
