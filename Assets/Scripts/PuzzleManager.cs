using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private List<PuzzlePiece> _puzzlePieces;
    private PuzzlePiece _firstPiece;
    private PuzzlePiece _secondPiece;


    private void InitializePuzzle()
    {
        foreach (PuzzlePiece puzzlePiece in _puzzlePieces)
        {

        }
    }

    /*
     * Se a tecla pressionada for um n�mero e converte o valor se a tecla for um n�mero entre 0 a 9 e -1 se n�o for.
    */
    public void DoPlay()
    {
        // se uma tecla for pressionada
        if (Input.anyKeyDown)
        {
            // obt�m o n�mero da tecla
            int inputtedNumber = GetNumericKeyValue();

            if (CheckValidPlay(inputtedNumber))
            {
                Debug.Log("Foi pressionada a tecla num�rica: " + inputtedNumber);

                // se AINDA N�O escolheu a primeira pe�a, escolhe essa
                // se J� escolheu a primeira, agora escolhe a segunda
                if (_firstPiece == null)
                {
                    _firstPiece = ChoosePiece(inputtedNumber);
                    return;
                }
                else
                {
                    _secondPiece = ChoosePiece(inputtedNumber);
                    MovePieces(_firstPiece, _secondPiece);
                }
            }
        }
    }

    /*
     * Recebe a tecla pressionada e converte o valor se a tecla for um n�mero entre 0 a 9 e -1 se n�o for.
    */
    private int GetNumericKeyValue()
    {
        string key = Input.inputString;
        int result;

        if (int.TryParse(key, out result))
        {
            return result;
        }
        else
        {
            return -1;
        }
    }

    private bool CheckValidPlay(int inputtedNumber)
    {
        if (inputtedNumber >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private PuzzlePiece ChoosePiece(int inputtedNumber)
    {
        foreach (PuzzlePiece puzzlePiece in _puzzlePieces)
        {
            if (puzzlePiece.position == inputtedNumber)
            {
                return puzzlePiece;
            }
        }

        return null;
    }

    private void MovePieces(PuzzlePiece firstPiece, PuzzlePiece secondPiece)
    {
        // se ambas as pe�as existem, troca as pe�as
        if (firstPiece != null && secondPiece != null)
        {
            int firstIndex = _puzzlePieces.IndexOf(firstPiece);
            int secondIndex = _puzzlePieces.IndexOf(secondPiece);

            // troca as posi��es dos objetos na cena
            Vector3 tempPosition = _puzzlePieces[firstIndex].piece.transform.position;
            //_puzzlePieces[firstIndex].piece.transform.position = _puzzlePieces[secondIndex].piece.transform.position;
            //_puzzlePieces[secondIndex].piece.transform.position = tempPosition;

            //int tempPositionValue = _puzzlePieces[firstIndex].position;
            //_puzzlePieces[firstIndex].position = _puzzlePieces[secondIndex].position;
            //_puzzlePieces[secondIndex].position = tempPositionValue;

            _puzzlePieces[firstIndex] = firstPiece;
            _puzzlePieces[secondIndex] = secondPiece;

            _puzzlePieces[firstIndex].piece.transform.position = _puzzlePieces[secondIndex].piece.transform.position;
            _puzzlePieces[secondIndex].piece.transform.position = tempPosition;

            firstPiece = null;
            secondPiece = null;
        }
    }

    public bool CheckPuzzleSolved()
    {
        bool isSolved = true;

        for (int i = 0; i < _puzzlePieces.Count; i++)
        {
            if (_puzzlePieces[i].position != i + 1)
            {
                isSolved = false;
                break;
            }
        }

        return isSolved;
    }
}