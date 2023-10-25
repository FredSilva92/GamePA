using UnityEngine;
using UnityEngine.Video;

/*
 * Para armazenar as informações dos estados do jogo.
*/
[System.Serializable]
public class GameStateInfo
{
    public GameState gameState;     // o identificador do estado do jogo (deve ser único)
    public bool hasCutscene;        // se o estado do jogo tem cutscene
    public VideoPlayer cutscene;    // a referência do objeto na cena para o vídeo da cutscene
    public Vector3 playerPosition;  // a posição inicial do jogador nesse estado
    public Vector3 playerRotation;  // a rotação inicial do jogador nesse estado
}