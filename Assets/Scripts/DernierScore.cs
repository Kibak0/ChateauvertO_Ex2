using TMPro;
using UnityEngine;

public class DernierScore : MonoBehaviour
{
    private const string CleScore = "dernierScore";

    [Header("UI")]
    [SerializeField] private TMP_Text dernierScoreText;

    private void Start()
    {
        AfficherDernierScore();
    }

    // =========================================================
    // SCORE
    // =========================================================

    private void AfficherDernierScore()
    {
        if (dernierScoreText == null)
        {
            Debug.LogError(
                $"{nameof(DernierScore)} : aucun texte de score n'est assigné.",
                this
            );

            return;
        }

        int dernierScore = PlayerPrefs.GetInt(CleScore, 0);

        dernierScoreText.text = $"Dernier score : {dernierScore}";
    }
}