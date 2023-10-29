using UnityEngine;

public class BeachAndForestManager : MonoBehaviour
{
    [SerializeField] private GameObject _goToCampCollider;

    /*
     * Disativa o colisor que existe antes de entrar no acampamento dos inimigos,
     * para que o colisor só esteja ativo antes da cutscene do acampamento iniciar.
    */
    public void DisableCampCollider()
    {
        _goToCampCollider.SetActive(false);
    }
}