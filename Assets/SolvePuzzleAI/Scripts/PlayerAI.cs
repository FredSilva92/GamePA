using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;

public class PlayerAI : Agent
{
    public PlayerTypeAI Player;

    public AgentStatusAI AgentStatusAI { get; set; }

    public PuzzleManager PuzzleManager;

    public BehaviorParameters BehaviourParameters { get; private set; }

    public VectorSensorComponent VectorSensorComponent { get; set; }

    private void Start()
    {
        VectorSensorComponent = GetComponent<VectorSensorComponent>();
        BehaviourParameters = GetComponent<BehaviorParameters>();
        AgentStatusAI = AgentStatusAI.WakingUp;
    }

    public override void Initialize()
    {
        AgentStatusAI = AgentStatusAI.Ready;
    }

    public override void OnEpisodeBegin()
    {
        AgentStatusAI = AgentStatusAI.Ready;
    }

    /*
     * Informações que o agente recebe sobre o ambiente e o estado do jogo atual, antes de tomar uma decisão.
     */
    public override void CollectObservations(VectorSensor sensor)
    {
        // observações para as posições atuais das peças
        // (posição no mundo e posição no puzzle)
        foreach (PuzzlePiece piece in PuzzleManager.Pieces)
        {
            VectorSensorComponent.GetSensor().AddObservation(piece.piece.transform.position);
            VectorSensorComponent.GetSensor().AddObservation(piece.position);
        }

        // observações para o saber o jogador atual
        // (1 para Jogador 1, 2 para Jogador 2)
        if (PuzzleManager.CurrentPlayer == PlayerTypeAI.player1)
        {
            VectorSensorComponent.GetSensor().AddObservation(1);
        }
        else if (PuzzleManager.CurrentPlayer == PlayerTypeAI.player2)
        {
            VectorSensorComponent.GetSensor().AddObservation(2);
        }

        //foreach (int value in PuzzleManager.BoardState)
        //{
        //    if (value == 1)
        //    {
        //        VectorSensorComponent.GetSensor().AddObservation(1f);
        //    }
        //    else
        //    {
        //        VectorSensorComponent.GetSensor().AddObservation(0f);
        //    }
        //}
        //foreach (int value in PuzzleManager.BoardState)
        //{
        //    if (value == 2)
        //    {
        //        VectorSensorComponent.GetSensor().AddObservation(1f);
        //    }
        //    else
        //    {
        //        VectorSensorComponent.GetSensor().AddObservation(0f);
        //    }
        //}
    }

    /*
     * Cada agente toma 2 decisões por turno.
     * Cada decisão é uma ação (número possível que a IA pode escolher).
     * 1º decisão: escolhe o número da peça (ação entre 1 a 9).
     * 2º decisão: verifica se resolveu o puzzle, etc. (representado pela ação 10).
     * Portanto na 1º decisão só são aceites números entre 1 a 9, e na 2º decisão apenas o 10.
    */
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        //bool[] availableGamePlayActions = PuzzleManager.GetAvailableMoves();

