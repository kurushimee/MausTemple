using UnityEngine;
using UnityEngine.InputSystem;

namespace MausTemple
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _speed;

        private Rigidbody2D _rb;
        private float _movement;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            _rb.velocity = new Vector2(_movement * _speed, _rb.velocity.y);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _movement = context.ReadValue<float>();
        }
    }
}
