// AForge Neural Net Library
//
// Copyright � Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

using System;
using System.Collections.Generic;

namespace AForge.Neuro
{
	
	/// <summary>
	/// Base neural network class
	/// </summary>
	/// 
	/// <remarks>This is a base neural netwok class, which represents
	/// collection of neuron's layers.</remarks>
	/// 
	public abstract class Network
	{
		/// <summary>
		/// Network's inputs count
		/// </summary>
		protected int	inputsCount;

		/// <summary>
		/// Network's layers count
		/// </summary>
		protected int	layersCount;

		/// <summary>
		/// Network's layers
		/// </summary>
		protected Layer[]	layers;

		/// <summary>
		/// Network's output vector
		/// </summary>
		protected double[]	output;

		/// <summary>
		/// Network's inputs count
		/// </summary>
		public int InputsCount
		{
			get { return inputsCount; }
		}

		/// <summary>
		/// Network's layers count
		/// </summary>
		public int LayersCount
		{
			get { return layersCount; }
		}

		/// <summary>
		/// Network's output vector
		/// </summary>
		/// 
		/// <remarks>The calculation way of network's output vector is determined by
		/// inherited class.</remarks>
		/// 
		public double[] Output
		{
			get { return output; }
		}

		/// <summary>
		/// Network's layers accessor
		/// </summary>
		/// 
		/// <param name="index">Layer index</param>
		/// 
		/// <remarks>Allows to access network's layer.</remarks>
		/// 
		public Layer this[int index]
		{
			get { return layers[index]; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="Network"/> class
		/// </summary>
		/// 
		/// <param name="inputsCount">Network's inputs count</param>
		/// <param name="layersCount">Network's layers count</param>
		/// 
		/// <remarks>Protected constructor, which initializes <see cref="inputsCount"/>,
		/// <see cref="layersCount"/> and <see cref="layers"/> members.</remarks>
		/// 
		protected Network( int inputsCount, int layersCount )
		{
			this.inputsCount = Math.Max( 1, inputsCount );
			this.layersCount = Math.Max( 1, layersCount );
			// create collection of layers
			layers = new Layer[this.layersCount];
		}

		/// <summary>
		/// Compute output vector of the network
		/// </summary>
		/// 
		/// <param name="input">Input vector</param>
		/// 
		/// <returns>Returns network's output vector</returns>
		/// 
		/// <remarks>The actual network's output vecor is determined by inherited class and it
		/// represents an output vector of the last layer of the network. The output vector is
		/// also stored in <see cref="Output"/> property.</remarks>
		/// 
		public virtual double[] Compute( double[] input )
		{
			output = input;

			// compute each layer
			foreach ( Layer layer in layers )
			{
				output = layer.Compute( output );
			}

			return output;
		}
		
		public virtual int GetTotalNeurons()
		{
			int totalNeurons = 0;

			foreach (Layer layer in layers)
			{
				totalNeurons += layer.NeuronsCount;
			}

			return totalNeurons;
		}

		public virtual double[] GetWeights()
		{
			List<double> neuralWeights = new List<double>();

			foreach (Layer layer in layers)
			{
				for (int n = 0; n < layer.NeuronsCount; n++)
				{
					Neuron neuron = layer[n];

					if (neuron is ActivationNeuron)
					{
						neuralWeights.Add(((ActivationNeuron)neuron).Threshold);	
					}
				}

				for (int n = 0; n < layer.NeuronsCount; n++)
				{
					Neuron neuron = layer[n];

					for (int w = 0; w < neuron.InputsCount; w++)
					{
						neuralWeights.Add(neuron[w]);
					}
				}
			}

			return neuralWeights.ToArray();
		}

		public virtual void SetWeights(double[] weights)
		{
			int index = 0;

			foreach (Layer layer in layers)
			{
				for (int n = 0; n < layer.NeuronsCount; n++)
				{
					Neuron neuron = layer[n];

					if (neuron is ActivationNeuron)
					{
						((ActivationNeuron)neuron).Threshold = weights[index++];	
					}
				}

				for (int n = 0; n < layer.NeuronsCount; n++)
				{
					Neuron neuron = layer[n];

					for (int w = 0; w < neuron.InputsCount; w++)
					{
						neuron[w] = weights[index++];
					}
				}
			}
		}

		/// <summary>
		/// Randomize layers of the network
		/// </summary>
		/// 
		/// <remarks>Randomizes network's layers by calling <see cref="Layer.Randomize"/> method
		/// of each layer.</remarks>
		/// 
		public virtual void Randomize( )
		{
			foreach ( Layer layer in layers )
			{
				layer.Randomize();
			}
		}
	}
}
