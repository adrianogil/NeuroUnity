using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class NeuralNetwork
{
    private NeuralLayer[] mLayers = null;
    private int[] p1;
    private int p2;

    public NeuralNetwork(int[] neuronsByLayer, int inputCount, System.Random randObj)
    {
        mLayers = new NeuralLayer[neuronsByLayer.Length];

        int layerInput = inputCount;

        for (int i = 0; i < neuronsByLayer.Length; i++)
        {
            mLayers[i] = new NeuralLayer(neuronsByLayer[i], layerInput, randObj);

            layerInput = neuronsByLayer[i];
        }
    }

    public float[] Evaluate(float[] input)
    {
        float[] output = input;

        for (int i = 0; i < mLayers.Length; i++)
        {
            output = mLayers[i].Evaluate(output);
        }

        return output;
    }

    public float[] GetTotalWeights()
    {
        List<float> totalWeights = new List<float>();

        foreach (NeuralLayer layer in mLayers)
        {
            foreach (Neuron neuron in layer.Neurons)
            {
                foreach (float w in neuron.Weights)
                {
                    totalWeights.Add(w);
                }
            }
        }

        return totalWeights.ToArray();
    }

	public void SetTotalWeights(float[] weights)
	{
		List<float> totalWeights = weights.ToList();

		int index = 0;

		foreach (NeuralLayer layer in mLayers)
		{
			foreach (Neuron neuron in layer.Neurons)
			{
				neuron.Weights = totalWeights.GetRange(index, neuron.Weights.Length).ToArray();
				index += neuron.Weights.Length;
			}
		}
	}
}

public class NeuralLayer
{
    private Neuron[] mNeurons;

    public Neuron[] Neurons { get { return mNeurons; } }

	public NeuralLayer(int neuronsCount, int inputCount, System.Random randObj)
    {
        mNeurons = new Neuron[neuronsCount];

        for (int n = 0; n < mNeurons.Length; n++)
        {
            mNeurons[n] = new Neuron(inputCount, randObj);
        }
    }

    public float[] Evaluate(float[] input)
    {
        float[] output = new float[mNeurons.Length];

        for (int n = 0; n < mNeurons.Length; n++)
        {
            output[n] = mNeurons[n].Evaluate(input);
        }

        return output;
    }
}

public class Neuron
{
    private float[] mInputs = null;
    private float[] mWeights = null;

    public float[] Inputs
    {
        get { return mInputs; }
        set { mInputs = value; }
    }

    public float[] Weights
    {
        get { return mWeights; }
        set { mWeights = value; }
    }

	public Neuron(int inputCount, System.Random randObj)
    {
        mWeights = new float[inputCount + 1]; // Weight plus bias 

        for (int i = 0; i < inputCount; i++)
        {
            mWeights[i] = (((float)randObj.NextDouble()) * 2.0f) - 1.0f;
        }
    }

    public float Evaluate(float[] input)
    {
        float output = 0.0f;

		//Debug.Log("Evaluate: " + input.Length + " / " + mWeights.Length);

        for (int n = 0; n < mWeights.Length-1; n++)
        {
            output += input[n] * mWeights[n];
        }

		// Sum bias
		output += mWeights[mWeights.Length-1];

        // Calculate the sigmoid value
        output = (float) (1 / (1 + Math.Exp(-output)));

        return output;
    }
}
