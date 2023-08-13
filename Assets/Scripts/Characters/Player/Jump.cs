using UnityEngine;
using UnityEngine.InputSystem;

namespace MausTemple
{
    public class Jump : MonoBehaviour
    {
        [SerializeField] private PlayerData _data;

        private Rigidbody2D _rb;
        private int _groundLayer;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = _data.gravityScale;
            _groundLayer = LayerMask.GetMask("Ground");
        }

        private void Update()
        {
            if (_rb.velocity.y < 0)
            {
                _rb.gravityScale = _data.gravityScale * _data.fallGravityMult;
                _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -_data.maxFallSpeed));
            }
            else
            {
                _rb.gravityScale = _data.gravityScale;
            }
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
            if (context.started)
            {
                var force = _data.jumpForce;
                if (_rb.velocity.y < 0)
                {
                    force -= _rb.velocity.y;
                }

                _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            }
        }
    }
}
