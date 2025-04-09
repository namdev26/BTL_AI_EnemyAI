using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private Vector2 lastDirection = Vector2.down;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        animator.SetFloat("InputX", 0);
        animator.SetFloat("InputY", -1);
        animator.SetFloat("LastInputX", 0);
        animator.SetFloat("LastInputY", -1);
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput * speed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);
            if (Mathf.Abs(moveInput.x) > 0.5f || Mathf.Abs(moveInput.y) > 0.5f)
            {
                lastDirection = moveInput.normalized;
                animator.SetFloat("LastInputX", lastDirection.x);
                animator.SetFloat("LastInputY", lastDirection.y);
            }
        }
        animator.SetBool("isWalking", isMoving);
        if (context.canceled)
        {
            animator.SetFloat("InputX", lastDirection.x);
            animator.SetFloat("InputY", lastDirection.y);
        }
    }
}