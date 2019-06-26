using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public enum GAFitnessMode
{
    Strict = 0,
    Lazy = 1
};

public class GeneticAlgorithm
{
    public GAFitnessMode fitnessMode = GAFitnessMode.Strict;
}

/// <summary>
/// This is a set of chromosomes
/// </summary>
public class Population
{
    private Chromosome[] mChromosomes;
    private int mCurrentChromosome;
    public int mCurrentPopulation;
    private double mBestFitness = -10000;

    public Chromosome BestChromosome;
	public Chromosome CurrentChromosome { get { return mChromosomes[mCurrentChromosome]; }}
    public int BestPopulation;

    public int CurrentPopulation { get { return mCurrentPopulation; } }
    public int CurrentChromosomeID { get { return mCurrentChromosome; } }
    public double BestFitness { get { return mBestFitness; } }

    public static string SAVE_FILE_NAME = "rlai_neuroevolutive.params";

    public static string AgentSaveFileName = "";

    public static void SetAgentName(string name)
    {
        AgentSaveFileName = name;
        SAVE_FILE_NAME = "rlai_neuroevolutive_" + AgentSaveFileName + ".params";
    }

    public Population(int chromosomeCount, int weightCount, System.Random randObj)
    {
        mChromosomes = new Chromosome[chromosomeCount];

        for (int c = 0; c < chromosomeCount; c++)
        {
            mChromosomes[c] = new Chromosome(weightCount, randObj);
        }

        mCurrentPopulation = 0;
        mCurrentChromosome = 0;
        BestChromosome = mChromosomes[0];
        BestPopulation = 0;
    }

	public bool IsLastChromosome()
	{
		return mCurrentChromosome == (mChromosomes.Length-1);
	}

	public void NextChromosome()
	{
        SetBestChromosome();
		mCurrentChromosome++;
	}

    public void LastChromosome()
    {
        if (mCurrentChromosome > 0)
        {
            mCurrentChromosome--;
        }
    }

	public void Save()
	{
		string[] lines  = new string[(BestChromosome.Weights.Length+1) * (mChromosomes.Length+1) + 4];

		lines[0] = mChromosomes.Length.ToString();
		lines[1] = BestChromosome.Weights.Length.ToString();

		int i  = 2;
		foreach (Chromosome chromosome in mChromosomes)
		{
			foreach (double weight in chromosome.Weights)
			{
				lines[i] = weight.ToString();
				i++;
			}
			lines[i] = chromosome.Fitness.ToString();
			i++;
		}

		foreach (double weight in BestChromosome.Weights)
		{
			lines[i] = weight.ToString();
			i++;
		}
		lines[i] = BestChromosome.Fitness.ToString();

        i++;
        lines[i] = CurrentChromosomeID.ToString();

        i++;
        lines[i] = mCurrentPopulation.ToString();

		//System.IO.File.Exists(
		System.IO.File.WriteAllLines(Application.persistentDataPath + "/" + SAVE_FILE_NAME, lines);
	}

	public static Population Restore(System.Random randObj) {

        Debug.Log(Application.persistentDataPath + "/" + SAVE_FILE_NAME);

        if (!System.IO.File.Exists(Application.persistentDataPath + "/" + SAVE_FILE_NAME))
		{
			return null;
		}

		return new Population(randObj);
	}

	private Population(System.Random randObj)
	{
        string[] lines = System.IO.File.ReadAllLines(Application.persistentDataPath + "/" + SAVE_FILE_NAME);

        SetupPopulation(randObj, lines);
    }

    private Population(System.Random randObj, TextAsset textAsset)
    {
        char[] archDelim = new char[] { '\r', '\n' };

        string[] lines = textAsset.text.Split(archDelim, StringSplitOptions.RemoveEmptyEntries);

        SetupPopulation(randObj, lines);
    }

