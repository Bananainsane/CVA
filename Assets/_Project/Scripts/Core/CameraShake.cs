using UnityEngine;

namespace CVA.Core
{
    /// <summary>
    /// Camera shake effect for impactful feedback.
    /// Supports multiple simultaneous shakes with stacking intensity.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraShake : MonoBehaviour
    {
        [Header("Shake Settings")]
        [SerializeField] private float _shakeMultiplier = 1f;
        [SerializeField] private bool _enableShake = true;

        // Runtime state
        private Vector3 _originalPosition;
        private float _currentShakeIntensity = 0f;
        private float _currentShakeDuration = 0f;
        private float _shakeTimer = 0f;

        #region Unity Lifecycle

        private void Awake()
        {
            _originalPosition = transform.localPosition;
        }

        private void Update()
        {
            if (!_enableShake)
                return;

            if (_shakeTimer > 0f)
            {
                _shakeTimer -= Time.deltaTime;

                // Apply shake offset
                Vector3 shakeOffset = Random.insideUnitCircle * _currentShakeIntensity * _shakeMultiplier;
                transform.localPosition = _originalPosition + shakeOffset;

                // Decay shake intensity
                float decayRate = _currentShakeIntensity / _currentShakeDuration;
                _currentShakeIntensity = Mathf.Max(0f, _currentShakeIntensity - decayRate * Time.deltaTime);
            }
            else
            {
                // Reset to original position
                transform.localPosition = _originalPosition;
                _currentShakeIntensity = 0f;
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Trigger a camera shake with custom intensity and duration.
        /// </summary>
        /// <param name="intensity">Shake strength (0-1 range recommended)</param>
        /// <param name="duration">How long the shake lasts in seconds</param>
        public void Shake(float intensity, float duration)
        {
            if (!_enableShake)
                return;

            // Stack shakes if one is already playing
            if (_shakeTimer > 0f)
            {
                _currentShakeIntensity += intensity;
                _shakeTimer = Mathf.Max(_shakeTimer, duration);
            }
            else
            {
                _currentShakeIntensity = intensity;
                _currentShakeDuration = duration;
                _shakeTimer = duration;
            }
        }

        /// <summary>
        /// Light shake (e.g., for collecting XP orbs or small hits).
        /// </summary>
        public void ShakeCameraLight()
        {
            Shake(0.1f, 0.1f);
        }

        /// <summary>
        /// Medium shake (e.g., for explosions or taking damage).
        /// </summary>
        public void ShakeCameraMedium()
        {
            Shake(0.3f, 0.2f);
        }

        /// <summary>
        /// Heavy shake (e.g., for boss attacks or player death).
        /// </summary>
        public void ShakeCameraHeavy()
        {
            Shake(0.5f, 0.3f);
        }

        /// <summary>
        /// Stop all camera shake immediately.
        /// </summary>
        public void StopShake()
        {
            _shakeTimer = 0f;
            _currentShakeIntensity = 0f;
            transform.localPosition = _originalPosition;
        }

        #endregion
    }
}
