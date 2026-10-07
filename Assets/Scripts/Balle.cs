using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Balle : MonoBehaviour
{
    [Header("État du jeu")]
    [SerializeField] private bool peutJouer = true;

    [Header("Paramètres du tir")]
    [SerializeField] private float angleTir = 0f;
    [SerializeField] private float vitesseRotation = 5f;
    [SerializeField] private float forceTir = 0f;
    [SerializeField] private float accumulateurForce = 1f;

    private int coups = 0;
    private Vector3 dernierePosition;

    [Header("UI")]
    [SerializeField] private Slider jaugeForce;
    [SerializeField] private TMP_Text coupsText;

    [Header("Input Actions")]
    [SerializeField] private InputAction tirAction;
    [SerializeField] private InputAction tournerAction;

    [Header("Composants")]
    [SerializeField] private Rigidbody rigidbodyBalle;
    [SerializeField] private LineRenderer lineRendererBalle;

    [Header("Scène")]
    [SerializeField] private string nomSceneMenu;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioTir;
    [SerializeField] private AudioClip audioTrou;
    [SerializeField] private AudioClip audioFoule;
    [SerializeField] private AudioClip audioMusique;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        dernierePosition = transform.position;

        MettreAJourUI();
        JouerMusique();
    }

    private void Update()
    {
        GererEtatBalle();

        if (!peutJouer)
        {
            return;
        }

        GererRotation();
        GererTir();
        MettreAJourLigne();
    }

    // =========================================================
    // GESTION DU JEU
    // =========================================================

    private void GererEtatBalle()
    {
        bool balleEnMouvement = rigidbodyBalle.linearVelocity.sqrMagnitude > 0.01f;

        if (balleEnMouvement)
        {
            peutJouer = false;
            lineRendererBalle.enabled = false;
        }
        else
        {
            peutJouer = true;
            lineRendererBalle.enabled = true;
        }
    }

    private void GererRotation()
    {
        float inputRotation = tournerAction.ReadValue<float>();

        angleTir += inputRotation * vitesseRotation * Time.deltaTime;
    }

    private void GererTir()
    {
        if (tirAction.WasPressedThisFrame())
        {
            CommencerChargement();
        }

        if (tirAction.IsPressed())
        {
            ChargerTir();
        }

        if (tirAction.WasReleasedThisFrame())
        {
            Tirer();
        }
    }

    private void CommencerChargement()
    {
        forceTir = 0f;
        dernierePosition = transform.position;

        MettreAJourUI();
    }

    private void ChargerTir()
    {
        forceTir += accumulateurForce * Time.deltaTime;

        forceTir = Mathf.Clamp(
            forceTir,
            jaugeForce.minValue,
            jaugeForce.maxValue
        );

        MettreAJourUI();
    }

    private void Tirer()
    {
        Vector3 direction = ObtenirDirectionTir();

        rigidbodyBalle.AddForce(
            direction * forceTir,
            ForceMode.Impulse
        );

        coups++;

        JouerSon(audioTir);

        forceTir = 0f;

        MettreAJourUI();
    }

    private Vector3 ObtenirDirectionTir()
    {
        return Quaternion.Euler(0f, angleTir, 0f) * Vector3.left;
    }

    private void MettreAJourLigne()
    {
        Vector3 direction = ObtenirDirectionTir();

        lineRendererBalle.SetPosition(0, transform.position);
        lineRendererBalle.SetPosition(1, transform.position + direction);
    }

    // =========================================================
    // COLLISIONS
    // =========================================================

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("horsParcours"))
        {
            return;
        }

        JouerSon(audioFoule);

        RevenirDernierePosition();
    }

    private void RevenirDernierePosition()
    {
        rigidbodyBalle.linearVelocity = Vector3.zero;
        rigidbodyBalle.angularVelocity = Vector3.zero;

        rigidbodyBalle.position = dernierePosition;
        transform.position = dernierePosition;
    }

    // =========================================================
    // TROU
    // =========================================================

    private void OnTriggerEnter(Collider collision)
    {
        if (!collision.gameObject.CompareTag("trou"))
        {
            return;
        }

        FinirPartie();
    }

    private void FinirPartie()
    {
        peutJouer = false;

        JouerSon(audioTrou);

        SauvegarderScore();

        StartCoroutine(ChargerMenuQuandAudioTermine());
    }

    // =========================================================
    // UI
    // =========================================================

    private void MettreAJourUI()
    {
        if (jaugeForce != null)
        {
            jaugeForce.value = forceTir;
        }

        if (coupsText != null)
        {
            coupsText.text = "Coup(s) : " + coups;
        }
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

    private void JouerSon(AudioClip clip)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip);
    }

    // =========================================================
    // SCORE
    // =========================================================

    private void SauvegarderScore()
    {
        PlayerPrefs.SetInt("dernierScore", coups);
        PlayerPrefs.Save();
    }

    // =========================================================
    // RETOUR AU MENU
    // =========================================================

    private IEnumerator ChargerMenuQuandAudioTermine()
    {
        // On attend au minimum une frame afin de laisser
        // le son du trou démarrer correctement.
        yield return null;

        if (audioSource != null && audioTrou != null)
        {
            yield return new WaitForSeconds(audioTrou.length);
        }

        SceneManager.LoadScene(nomSceneMenu);
    }

    // =========================================================
    // INPUT ACTIONS
    // =========================================================

    private void OnEnable()
    {
        tirAction.Enable();
        tournerAction.Enable();
    }

    private void OnDisable()
    {
        tirAction.Disable();
        tournerAction.Disable();
    }
}