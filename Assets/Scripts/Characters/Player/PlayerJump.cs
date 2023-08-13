using UnityEngine;
using UnityEngine.InputSystem;

namespace MausTemple
{
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField] private PlayerData _data;

        private Rigidbody2D _rb;

        [SerializeField] private Transform _groundCheckPoint;
        [SerializeField] private Vector2 _groundCheckSize;
        [SerializeField] private LayerMask _groundLayer;

        private float _lastOnGroundTime;
        private float _lastPressedJumpTime;

        private bool _isJumping;
        private bool _isJumpCut;
        private bool _isJumpFalling;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = _data.gravityScale;
        }

        private void Update()
        {
            #region Collision Checks
            if (!_isJumping && Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
            {
                _lastOnGroundTime = _data.coyoteTime;
            }
            #endregion

            #region Jump Checks
            if (_isJumping && _rb.velocity.y < 0)
            {
                _isJumping = false;
                _isJumpFalling = true;
            }

            if (_lastOnGroundTime > 0 && !_isJumping)
            {
                _isJumpCut = false;
                _isJumpFalling = false;
            }

            if (CanJump() && _lastPressedJumpTime > 0)
            {
                _isJumping = true;
                _isJumpCut = false;
                _isJumpFalling = false;
                Jump();
            }
            #endregion

            #region Gravity
            if (_isJumpCut)
            {
                _rb.gravityScale = _data.gravityScale * _data.jumpCutGravityMult;
                _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -_data.maxFastFallSpeed));
            }
            else if (_isJumping || _isJumpFalling && Mathf.Abs(_rb.velocity.y) < _data.jumpHangTimeThreshold)
            {
                _rb.gravityScale = _data.gravityScale * _data.jumpHangGravityMult;
            }
            else if (_rb.velocity.y < 0)
            {
                _rb.gravityScale = _data.gravityScale * _data.fallGravityMult;
                _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -_data.maxFallSpeed));
            }
            else
            {
                _rb.gravityScale = _data.gravityScale;
            }
            #endregion
        }

        private void Jump()
        {
            _lastPressedJumpTime = 0;
            _lastOnGroundTime = 0;

            var force = _data.jumpForce;
            if (_rb.velocity.y < 0)
            {
                force -= _rb.velocity.y;
            }

            _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        }

        private bool CanJump() => _lastOnGroundTime > 0 && !_isJumping;

        private bool CanJumpCut() => _isJumping && _rb.velocity.y > 0;

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _lastPressedJumpTime = _data.jumpInputBufferTime;
            }
            if (context.canceled && CanJumpCut())
            {
                _isJumpCut = true;
            }
        }
    }
}
