public interface ILearningAgent
{
    void SetupPerception(IPerception perception);
    void SetupOutput(IAgentOutput agentOutput);

    void InitializeLearningMethod();
    void UpdateAgentReasoning();
}