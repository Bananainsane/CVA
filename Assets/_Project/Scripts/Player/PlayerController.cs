using UnityEngine;
using UnityEngine.InputSystem;

namespace CVA.Player
{
    /// <summary>
    /// Handles player movement using New Input System.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _baseMoveSpeed = 5f;

        // Components
        private Rigidbody2D _rb;
        private PowerupManager _powerupManager;

        #region Unity Lifecycle

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _powerupManager = GetComponent<PowerupManager>();
        }

        private void Update()
        {
            ReadInput();
        }

        private void FixedUpdate()
        {
            Move();
        }

        #endregion

        #region Input Handling

        private Vector2 _moveInput;

        private void ReadInput()
        {
            // Read input from keyboard using new Input System
            Vector2 input = Vector2.zero;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed) input.y += 1f;
                if (Keyboard.current.sKey.isPressed) input.y -= 1f;
                if (Keyboard.current.aKey.isPressed) input.x -= 1f;
                if (Keyboard.current.dKey.isPressed) input.x += 1f;
            }

            _moveInput = input;
        }

        #endregion

        #region Movement

        private void Move()
        {
            // Apply move speed multiplier from powerups
            float effectiveMoveSpeed = _baseMoveSpeed;
            if (_powerupManager != null)
            {
                effectiveMoveSpeed *= _powerupManager.MoveSpeedMultiplier;
            }

            Vector2 movement = _moveInput.normalized * effectiveMoveSpeed;
            _rb.linearVelocity = movement;
        }

        #endregion

        #region Public API

        public void SetMoveSpeed(float newSpeed)
        {
            _baseMoveSpeed = Mathf.Max(0f, newSpeed);
        }

        public void AddMoveSpeed(float amount)
        {
            _baseMoveSpeed += amount;
        }

        public float GetMoveSpeed()
        {
            // Return effective speed with multipliers applied
            float effectiveSpeed = _baseMoveSpeed;
            if (_powerupManager != null)
            {
                effectiveSpeed *= _powerupManager.MoveSpeedMultiplier;
            }
            return effectiveSpeed;
        }

        #endregion
    }
}
