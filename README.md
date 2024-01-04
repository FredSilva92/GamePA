# JOGO - Cosmic Confrontation: The Island's Legacy

É um jogo de aventura 3D que passa-se em uma ilha perdida num planeta distante, onde o jogador iniciará o jogo com Halley Bennet, personagem principal que parte em uma aventura para encontrar um tesouro escondido antes que os piratas do universo cheguem primeiro, dessa forma para devolver a vida à normalidade do cosmo pelos que foi conquistada pelos piratas.


---


## 1º Técnica de IA - Path Finding para movimento dos inimigos e aranhas

[INFORMAÇÕES]


---


## 2º Técnica de IA - Máquina de Estados para tomada de decisão dos inimigos

Para este tópico, foi criada uma máquina de estados, que faz a transição entre 5 estados:
Idle, patrulhar, atacar parado, atacar a caminhar e morrer. Para cada um destes estados foram criadas uma classe para cada estado.
Estas classes foram criadas a partir da baseState, que é a classe da qual herdam os métodos principais da máquina de estados: Enter, Update e Exit.
Também foi criado uma classe cahamda stateManager, que vai gerir a transição entre estados. Esta classe é a super classe da EnemyStateMachine, na qual vai injectar as features do enemy nos diferentes estados.
A execução deste código acontece na classe EnemyScript.

Além disso, também é executado um script chamado EnemyGroupScript que permite mudar estado para diferentes enemies ao mesmo tempo.

O código encontra-se no seguinte branch: 
https://github.com/FredSilva92/GamePA/tree/AIStateMachine


---


## 3º Técnica de IA - Aprendizado por Reforço para resolver um puzzle

[INFORMAÇÕES]
