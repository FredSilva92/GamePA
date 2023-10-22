using UnityEngine.Video;

/*
 * Para armazenar as informa��es dos estados do jogo.
*/
[System.Serializable]
public class GameStateInfo
{
    public GameState gameState;
    public bool hasCutscene;
    public VideoPlayer cutscene;
}