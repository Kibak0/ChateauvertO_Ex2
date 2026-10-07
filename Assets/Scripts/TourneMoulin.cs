using UnityEngine;

public class TourneMoulin : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float vitesseRotation = 100f;

    private void Update()
    {
        Tourner();
    }

    // =========================================================
    // ROTATION
    // =========================================================

    private void Tourner()
    {
        transform.Rotate(
            Vector3.forward,
            vitesseRotation * Time.deltaTime
        );
    }
}