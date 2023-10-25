using UnityEngine;
using UnityEngine.Video;

/*
 * Para armazenar as informa��es dos estados do jogo.
*/
[System.Serializable]
public class GameStateInfo
{
    public GameState gameState;     // o identificador do estado do jogo (deve ser �nico)
    public bool hasCutscene;        // se o estado do jogo tem cutscene
    public VideoPlayer cutscene;    // a refer�ncia do objeto na cena para o v�deo da cutscene
    public bool hasNewPosition;     // se o jogador precisa de estar numa nova posi��o no mapa
    public Vector3 position;        // a posi��o inicial do jogador nesse estado
    public Vector3 rotation;        // a rota��o inicial do jogador nesse estado
}