    private void SetupPopulation(System.Random randObj, string[] lines)
    {
		int chromosomeCount = (int)double.Parse(lines[0]);
		int weightCount = (int)double.Parse(lines[1]);

		mChromosomes = new Chromosome[chromosomeCount];

		for (int c = 0; c < chromosomeCount; c++)
		{
			mChromosomes[c] = new Chromosome(weightCount, randObj);
		}

		mCurrentPopulation = 0;
		mCurrentChromosome = 0;
		BestPopulation = 0;

		int index = 2;

		for (int c = 0; c < chromosomeCount; c++)
		{
			double[] weights = new double[weightCount];
			for (int i = 0; i < weightCount; i++)	{
				weights[i] = double.Parse(lines[index]);
				index++;
			}
			mChromosomes[c].Weights = weights;
			mChromosomes[c].Fitness = double.Parse(lines[index]);
			index++;
		}

		BestChromosome = new Chromosome(weightCount, randObj);

		double[] bestWeights = new double[weightCount];
		for (int i = 0; i < weightCount; i++)	{
			bestWeights[i] = double.Parse(lines[index]);
			index++;
		}

		BestChromosome.Weights = bestWeights;
		BestChromosome.Fitness = double.Parse(lines[index]);

        mBestFitness = BestChromosome.Fitness;

        index++;
        if (lines.Length > index)
        {
            mCurrentChromosome = (int)double.Parse(lines[index]);
        }

        index++;
        if (lines.Length > index)
        {
            mCurrentPopulation = (int)double.Parse(lines[index]);
        }
	}


	/* save the best chromosome by writing all its weights on "bestchr.txt" */
	public void SaveBestChromosome()
	{
		string[] lines  = new string[BestChromosome.Weights.Length];
		int i  = 0;
		foreach (double weight in BestChromosome.Weights)
		{
			lines[i] = weight.ToString();
			i++;
		}

		System.IO.File.WriteAllLines("bestchr.txt", lines);
	}

	/* returns the best chromosome by reading all its weights from "bestchr.txt" */
	public double[] RestoreBestChromosome() {
		string[] lines = System.IO.File.ReadAllLines("bestchr.txt");
		double[] weights = new double[lines.Length];
		for (int i = 0; i < lines.Length; i++)	{
			weights[i] = double.Parse(lines[i]);
		}
		return weights;
	}

    public void NewGeneration(System.Random randObj)
    {
        mCurrentPopulation++;

        ResetCurrentChromosome();

        Chromosome[] newChromosomes = new Chromosome[mChromosomes.Length];
        double crossOverProbability = 0.85f;

        for (int i = 0; i < mChromosomes.Length-1; i=i+2)
        {
            // new chromosomes are chosen with Roulette Wheel method
            Chromosome firstChromosome = RouletteWheel(randObj);
            Chromosome secondChromosome = RouletteWheel(randObj);

            if (randObj.NextDouble() <= crossOverProbability)
            {
                // Perform a crossover
                Chromosome[] chromosomePair = CrossOver(firstChromosome, secondChromosome, randObj);

                newChromosomes[i] = chromosomePair[0];
                newChromosomes[i + 1] = chromosomePair[1];
            }
            else
            {
                // If not a crossover, just copy the 2 chromosomes taken with the Roulette Wheel method
                newChromosomes[i] = firstChromosome;
                newChromosomes[i + 1] = secondChromosome;
            }

            // in both cases, try a mutation of each chromosome's weights with a low probability
            newChromosomes[i] = Mutate(newChromosomes[i], randObj);
            newChromosomes[i + 1] = Mutate(newChromosomes[i + 1], randObj);
        }

        mChromosomes = newChromosomes;
    }

    public void ResetCurrentChromosome()
    {
        mCurrentChromosome = 0;
    }

    private void SetBestChromosome()
    {
        if (mChromosomes[mCurrentChromosome].Fitness > mBestFitness)
        {
            mBestFitness = mChromosomes[mCurrentChromosome].Fitness;
            BestChromosome = mChromosomes[mCurrentChromosome];
        }
    }

