using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPull : MonoBehaviour
{
    [Header("Pull Settings")]
    [SerializeField] private float pullDistance = 1.5f;
    [SerializeField] private KeyCode pullKey = KeyCode.E;
    [SerializeField] private LayerMask pushableLayer;
    [SerializeField] private Transform grabPoint;
    [SerializeField] private float grabRadius = 0.3f;

    private FixedJoint2D fixedJoint;
    private Rigidbody2D connectedBox;

    private void Awake()
    {
        fixedJoint = GetComponent<FixedJoint2D>();

        if (fixedJoint != null)
            fixedJoint.enabled = false;
    }

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (connectedBox == null)
                StartPull();
            else
                StopPull();
        }
    }

    private void StartPull()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            grabPoint.position,
            grabRadius,
            pushableLayer
        );

        if (hit == null)
            return;

        Rigidbody2D boxRb = hit.attachedRigidbody;

        if (boxRb == null)
            return;

        connectedBox = boxRb;

        fixedJoint.connectedBody = connectedBox;
        fixedJoint.enabled = true;
    }

    private void StopPull()
    {
        fixedJoint.enabled = false;
        fixedJoint.connectedBody = null;
        connectedBox = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pullDistance);
    }
}
