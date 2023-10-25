/*
 * Para definir os diferentes tipos de estado do jogo.
*/
public enum GameState
{
    INTRO_GAME,     // quando passa do menu para a cutscene inicial
    HIDE_SHIP,      // quando est� � procura da nave
    GO_TO_FOREST,   // quando est� � procura do caminho para a floresta
    INTRO_FOREST,   // quando passa da praia para a floresta
    GO_TO_CAMP,     // quando segue o caminho da floresta � ao acampamento
    GO_TO_CAVE,     // quando segue o caminho depois do acampamento at� � entrada da caverna
    INTRO_CAVE,     // quando passa da floresta para a caverna
    GO_TO_MAZE,     // quando segue o caminho da caverna at� ao labirinto
}