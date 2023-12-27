using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;

public class PlayerAI : Agent
{
    /* ATRIBUTOS */

    public PlayerTypeAI Player;

    public AgentStatusAI AgentStatusAI { get; set; }

    public BehaviorParameters BehaviourParameters { get; private set; }

    public VectorSensorComponent VectorSensorComponent { get; set; }

    public PuzzleManager PuzzleManager;


    /* M�TODOS */

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
     * Define informa��es que o agente recebe sobre o ambiente e o estado do jogo atual, antes de tomar uma decis�o.
     */
    public override void CollectObservations(VectorSensor sensor)
    {
        // observa��es para as posi��es atuais das pe�as
        // (posi��o no mundo e posi��o no puzzle)
        foreach (PuzzlePiece piece in PuzzleManager.Pieces)
        {
            VectorSensorComponent.GetSensor().AddObservation(piece.piece.transform.position);
            VectorSensorComponent.GetSensor().AddObservation(piece.position);
        }

        // observa��es para o saber o jogador atual
        // (1 para Jogador 1, 2 para Jogador 2)
        if (PuzzleManager.CurrentPlayer == PlayerTypeAI.player1)
        {
            VectorSensorComponent.GetSensor().AddObservation(1);
        }
        else if (PuzzleManager.CurrentPlayer == PlayerTypeAI.player2)
        {
            VectorSensorComponent.GetSensor().AddObservation(2);
        }

        // observa��es para o saber a �ltima escolha de um n�mero
        // (-1 se escolha ainda n�o foi definida)
        if (PuzzleManager.LastChoice != -1)
        {
            VectorSensorComponent.GetSensor().AddObservation(PuzzleManager.LastChoice);
            PuzzleManager.LastChoice = -1;
        }
    }

    /*
     * Cada agente toma 2 decis�es por turno.
     * Cada decis�o � uma a��o (n�mero poss�vel que a IA pode escolher).
     * 1� decis�o: escolhe o n�mero da pe�a (a��o entre 1 a 9),
     * mas usamos "availablePiecesToOrder" pois s� � preciso escolher as pe�as que ainda n�o est�o no s�tio correto.
     * 2� decis�o: verifica se resolveu o puzzle, etc. (representado pela a��o 10).
     * Portanto na 1� decis�o s� s�o aceites n�meros entre 1 a 9, e na 2� decis�o apenas o 10.
    */
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        bool[] availablePiecesToOrder = PuzzleManager.GetAvailablePiecesToOrder();

        if (PuzzleManager.GameStatusAI == GameStatusAI.PerformingMove)
        {
            for (int i = 1; i < availablePiecesToOrder.Length; i++)
            {
                actionMask.SetActionEnabled(0, 0, false);
                actionMask.SetActionEnabled(0, i, availablePiecesToOrder[i]);
                actionMask.SetActionEnabled(0, 10, false);
            }
        }
        else if (PuzzleManager.GameStatusAI == GameStatusAI.ObservingMove || PuzzleManager.GameStatusAI == GameStatusAI.MakingFinalObservation)
        {
            for (int i = 0; i <= PuzzleManager.Pieces.Count; i++)
            {
                actionMask.SetActionEnabled(0, i, false);
            }

            actionMask.SetActionEnabled(0, 10, true);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActionsOut = actionsOut.DiscreteActions;

        if (PuzzleManager.GameStatusAI == GameStatusAI.PerformingMove)
        {
            if (PuzzleManager.Training)
            {
                bool[] availableActions = PuzzleManager.GetAvailablePiecesToOrder();

                List<int> trueIndices = new List<int>();

                for (int i = 1; i < availableActions.Length; i++)
                {
                    if (availableActions[i])
                    {
                        trueIndices.Add(i);
                    }
                }

                int randomPiece = trueIndices[UnityEngine.Random.Range(0, trueIndices.Count)];

                discreteActionsOut[0] = randomPiece;
            }
            else
            {
                int a = 3;
                //discreteActionsOut[0] = PuzzleManager.HeuristicSelectedPiece;
            }
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
        bool samePiece = false;

        if (action > 0 && action < 10)
        {
            PuzzleManager.LastChoice = action;

            if (Player == PlayerTypeAI.player1)
            {
                PuzzleManager.LastFirstChoice = action;
            }
            else if (Player == PlayerTypeAI.player2)
            {
                PuzzleManager.LastSecondChoice = action;

                if (PuzzleManager.LastFirstChoice == action)
                {
                    samePiece = true;
                }
                else
                {
                    samePiece = false;
                }
            }

            //if (Player == PlayerTypeAI.player1)
            //{
            //    couldWinThisTurn = PuzzleManager.CheckCouldWinOnNextMove(PlayerTypeAI.player1);
            //    missedBlockThisTurn = PuzzleManager.CheckCouldWinOnNextMove(PlayerTypeAI.player2);
            //}
            //else if (Player == PlayerTypeAI.player2)
            //{
            //    couldWinThisTurn = PuzzleManager.CheckCouldWinOnNextMove(PlayerTypeAI.player2);
            //    missedBlockThisTurn = PuzzleManager.CheckCouldWinOnNextMove(PlayerTypeAI.player1);
            //}

            placedPiece = await PuzzleManager.PlacePiece(action);

            // se jogada n�o � v�lida (n�mero inv�lido)
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
                    //    if (Player == PlayerTypeAI.player1)
                    //    {
                    //        if (missedBlockThisTurn > 0 && missedBlockThisTurn == PuzzleManager.CheckCouldWinOnNextMove(PlayerTypeAI.player2))
                    //        {
                    //            missedBlock = true;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (missedBlockThisTurn > 0 && missedBlockThisTurn == PuzzleManager.CheckCouldWinOnNextMove(PlayerTypeAI.player1))
                    //        {
                    //            missedBlock = true;
                    //        }
                    //    }
                    //}

                    //if (missedBlock)
                    //{
                    //    AddReward(PuzzleManager.Rewards.CouldHaveBlocked);
                    //}

                    if (Player == PlayerTypeAI.player2)
                    {
                        if (samePiece)
                        {
                            AddReward(PuzzleManager.RewardsAI.SAME_PIECE);
                        }
                        else
                        {
                            AddReward(PuzzleManager.RewardsAI.DIFFERENT_PIECCE);
                        }
                    }

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