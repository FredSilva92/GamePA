using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private List<PuzzlePiece> _puzzlePieces;
    private PuzzlePiece _firstPiece;
    private PuzzlePiece _secondPiece;


    private void Start()
    {
        InitializePuzzle();
    }

    private void InitializePuzzle()
    {
        PuzzlePiece originalPiece1 = _puzzlePieces[0];
        PuzzlePiece originalPiece2 = _puzzlePieces[1];
        PuzzlePiece originalPiece3 = _puzzlePieces[2];
        PuzzlePiece originalPiece4 = _puzzlePieces[3];
        PuzzlePiece originalPiece5 = _puzzlePieces[4];
        PuzzlePiece originalPiece6 = _puzzlePieces[5];
        PuzzlePiece originalPiece7 = _puzzlePieces[6];
        PuzzlePiece originalPiece8 = _puzzlePieces[7];
        PuzzlePiece originalPiece9 = _puzzlePieces[8];

        Vector3 originalPositionPoint1 = _puzzlePieces[0].point.transform.localPosition;
        Vector3 originalPositionPoint2 = _puzzlePieces[1].point.transform.localPosition;
        Vector3 originalPositionPoint3 = _puzzlePieces[2].point.transform.localPosition;
        Vector3 originalPositionPoint4 = _puzzlePieces[3].point.transform.localPosition;
        Vector3 originalPositionPoint5 = _puzzlePieces[4].point.transform.localPosition;
        Vector3 originalPositionPoint6 = _puzzlePieces[5].point.transform.localPosition;
        Vector3 originalPositionPoint7 = _puzzlePieces[6].point.transform.localPosition;
        Vector3 originalPositionPoint8 = _puzzlePieces[7].point.transform.localPosition;
        Vector3 originalPositionPoint9 = _puzzlePieces[8].point.transform.localPosition;

        _puzzlePieces[0].piece.transform.localPosition = originalPositionPoint9;
        _puzzlePieces[1].piece.transform.localPosition = originalPositionPoint5;
        _puzzlePieces[2].piece.transform.localPosition = originalPositionPoint8;
        _puzzlePieces[3].piece.transform.localPosition = originalPositionPoint6;
        _puzzlePieces[4].piece.transform.localPosition = originalPositionPoint7;
        _puzzlePieces[5].piece.transform.localPosition = originalPositionPoint1;
        _puzzlePieces[6].piece.transform.localPosition = originalPositionPoint4;
        _puzzlePieces[7].piece.transform.localPosition = originalPositionPoint3;
        _puzzlePieces[8].piece.transform.localPosition = originalPositionPoint2;

        _puzzlePieces[0] = originalPiece9;
        _puzzlePieces[1] = originalPiece5;
        _puzzlePieces[2] = originalPiece8;
        _puzzlePieces[3] = originalPiece6;
        _puzzlePieces[4] = originalPiece7;
        _puzzlePieces[5] = originalPiece1;
        _puzzlePieces[6] = originalPiece4;
        _puzzlePieces[7] = originalPiece3;
        _puzzlePieces[8] = originalPiece2;
    }

    /*
     * Se a tecla pressionada for um número e converte o valor se a tecla for um número entre 0 a 9 e -1 se não for.
    */
    public void DoPlay()
    {
        // se uma tecla for pressionada
        if (Input.anyKeyDown)
        {
            // obtém o número da tecla
            int inputtedNumber = GetNumericKeyValue();

            if (CheckValidPlay(inputtedNumber))
            {
                // se AINDA NÃO escolheu a primeira peça, escolhe essa
                // se JÁ escolheu a primeira, agora escolhe a segunda
                if (_firstPiece == null)
                {
                    _firstPiece = ChoosePiece(inputtedNumber);
                    return;
                }
                else
                {
                    _secondPiece = ChoosePiece(inputtedNumber);
                    MovePieces();
                }
            }
        }
    }

    /*
     * Recebe a tecla pressionada e converte o valor se a tecla for um número entre 0 a 9 e -1 se não for.
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

    private void MovePieces()
    {
        // se ambas as peças existem, troca as peças
        if (_firstPiece != null && _secondPiece != null)
        {
            int firstIndex = _puzzlePieces.IndexOf(_firstPiece);
            int secondIndex = _puzzlePieces.IndexOf(_secondPiece);

            // troca as posições dos objetos na cena
            Vector3 tempPosition = _puzzlePieces[firstIndex].piece.transform.localPosition;
            //_puzzlePieces[firstIndex].piece.transform.position = _puzzlePieces[secondIndex].piece.transform.position;
            //_puzzlePieces[secondIndex].piece.transform.position = tempPosition;

            //int tempPositionValue = _puzzlePieces[firstIndex].position;
            //_puzzlePieces[firstIndex].position = _puzzlePieces[secondIndex].position;
            //_puzzlePieces[secondIndex].position = tempPositionValue;

            _puzzlePieces[firstIndex].piece.transform.localPosition = _puzzlePieces[secondIndex].piece.transform.localPosition;
            _puzzlePieces[secondIndex].piece.transform.localPosition = tempPosition;

            _puzzlePieces[firstIndex] = _firstPiece;
            _puzzlePieces[secondIndex] = _secondPiece;

            _firstPiece = null;
            _secondPiece = null;
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