/*
 * Para definir os diferentes tipos de estado do jogo.
*/
public enum GameState
{
    INTRO_TO_GAME = 1,      // quando passa do menu para a cutscene inicial
    HIDE_SHIP = 2,          // quando est� � procura da nave
    GO_TO_FOREST = 3        // quando est� � procura do caminho para a floresta
}