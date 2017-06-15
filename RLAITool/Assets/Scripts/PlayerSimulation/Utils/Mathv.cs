using UnityEngine;
using System.Collections;

public static class Mathv {
	public static float Max(float[] values)
	{
		float v = values[0];

		for (int i = 1; i < values.Length; i++)
		{
			if (values[i] > v)
			{
				v = values[i];
			}
		}

		return v;
	}
}
