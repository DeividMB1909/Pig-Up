using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // El personaje a seguir
    public float smoothSpeed = 0.125f; // Velocidad de seguimiento suave
    public Vector3 offset = new Vector3(0, 0, -10); // Desplazamiento de la cámara

    void LateUpdate()
    {
        // Verificar que el target esté asignado
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No hay target asignado!");
            return;
        }

        // Calcular posición deseada
        Vector3 desiredPosition = target.position + offset;

        // Suavizar el movimiento
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Aplicar la posición (mantener Z en -10 siempre)
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, -10);
    }
}