using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Configuración de movimiento")]
    [Range(0.01f, 1f)] public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 1, -3);

    [Header("Límites manuales (usa si no hay Tilemap)")]
    public bool useBounds = true;
    public float minX = 0f;
    public float maxX = 20f;
    public float minY = 0f;
    public float maxY = 10f;

    [Header("Configuración automática de límites (opcional)")]
    public bool autoBounds = false;
    public Tilemap tilemap;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        // Si está activada la detección automática y hay Tilemap
        if (autoBounds && tilemap != null)
        {
            SetBoundsFromTilemap();
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No se asignó un Target a la cámara.");
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        if (useBounds)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);
        }

        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, offset.z);
    }

    // Calcula automáticamente los límites según el tamaño del Tilemap
    void SetBoundsFromTilemap()
    {
        var bounds = tilemap.localBounds;
        minX = bounds.min.x;
        maxX = bounds.max.x;
        minY = bounds.min.y;
        maxY = bounds.max.y;

        Debug.Log($"Límites de cámara ajustados automáticamente: " +
                  $"X({minX}, {maxX}) | Y({minY}, {maxY})");
    }
}
