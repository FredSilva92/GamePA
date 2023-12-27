public enum PlayerTypeAI
{
    player1,
    player2
}

public enum AgentStatusAI
{
    WakingUp,
    Ready,
    Working,
    MadeFinalObservation,
    EndingGame,
    Resetting
}

public enum GameStatusAI
{
    WaitingToStart,
    WaitingOnHuman,
    ReadyToMove,
    PerformingMove,
    ObserveMove,
    ObservingMove,
    ChangePlayer,
    ChangingPlayer,
    GiveRewards,
    GivingRewards,
    FinalObservation,
    MakingFinalObservation,
    EndingGame
}

public enum GameResultAI
{
    notSolved,
    solved
}