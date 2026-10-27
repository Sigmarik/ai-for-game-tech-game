using System.Collections;
using UnityEngine;

/// <summary>
/// Decorative train that occasionally zooms past a fixed point on straight tracks.
/// Attach this to the train object and place it at the point the train should pass by,
/// with its forward (+Z) axis pointing along the tracks.
/// </summary>
public class Train : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("How fast the train travels. Keep this large for a 'zoom past' effect.")]
    [SerializeField] private float speed = 200f;

    [Tooltip("How far from the passing point the train spawns and despawns.")]
    [SerializeField] private float travelDistance = 250f;

    [Header("Timing")]
    [Tooltip("Min (x) and max (y) seconds to wait between passes.")]
    [SerializeField] private Vector2 waitTimeRange = new Vector2(6f, 20f);

    private Vector3 passingPoint;
    private Vector3 trackDirection;
    private Quaternion forwardRotation;
    private Renderer[] renderers;

    private void Awake()
    {
        // The train's initial position is the point it passes by.
        passingPoint = transform.position;

        // Tracks are straight, so the train's own forward axis defines the track direction.
        trackDirection = transform.forward.normalized;
        forwardRotation = transform.rotation;

        renderers = GetComponentsInChildren<Renderer>(true);
        SetVisible(false);
    }

    private void Start()
    {
        StartCoroutine(PassRoutine());
    }

    private IEnumerator PassRoutine()
    {
        while (true)
        {
            // Random interval between passes.
            yield return new WaitForSeconds(Random.Range(waitTimeRange.x, waitTimeRange.y));

            // Random direction along the tracks.
            bool reverse = Random.value < 0.5f;
            Vector3 travelDirection = reverse ? -trackDirection : trackDirection;

            // Face the direction of travel (flip the model when going the other way).
            transform.rotation = reverse
                ? forwardRotation * Quaternion.Euler(0f, 180f, 0f)
                : forwardRotation;

            // Spawn out of sight on one side, then drive straight through the passing point.
            transform.position = passingPoint - travelDirection * travelDistance;
            SetVisible(true);

            float travelled = 0f;
            float totalDistance = travelDistance * 2f;

            while (travelled < totalDistance)
            {
                float step = speed * Time.deltaTime;
                transform.position += travelDirection * step;
                travelled += step;
                yield return null;
            }

            // Hide until the next pass.
            SetVisible(false);
            transform.position = passingPoint;
        }
    }

    private void SetVisible(bool visible)
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].enabled = visible;
    }
}