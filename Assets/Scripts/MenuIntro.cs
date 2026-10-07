using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuIntro : MonoBehaviour
{
    [Header("Scène")]
    [SerializeField] private string nomSceneJeu;

    [Header("UI")]
    [SerializeField] private Button boutonDemarrer;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioMusique;

    private void Start()
    {
        InitialiserCurseur();
        InitialiserBouton();
        JouerMusique();
    }

    // =========================================================
    // INITIALISATION
    // =========================================================

    private void InitialiserCurseur()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void InitialiserBouton()
    {
        if (boutonDemarrer == null)
        {
            Debug.LogError(
                $"{nameof(MenuIntro)} : aucun bouton de démarrage n'est assigné.",
                this
            );

            return;
        }

        boutonDemarrer.onClick.AddListener(Demarrer);
    }

    // =========================================================
    // AUDIO
    // =========================================================

    private void JouerMusique()
    {
        if (audioSource == null || audioMusique == null)
        {
            return;
        }

        audioSource.clip = audioMusique;
        audioSource.loop = true;
        audioSource.Play();
    }

    // =========================================================
    // BOUTON DÉMARRER
    // =========================================================

    public void Demarrer()
    {
        if (string.IsNullOrWhiteSpace(nomSceneJeu))
        {
            Debug.LogError(
                $"{nameof(MenuIntro)} : le nom de la scène de jeu n'est pas défini.",
                this
            );

            return;
        }

        SceneManager.LoadScene(nomSceneJeu);
    }

    // =========================================================
    // NETTOYAGE
    // =========================================================

    private void OnDestroy()
    {
        if (boutonDemarrer != null)
        {
            boutonDemarrer.onClick.RemoveListener(Demarrer);
        }
    }
}