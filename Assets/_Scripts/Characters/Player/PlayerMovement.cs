// Inspired by https://github.com/DawnosaurDev/platformer-movement/blob/e640b3498621cfe2f5e4a28e7c31f4d91610b6c7/Platformer%20Demo%20-%20Unity%20Project/Assets/Scripts/PlayerMovement.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private PlayerData _data;

    #region Components
    private Rigidbody2D _rb;
    [SerializeField] private AudioSource _speedAudioSource;
    [SerializeField] private AudioSource _movementAudioSource;
    #endregion

    #region State Parameters
    // Variables that control the various actions player can perform at any time
    private bool _isJumping;
    private bool _isWallJumping;

    // Timers
    private float _lastOnGroundTime;
    private float _lastOnWallTime;
    private float _lastOnWallRightTime;
    private float _lastOnWallLeftTime;

    // Jump
    private bool _isJumpCut;
    private bool _isJumpFalling;
    private bool _isFastFalling;

    // Wall Jump
    private float _wallJumpStartTime;
    private int _lastWallJumpDirection;
    #endregion

    #region Input Parameters
    private Vector2 _moveInput;

    private float _lastPressedJumpTime;
    #endregion

    #region Check Parameters
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize;
    [Space(5)]
    [SerializeField] private Transform _rightWallCheckPoint;
    [SerializeField] private Transform _leftWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize;
    #endregion

    #region Layers & Tags
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    #endregion

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        _rb.gravityScale = _data.gravityScale;
    }

    private void Update() {
        #region Timers
        _lastOnGroundTime -= Time.deltaTime;
        _lastOnWallTime -= Time.deltaTime;
        _lastOnWallRightTime -= Time.deltaTime;
        _lastOnWallLeftTime -= Time.deltaTime;

        _lastPressedJumpTime -= Time.deltaTime;
        #endregion

        #region Collision Checks
        if (!_isJumping) {
            // Ground Check
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
                _lastOnGroundTime = _data.coyoteTime;

            // Right Wall Check
            if (Physics2D.OverlapBox(_rightWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !_isWallJumping)
                _lastOnWallRightTime = _data.coyoteTime;

            // Left Wall Check
            if (Physics2D.OverlapBox(_leftWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !_isWallJumping)
                _lastOnWallLeftTime = _data.coyoteTime;

            _lastOnWallTime = Mathf.Max(_lastOnWallLeftTime, _lastOnWallRightTime);
        }
        #endregion

        #region Jump Checks
        if (_isJumping && _rb.velocity.y < 0) {
            _isJumping = false;
            _isJumpFalling = true;
        }

        if (_isWallJumping && Time.time - _wallJumpStartTime > _data.wallJumpTime)
            _isWallJumping = false;

        if (_lastOnGroundTime > 0 && !_isJumping && !_isWallJumping) {
            _isJumpCut = false;
            _isJumpFalling = false;
        }

        // Jump
        if (CanJump() && _lastPressedJumpTime > 0) {
            _isJumping = true;
            _isWallJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;
            Jump();
        }

        // Wall Jump
        else if (CanWallJump() && _lastPressedJumpTime > 0) {
            _isWallJumping = true;
            _isJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;

            _wallJumpStartTime = Time.time;
            _lastWallJumpDirection = (_lastOnWallRightTime > 0) ? -1 : 1;

            WallJump(_lastWallJumpDirection);
        }
        #endregion

        #region Gravity
        if (_rb.velocity.y < 0 && _isFastFalling) {
            _rb.gravityScale = _data.gravityScale * _data.fastFallGravityMult;
            _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -_data.maxFastFallSpeed));
        }
        else if (_isJumpCut) {
            _rb.gravityScale = _data.gravityScale * _data.jumpCutGravityMult;
            _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -_data.maxFallSpeed));
        }
        else if ((_isJumping || _isWallJumping || _isJumpFalling) && Mathf.Abs(_rb.velocity.y) < _data.jumpHangTimeThreshold) {
            _rb.gravityScale = _data.gravityScale * _data.jumpHangGravityMult;
        }
        else if (_rb.velocity.y < 0) {
            _rb.gravityScale = _data.gravityScale * _data.fallGravityMult;
            _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -_data.maxFallSpeed));
        }
        else {
            _rb.gravityScale = _data.gravityScale;
        }
        #endregion

        #region Audio
        var maxSpeedVolume = 0.05f;
        _speedAudioSource.volume = maxSpeedVolume * (_rb.velocity.sqrMagnitude / (_data.maxFastFallSpeed * _data.maxFallSpeed));
        #endregion
    }

    private void FixedUpdate() {
        if (_isWallJumping) {
            Run(_data.wallJumpRunLerp);
        }
        else {
            Run(1);
        }
    }

    #region Movement Methods
    private void Run(float lerpAmount) {
        // Calculate the direction we want to move in and our desired velocity
        float targetSpeed = _moveInput.x * _data.runMaxSpeed;
        // We can reduce air control using Lerp(), this smoothes changes to air direction and speed
        targetSpeed = Mathf.Lerp(_rb.velocity.x, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;

        // Gets an acceleration value based on if we are accelerating
        // or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (_lastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _data.runAccelAmount : _data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _data.runAccelAmount * _data.accelInAir : _data.runDeccelAmount * _data.deccelInAir;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        // Increase air acceleration and maxSpeed when at the apex of the jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((_isJumping || _isWallJumping || _isJumpFalling) && Mathf.Abs(_rb.velocity.y) < _data.jumpHangTimeThreshold) {
            accelRate *= _data.jumpHangAccelerationMult;
            targetSpeed *= _data.jumpHangMaxSpeedMult;
        }
        #endregion

        #region Conserve Momentum
        // We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (_data.doConserveMomentum && Mathf.Abs(_rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(_rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && _lastOnGroundTime < 0) {
            // Prevent any deceleration from happening, or in other words conserve current momentum
            accelRate = 0;
        }
        #endregion

        // Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - _rb.velocity.x;
        // Calculate force along x-axis to apply to the player
        float movement = speedDif * accelRate;

        // Convert this to a vector and apply to rigidbody
        _rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void Jump() {
        _lastPressedJumpTime = 0;
        _lastOnGroundTime = 0;

        var force = _data.jumpForce;
        if (_rb.velocity.y < 0) {
            force -= _rb.velocity.y;
        }

        _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        _movementAudioSource.PlayOneShot(_data.jumpSound);
    }

    private void WallJump(int direction) {
        // Ensures we can't call Wall Jump multiple times from one press
        _lastPressedJumpTime = 0;
        _lastOnGroundTime = 0;
        _lastOnWallRightTime = 0;
        _lastOnWallLeftTime = 0;

        // Perform wall jump
        var force = new Vector2(_data.wallJumpForce.x, _data.wallJumpForce.y);
        force.x *= direction; // Apply force in opposite direction of wall

        if (Mathf.Sign(_rb.velocity.x) != Mathf.Sign(force.x)) {
            force.x -= _rb.velocity.x;
        }

        // Checks whether player is falling, if so we counteract the force of gravity. This ensures that player always reaches desired jump force or greater.
        if (_rb.velocity.y < 0) {
            force.y -= _rb.velocity.y;
        }

        _rb.AddForce(force, ForceMode2D.Impulse);

        _movementAudioSource.PlayOneShot(_data.wallJumpSound);
    }
    #endregion

    #region Check Methods
    private bool CanJump() => _lastOnGroundTime > 0 && !_isJumping;

    private bool CanWallJump() => _lastPressedJumpTime > 0 && _lastOnWallTime > 0 && _lastOnGroundTime <= 0 && (!_isWallJumping ||
         (_lastOnWallRightTime > 0 && _lastWallJumpDirection == 1) || (_lastOnWallLeftTime > 0 && _lastWallJumpDirection == -1));

    private bool CanJumpCut() => _isJumping && _rb.velocity.y > 0;

    private bool CanWallJumpCut() => _isWallJumping && _rb.velocity.y > 0;
    #endregion

    #region Event Handlers
    public void OnMove(InputAction.CallbackContext context) {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (context.started) {
            _lastPressedJumpTime = _data.jumpInputBufferTime;
        }
        if (context.canceled && (CanJumpCut() || CanWallJumpCut())) {
            _isJumpCut = true;
        }
    }

    public void OnDrop(InputAction.CallbackContext context) {
        if (context.performed) return;
        _isFastFalling = context.started;
    }
    #endregion

    #region EDITOR METHODS
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_rightWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_leftWallCheckPoint.position, _wallCheckSize);
    }
    #endregion
}