    /// <summary>
    /// Creates 2 new chromosomes (offspring) by corssovering 2 input chromosomes
    /// </summary>
    /// <param name="firstChromosome">'firstChromosome' is like the dad</param>
    /// <param name="secondChromosome">'secondChromosome' is like the mum</param>
    /// <param name="randObj"></param>
    /// <returns></returns>
	private Chromosome[] CrossOver(Chromosome firstChromosome, Chromosome secondChromosome, System.Random randObj)
    {
        int totalWeights = firstChromosome.Weights.Length;
        int crossingPoints = randObj.Next(0, totalWeights - 1); // Choose a random crossing point

        double[] weights1 = new double[totalWeights];
        double[] weights2 = new double[totalWeights];

        for (int i = 0; i < totalWeights; i++)
        {
            if (i < crossingPoints)
            {
                weights1[i] = firstChromosome.Weights[i];
                weights2[i] = secondChromosome.Weights[i];
            }
            else
            {
                weights1[i] = secondChromosome.Weights[i];
                weights2[i] = firstChromosome.Weights[i];
            }
        }

        Chromosome[] chromosomePair = new Chromosome[2];
        chromosomePair[0] = new Chromosome(weights1);
        chromosomePair[1] = new Chromosome(weights2);

        return chromosomePair;
    }

    /// <summary>
    /// Roulette Wheel" is a method to extract a chromosome by looking at its fitness value:
    /// If some chromosome's fitness is higher than another, then that chromosome has more
    /// possibilities to be taken
    /// - [SUM] calculate sum of all chromosome fitness in population -> S
    /// - [SELECT] Generate random number from interval (0,S) -> r
    /// - [LOOP] GO through the population and sum fitness from 0 to S -> s
    /// When the sim s is greater than r, stop and return the chromosome where you are
    /// </summary>
    /// <param name="randObj"></param>
    /// <returns></returns>
	private Chromosome RouletteWheel(System.Random randObj)
    {
        double fitnessSum = 0;
        double randomNum;
        int selectedChromosome = 0;

        for (int c = 0; c < mChromosomes.Length; c++)
        {
            fitnessSum += mChromosomes[c].Fitness;
        }

        randomNum = (double) (randObj.NextDouble() * fitnessSum);
        fitnessSum = 0;

        for (int c = 0; c < mChromosomes.Length; c++)
        {
            fitnessSum += mChromosomes[c].Fitness;

            if (fitnessSum > randomNum)
            {
                break;
            }
            else
            {
                selectedChromosome++;
            }
        }

        return mChromosomes[Math.Min(mChromosomes.Length-1, selectedChromosome)];
    }

    /// <summary>
    /// Perform a random mutation of a chromosome
    /// </summary>
    /// <param name="chromosome"></param>
    /// <param name="randObj"></param>
    /// <returns></returns>
	private Chromosome Mutate(Chromosome chromosome, System.Random randObj)
    {
        double mutationProbability = 0.008f; // each weight has a low probability to be mutated

        for (int w = 0; w < chromosome.Weights.Length; w++)
        {
            if (randObj.NextDouble() <= mutationProbability)
            {
                chromosome.Weights[w] += (double)((randObj.NextDouble() * 2.0) - 1.0);
            }
        }

        return chromosome;
    }

    public void SaveFitnessValue(int chromosomeId, Chromosome chromosome, double lastFitness, System.Random randObj)
    {
        Population mGeneticPopulation = Population.Restore(randObj);

        if (mGeneticPopulation == null)
        {
            Save();
        }
        else
        {
            if (mGeneticPopulation.CurrentPopulation == CurrentPopulation)
            {
                mGeneticPopulation.mChromosomes[chromosomeId].Fitness = lastFitness;
            }

            if (mGeneticPopulation.BestFitness < lastFitness)
            {
                mGeneticPopulation.mBestFitness = lastFitness;
                mGeneticPopulation.BestChromosome = chromosome;
            }

            mGeneticPopulation.Save();
        }
    }

    public double LastFitness {
        get
        {
            if (mCurrentChromosome == 0)
            {
                return double.MinValue;
            }

            return mChromosomes[mCurrentChromosome - 1].Fitness;
        }
    }
}

public class Chromosome
{
    private double mFitness;
    private double[] mWeights;

    #region Properties
    public double Fitness
    {
        get { return mFitness; }
        set { mFitness = value; }
    }

    public double[] Weights
    {
        get { return mWeights; }
        set { mWeights = value; }
    }
    #endregion

	public Chromosome(int weightCount, System.Random randObj)
    {
        mFitness = 0;
        mWeights = new double[weightCount];

        for (int w = 0; w < weightCount; w++)
        {
            mWeights[w] = (double)((randObj.NextDouble() * 2.0) - 1.0f);
        }
    }

    public Chromosome(double[] weights)
    {
        mFitness = 0;
        mWeights = weights;
    }
}
