using UnityEngine;

namespace CVA.Core
{
    /// <summary>
    /// Smooth camera follow for top-down 2D games.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform _target;
        [SerializeField] private bool _autoFindPlayer = true;

        [Header("Follow Settings")]
        [SerializeField] private float _smoothSpeed = 5f;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 0f, -10f);

        [Header("Bounds (Optional)")]
        [SerializeField] private bool _useBounds = false;
        [SerializeField] private Vector2 _minBounds = new Vector2(-50f, -50f);
        [SerializeField] private Vector2 _maxBounds = new Vector2(50f, 50f);

        #region Unity Lifecycle

        private void Start()
        {
            if (_autoFindPlayer && _target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    _target = player.transform;
                }
            }
        }

        private void LateUpdate()
        {
            if (_target == null)
                return;

            Vector3 desiredPosition = _target.position + _offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);

            // Apply bounds if enabled
            if (_useBounds)
            {
                smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, _minBounds.x, _maxBounds.x);
                smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, _minBounds.y, _maxBounds.y);
            }

            transform.position = smoothedPosition;
        }

        #endregion

        #region Public API

        public void SetTarget(Transform newTarget)
        {
            _target = newTarget;
        }

        #endregion
    }
}
