using UnityEngine;
using UnityEngine.InputSystem;

namespace MausTemple
{
    public class Jump : MonoBehaviour
    {
        [SerializeField] private float _jumpForce;

        private Rigidbody2D _rb;
        private int _groundLayer;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _groundLayer = LayerMask.GetMask("Ground");
        }

        private bool IsGrounded()
        {
            var distanceToGround = 0.6f;
            var hit = Physics2D.Raycast(
                transform.position,
                Vector2.down,
                distanceToGround,
                _groundLayer
            );

            return hit.collider != null;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!IsGrounded()) return;
            if (!context.performed) return;

            _rb.velocity += Vector2.up * _jumpForce;
        }
    }
}
