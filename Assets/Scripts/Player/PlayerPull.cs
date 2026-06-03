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

    // Variável que o script de movimento lê para saber se trava o pulo/giro
    [HideInInspector] public bool isHoldingBox = false;

    private FixedJoint2D fixedJoint;
    private Rigidbody2D connectedBox;
    
    // Variável para guardar o peso original da caixa
    private float massaOriginalDaCaixa;

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

        // Se estiver segurando a caixa e apertar R, inverte os lados
        if (isHoldingBox && Keyboard.current.rKey.wasPressedThisFrame)
        {
            VirarComObjeto();
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

        // SALVA o peso original da caixa e a deixa leve para o Player conseguir puxar
        massaOriginalDaCaixa = connectedBox.mass;
        connectedBox.mass = 1f;

        fixedJoint.connectedBody = connectedBox;
        fixedJoint.enabled = true;

        // AVISA QUE ESTÁ SEGURANDO A CAIXA
        isHoldingBox = true;
    }

    private void StopPull()
    {
        if (connectedBox != null)
        {
            // DEVOLVE o peso original para ela voltar a ser pesada e inamovível
            connectedBox.mass = massaOriginalDaCaixa;
        }

        fixedJoint.enabled = false;
        fixedJoint.connectedBody = null;
        connectedBox = null;

        // AVISA QUE SOLTOU A CAIXA
        isHoldingBox = false;
    }

    private void VirarComObjeto()
    {
        if (connectedBox == null) return;

        // 1. Desliga o Joint temporariamente para evitar bugs físicos
        fixedJoint.enabled = false;

        // 2. Vira o corpo do Player visual e fisicamente
        Vector3 playerScale = transform.localScale;
        playerScale.x *= -1;
        transform.localScale = playerScale;

        // 3. Calcula a distância atual da caixa e espelha para o outro lado perfeitamente
        Vector2 offsetDaCaixa = connectedBox.transform.position - transform.position;
        offsetDaCaixa.x *= -1; 
        
        // Teleporta a caixa para as costas/frente do player na nova direção
        connectedBox.transform.position = (Vector2)transform.position + offsetDaCaixa;

        // 4. Vira a Caixa visualmente (inverte a escala X dela também)
        Vector3 boxScale = connectedBox.transform.localScale;
        boxScale.x *= -1;
        connectedBox.transform.localScale = boxScale;

        // 5. Religa o Joint para grudar tudo de novo
        fixedJoint.connectedBody = connectedBox;
        fixedJoint.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        
        // Círculo amarelo vai aparecer exatamente no grabPoint com o raio correto
        if (grabPoint != null)
        {
            Gizmos.DrawWireSphere(grabPoint.position, grabRadius);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, pullDistance);
        }
    }
}