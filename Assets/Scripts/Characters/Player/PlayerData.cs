// Inspired by https://github.com/DawnosaurDev/platformer-movement/blob/e640b3498621cfe2f5e4a28e7c31f4d91610b6c7/Platformer%20Demo%20-%20Unity%20Project/Assets/Scripts/PlayerData.cs
using UnityEngine;

namespace MausTemple
{
    [CreateAssetMenu(menuName = "Player Data")]
    public class PlayerData : ScriptableObject
    {
        [Header("Gravity")]
        [HideInInspector] public float gravityStrength; // Downwards force needed for the desired jumpHeight and jumpTimeToApex
        [HideInInspector] public float gravityScale; // Strength of player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D)
                                                     // Also the value player's rigidbody2D.gravityScale is set to
        [Space(5)]
        public float fallGravityMult; // Multiplier to player's gravityScale when falling
        public float maxFallSpeed; // Maximum fall speed of player when falling
        [Space(5)]
        public float fastFallGravityMult; // Larger multiplier to player's gravityScale when they are falling and a downwards input is pressed
        public float maxFastFallSpeed; // Maximum fall speed of player when performing a faster fall

        [Space(20)]

        [Header("Run")]
        public float runMaxSpeed; // Target speed we want player to reach
        public float runAcceleration; // The speed at which player accelerates to max speed
        [HideInInspector] public float runAccelAmount; // The actual force (multiplied with speedDiff) applied to player
        public float runDecceleration; // The speed at which player decelerates from their current speed
        [HideInInspector] public float runDeccelAmount; // Actual force (multiplied with speedDiff) applied to player
        [Space(5)]
        [Range(0f, 1)] public float accelInAir; // Multipliers applied to acceleration rate when airborne
        [Range(0f, 1)] public float deccelInAir;
        [Space(5)]
        public bool doConserveMomentum = true;

        [Space(20)]

        [Header("Jump")]
        public float jumpHeight; // Height of player's jump
        public float jumpTimeToApex; // Time between applying the jump force and reaching the desired jump height. These values also control player's gravity and jump force.
        [HideInInspector] public float jumpForce; // The actual force applied to player when they jump
        [Space(0.5f)]

        [Header("Both Jumps")]
        public float jumpCutGravityMult; // Multiplier to increase gravity if cplayer releases the jump button while still jumping
        [Range(0f, 1)] public float jumpHangGravityMult; // Reduces gravity while close to the apex of the jump
        public float jumpHangTimeThreshold; // Speeds where player will experience extra "jump hang"
        [Space(0.5f)]
        public float jumpHangAccelerationMult;
        public float jumpHangMaxSpeedMult;

        [Header("Wall Jump")]
        public Vector2 wallJumpForce; // The actual force applied to player when wall jumping
        [Space(5)]
        [Range(0f, 1f)] public float wallJumpRunLerp; // Reduces the effect of player's movement while wall jumping
        [Range(0f, 1.5f)] public float wallJumpTime; // Time after wall jumping player's movement is slowed for
        public bool doTurnOnWallJump; // Player will rotate to face wall jumping direction
        [Space(5)]

        [Space(20)]

        [Header("Assists")]
        [Range(0.01f, 0.5f)] public float coyoteTime; // Grace period after falling off a platform, where you can still jump
        [Range(0.01f, 0.5f)] public float jumpInputBufferTime; // Grace period after pressing jump where a jump will be automatically performed once the requirements are met

        [Header("Audio Clips")]
        public AudioClip jumpSound;
        public AudioClip wallJumpSound;
        public AudioClip landSound;


        private void OnValidate()
        {
            // Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
            gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

            // Calculate the rigidbody's gravity scale
            gravityScale = gravityStrength / Physics2D.gravity.y;

            // Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
            runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
            runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

            // Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
            jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

            #region Variable Ranges
            runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
            runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
            #endregion
        }
    }
}
