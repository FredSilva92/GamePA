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
    none,
    xWon,
    oWon,
    draw,
    solved,
    notSolved
}

public enum MenuGroupAI
{
    Default = 0,
    Sensors = 50,
    Actuators = 100
}