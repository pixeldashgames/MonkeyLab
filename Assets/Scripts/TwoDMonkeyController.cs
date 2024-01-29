using UnityEngine;
using UnityEngine.InputSystem;

public class TwoDMonkeyController : MonoBehaviour
{
    [SerializeField] private float moveVelocity;
    [SerializeField] private float jumpForce;
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform monkeyImage;

    private Rigidbody2D _body;
    private StarterAssetsInput _input;
    private bool _isGrounded;

    private void Awake()
    {
        _input = new StarterAssetsInput();
        _input.Enable();
        _body = GetComponent<Rigidbody2D>();
        _input.Player.Jump.performed += JumpOnPerformed;
    }

    private void FixedUpdate()
    {
        CheckGround();

        var horizontalMovement = _input.Player.Move.ReadValue<Vector2>().x;

        if (!(Mathf.Abs(horizontalMovement) > 0.1f)) return;

        _body.velocityX = moveVelocity * horizontalMovement;

        var monkeyScale = monkeyImage.localScale;

        monkeyScale.x = Mathf.Abs(monkeyScale.x) * (horizontalMovement > 0 ? 1 : -1);

        monkeyImage.localScale = monkeyScale;
    }

    private void JumpOnPerformed(InputAction.CallbackContext obj)
    {
        if (_isGrounded)
            _body.AddForce(Vector2.up * jumpForce);
    }

    private void CheckGround()
    {
        _isGrounded = Physics2D.Raycast(transform.position,
            Vector2.down,
            playerHeight / 2 * 1.05f, groundLayer);
    }
}