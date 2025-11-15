using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveDistance = 1f;
    public float moveSpeed = 5f;
    
    private Vector3 targetPosition;
    private bool isMoving = false;
    
    // Referencias al nuevo Manager y feedback
    private RhythmManager rhythmManager; 
    private CameraShake cameraShake;
    private BPMErrorFeedback bpmErrorFeedback;

    void Start()
    {
        targetPosition = transform.position;
        // Encuentra las referencias una sola vez
        rhythmManager = FindObjectOfType<RhythmManager>(); 
        bpmErrorFeedback = FindObjectOfType<BPMErrorFeedback>();
        cameraShake = Camera.main.GetComponent<CameraShake>();

        if (rhythmManager == null)
        {
            Debug.LogError("PlayerMove: No se encontró el RhythmManager. ¡El juego no funcionará rítmicamente!");
        }
    }

    void Update()
    {
        if (isMoving)
            return;

        Vector3 direction = Vector3.zero;
        
        // 1. Detección de Input
        if (Input.GetKeyDown(KeyCode.W)) direction = Vector3.forward;
        else if (Input.GetKeyDown(KeyCode.S)) direction = Vector3.back;
        else if (Input.GetKeyDown(KeyCode.A)) direction = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.D)) direction = Vector3.right;

        if (direction != Vector3.zero)
        {
            // 2. Verificación del Beat con el Manager
            if (rhythmManager != null && rhythmManager.IsTimeToMove())
            {
                // Movimiento exitoso
                targetPosition += direction * moveDistance;
                if (cameraShake != null) cameraShake.Shake();
                StartCoroutine(MoveToPosition(targetPosition));
            }
            else
            {
                // Feedback de error por moverse fuera de ritmo
                if (bpmErrorFeedback != null) bpmErrorFeedback.TriggerFlash();
            }
        }
    }

    // Coroutine de movimiento (se mantiene igual, es eficiente)
    IEnumerator MoveToPosition(Vector3 destination)
    {
        isMoving = true;
        while (Vector3.Distance(transform.position, destination) > 0.01f)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = destination;
        isMoving = false;
    }
}