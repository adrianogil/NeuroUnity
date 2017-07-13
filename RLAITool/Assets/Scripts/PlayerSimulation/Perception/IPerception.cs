using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IPerception
{
    double[] GetFeatureVector();

    int GetFeatureSize();

    void DebugDraw();
}
