/*
 * Para armazenar e determinar as diversas ações e estado de progresso do jogador no jogo.
*/
public class MapAction
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Priority { get; set; }
    public bool HasProgress { get; set; }
    public bool HasCutscene { get; set; }
    public bool HasClick { get; set; }
    public bool HasDialogue { get; set; }

    public MapAction(int id, string title, int priority, bool hasProgress, bool hasCutscene, bool hasClick, bool hasDialogue)
    {
        Id = id;
        Title = title;
        Priority = priority;
        HasProgress = hasProgress;
        HasCutscene = hasCutscene;
        HasClick = hasClick;
        HasDialogue = hasDialogue;
    }
}