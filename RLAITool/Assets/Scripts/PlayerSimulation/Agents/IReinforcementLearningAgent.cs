public interface IReinforcementLearningAgent : ILearningAgent
{
    void OnSimulationOver(float reinforcementScore);
}