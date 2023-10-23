/*
 * Para armazenar as diversas ações do mapa e o seu estado de jogo correspondente.
*/
using UnityEngine;

[System.Serializable]
public class MapAction
{
    public int id;
    public GameStateInfo gameStateInfo;
    public string title;
    public int priority;
    public bool hasProgress;
    public bool hasClick;
    public GameObject button;
    public bool hasDialogue;
    public bool isSingle;
}