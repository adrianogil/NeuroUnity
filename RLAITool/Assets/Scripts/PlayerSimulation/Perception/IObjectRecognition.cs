using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public interface IObjectRecognition
{
    int GetNumberOfObjects();

	int GetId(GameObject gameObject);
}
