//using TMPro;
//using Unity.MLAgents.Policies;
//using UnityEngine;

//public class HumanPlayer : MonoBehaviour
//{
//    public PuzzleManager PuzzleManager;

//    public TextMeshPro playerOText;

//    public TextMeshPro playerXText;

//    public TextMeshPro playerStarted;

//    void Start()
//    {
//        SetPlayerText();
//    }

//    private void SetPlayerText()
//    {
//        if (PuzzleManager.Player1.BehaviourParameters.BehaviorType == BehaviorType.HeuristicOnly)
//        {
//            playerXText.text = "PlayerX\nHeuristics";
//            playerXText.color = Color.white;
//        }
//        else
//        {
//            playerXText.text = "PlayerX\nInference";
//            playerXText.color = Color.blue;
//        }
//        if (PuzzleManager.Player2.BehaviourParameters.BehaviorType == BehaviorType.HeuristicOnly)
//        {
//            playerOText.text = "PlayerO\nHeuristics";
//            playerOText.color = Color.white;
//        }
//        else
//        {
//            playerOText.text = "PlayerO\nInference";
//            playerOText.color = Color.blue;
//        }
//    }

//    void Update()
//    {
//        if (PuzzleManager.AgentsWorking && PuzzleManager.Player1.BehaviourParameters.BehaviorType != BehaviorType.HeuristicOnly)
//        {
//            if (PuzzleManager.CurrentPlayer == PlayerAI.player1 && PuzzleManager.GameStatus == GameStatus.WaitingOnHuman)
//            {
//                PuzzleManager.MakeAIMove();
//            }
//        }

//        if (PuzzleManager.AgentsWorking && PuzzleManager.Player2.BehaviourParameters.BehaviorType != BehaviorType.HeuristicOnly)
//        {
//            if (PuzzleManager.CurrentPlayer == PlayerAI.player2 && PuzzleManager.GameStatus == GameStatus.WaitingOnHuman)
//            {
//                PuzzleManager.MakeAIMove();
//            }
//        }

//        if (PuzzleManager.GameStatus == GameStatus.WaitingToStart)
//        {
//            playerStarted.color = Color.cyan;
//        }
//        else
//        {
//            playerStarted.color = Color.blue;
//        }

//        if (Input.GetMouseButtonDown(0))
//        {
//            RaycastHit hit;
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

//            if (Physics.Raycast(ray, out hit))
//            {
//                if (hit.transform.CompareTag("reset"))
//                {
//                    PuzzleManager.ResetGame();
//                }
//                else if (PuzzleManager.GameStatus == GameStatus.WaitingToStart && hit.transform.CompareTag("start"))
//                {
//                    PuzzleManager.StartHumanGame();
//                }
//                else if (hit.transform.CompareTag("playerX"))
//                {
//                    if (PuzzleManager.GameStatus == GameStatus.WaitingToStart)
//                    {
//                        if (PuzzleManager.Player1.BehaviourParameters.BehaviorType == BehaviorType.HeuristicOnly)
//                        {
//                            if (PuzzleManager.Player1.BehaviourParameters.Model != null)
//                            {
//                                PuzzleManager.Player1.BehaviourParameters.BehaviorType = BehaviorType.InferenceOnly;
//                            }
//                        }
//                        else
//                        {
//                            PuzzleManager.Player1.BehaviourParameters.BehaviorType = BehaviorType.HeuristicOnly;
//                        }
//                        SetPlayerText();
//                    }
//                }
//                else if (hit.transform.CompareTag("playerO"))
//                {
//                    if (PuzzleManager.GameStatus == GameStatus.WaitingToStart)
//                    {
//                        if (PuzzleManager.Player2.BehaviourParameters.BehaviorType == BehaviorType.HeuristicOnly)
//                        {
//                            if (PuzzleManager.Player2.BehaviourParameters.Model != null)
//                            {
//                                PuzzleManager.Player2.BehaviourParameters.BehaviorType = BehaviorType.InferenceOnly;
//                            }
//                        }
//                        else
//                        {
//                            PuzzleManager.Player2.BehaviourParameters.BehaviorType = BehaviorType.HeuristicOnly;
//                        }
//                        SetPlayerText();
//                    }
//                }
//                else if (PuzzleManager.GameStatus == GameStatus.WaitingOnHuman)
//                {
//                    int piece;
//                    bool foundPiece = int.TryParse(hit.transform.name, out piece);

//                    if (foundPiece)
//                    {
//                        PuzzleManager.MakeHeuristicMove(piece);
//                    }
//                }
//            }
//        }
//    }
//}