        if (PuzzleManager.GameStatusAI == GameStatusAI.PerformingMove)
        {
            //if (PuzzleManager.CurrentPlayer == PlayerTypeAI.player1 && PuzzleManager.Turn == 0 && PuzzleManager.PlayerXRandomFirstTurn)
            //{
            //    if (UnityEngine.Random.Range(0, 10) < 4)
            //    {
            //        for (int i = 1; i < 10; i++)
            //        {
            //            actionMask.SetActionEnabled(0, i, false);
            //        }

            //        int rnd = UnityEngine.Random.Range(1, 10);
            //        actionMask.SetActionEnabled(0, 0, false);
            //        actionMask.SetActionEnabled(0, rnd, true);
            //        actionMask.SetActionEnabled(0, 10, false);
            //    }
            //    else
            //    {
            //        for (int i = 1; i < availableGamePlayActions.Count(); i++)
            //        {
            //            actionMask.SetActionEnabled(0, 0, false);
            //            actionMask.SetActionEnabled(0, i, availableGamePlayActions[i]);
            //            actionMask.SetActionEnabled(0, 10, false);
            //        }
            //    }
            //}
            //else
            //{
            //for (int i = 1; i < availableGamePlayActions.Count(); i++)
            //{
            //    actionMask.SetActionEnabled(0, 0, false);
            //    actionMask.SetActionEnabled(0, i, availableGamePlayActions[i]);
            //    actionMask.SetActionEnabled(0, 10, false);
            //}
            actionMask.SetActionEnabled(0, 0, false);
            for (int i = 1; i <= PuzzleManager.Pieces.Count; i++)
            {
                actionMask.SetActionEnabled(0, i, true);
            }
            actionMask.SetActionEnabled(0, 10, false);
            int a = 2;
            //}
        }
        else if (PuzzleManager.GameStatusAI == GameStatusAI.ObservingMove || PuzzleManager.GameStatusAI == GameStatusAI.MakingFinalObservation)
        {
            //actionMask.SetActionEnabled(0, 0, false);
            //for (int i = 1; i < 10; i++)
            //{
            //    actionMask.SetActionEnabled(0, i, false);
            //}
            //actionMask.SetActionEnabled(0, 10, true);
            for (int i = 0; i <= PuzzleManager.Pieces.Count; i++)
            {
                actionMask.SetActionEnabled(0, i, false);
            }
            actionMask.SetActionEnabled(0, 10, true);
            int a = 2;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActionsOut = actionsOut.DiscreteActions;

        if (PuzzleManager.GameStatusAI == GameStatusAI.PerformingMove)
        {
            if (PuzzleManager.Training)
            {
                //bool[] availableActions = PuzzleManager.GetAvailableMoves();

                //List<int> trueIndices = new List<int>();

                //for (int i = 1; i < 10; i++)
                //{
                //    if (availableActions[i])
                //    {
                //        trueIndices.Add(i);
                //    }
                //}

                //int randomPiece = trueIndices[UnityEngine.Random.Range(0, trueIndices.Count)];
                int randomPiece = UnityEngine.Random.Range(0, PuzzleManager.Pieces.Count);

                discreteActionsOut[0] = randomPiece;
            }
            //else
            //{
            //    discreteActionsOut[0] = PuzzleManager.HeuristicSelectedPiece;
            //}
        }
        else if (PuzzleManager.GameStatusAI == GameStatusAI.ObservingMove || PuzzleManager.GameStatusAI == GameStatusAI.MakingFinalObservation)
        {
            discreteActionsOut[0] = 10;
        }
    }

    public override async void OnActionReceived(ActionBuffers actions)
    {
        int action = actions.DiscreteActions[0];

        bool placedPiece = false;
        //int couldWinThisTurn = 0;
        //int missedBlockThisTurn = 0;
        //bool missedBlock = false;

        if (action > 0 && action < 10)
        {
            //if (Player == PlayerTypeAI.player1)
            //{
            //    couldWinThisTurn = PuzzleManager.CheckCouldWinOnNextMove(PlayerTypeAI.player1);
            //    missedBlockThisTurn = PuzzleManager.CheckCouldWinOnNextMove(PlayerTypeAI.player2);
            //}
            //else
            //{
            //    couldWinThisTurn = PuzzleManager.CheckCouldWinOnNextMove(PlayerTypeAI.player2);
            //    missedBlockThisTurn = PuzzleManager.CheckCouldWinOnNextMove(PlayerTypeAI.player1);
            //}

            placedPiece = await PuzzleManager.PlacePiece(action);

            // se jogada não é válida (número inválido)
            if (!placedPiece)
            {
                PuzzleManager.ResetGame();
            }
            else
            {
                PuzzleManager.GameResultAI = PuzzleManager.CheckGameStatusAI();

                if (PuzzleManager.GameResultAI == GameResultAI.notSolved)
                {
                    //if (couldWinThisTurn > 0)
                    //{
                    //    AddReward(PuzzleManager.Rewards.CouldHaveWon);
                    //}
                    //else
                    //{
                    //if (Player == PlayerTypeAI.player1)
                    //{
                    //    if (missedBlockThisTurn > 0 && missedBlockThisTurn == PuzzleManager.CheckCouldWinOnNextMove(PlayerTypeAI.player2))
                    //    {
                    //        missedBlock = true;
                    //    }
                    //}
                    //else
                    //{
                    //    if (missedBlockThisTurn > 0 && missedBlockThisTurn == PuzzleManager.CheckCouldWinOnNextMove(PlayerTypeAI.player1))
                    //    {
                    //        missedBlock = true;
                    //    }
                    //}
                    //}

                    //if (missedBlock)
                    //{
                    //    AddReward(PuzzleManager.Rewards.CouldHaveBlocked);
                    //}

                    PuzzleManager.GameStatusAI = GameStatusAI.ObserveMove;
                }
                else if (PuzzleManager.GameResultAI == GameResultAI.solved)
                {
                    PuzzleManager.GameStatusAI = GameStatusAI.GiveRewards;
                }
            }
        }
        else if (action == 10)
        {
            if (PuzzleManager.GameStatusAI == GameStatusAI.ObservingMove)
            {
                PuzzleManager.GameStatusAI = GameStatusAI.ChangePlayer;
            }
            else if (PuzzleManager.GameStatusAI == GameStatusAI.MakingFinalObservation)
            {
                AgentStatusAI = AgentStatusAI.MadeFinalObservation;
            }
        }
        else
        {
            PuzzleManager.ResetGame();
        }
    }
}