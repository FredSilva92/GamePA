using UnityEngine;

//[CreateAssetMenu(fileName = "RewardsAI", menuName = "RewardsAI")]
//public class RewardsAI : ScriptableObject
public class RewardsAI : MonoBehaviour
{
    //public float Player1Win = 100.0f;
    //public float Player1Lost = -100.0f;
    //public float Player1Draw = 50.0f;
    //public float Player2Win = 100.0f;
    //public float Player2Lost = -100.0f;
    //public float Player2Draw = 50.0f;
    //public float CouldHaveWon = -0.5f;
    //public float CouldHaveBlocked = -0.5f;

    // agente 1 - escolhe primeiro n�mero
    public float FIRST_PIECE_IN_CORRECT_POSITION = 50f;
    public float FIRST_PIECE_IN_WRONG_POSITION = -50f;

    // agente 2 - escolhe segundo n�mero
    public float SECOND_PIECE_IN_CORRECT_POSITION = 50f;
    public float SECOND_PIECE_IN_WRONG_POSITION = -50f;
    public float DIFFERENT_PIECCE = 25f;
    public float SAME_PIECE = -25f;

    // no fim de resolver o puzzle
    public float HAS_PUZZLE_SOLVED = 100f;
    public float NOT_HAS_PUZZLE_SOLVED = -100f;
}