using UnityEngine;

public class GestionnaireJeu : MonoBehaviour
{
    public static GestionnaireJeu Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    // =========================================================
    // GESTION DU JEU
    // =========================================================

    public void TerminerJeu()
    {

    }
}