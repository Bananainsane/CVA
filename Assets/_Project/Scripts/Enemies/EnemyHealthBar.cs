using UnityEngine;

namespace CVA.Enemies
{
    /// <summary>
    /// Floating health bar above enemy using LineRenderer.
    /// </summary>
    public class EnemyHealthBar : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _barWidth = 1f;
        [SerializeField] private float _barHeight = 0.15f;
        [SerializeField] private float _yOffset = 0.6f;

        [Header("Colors")]
        [SerializeField] private Color _backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        [SerializeField] private Color _healthColorHigh = new Color(0.2f, 1f, 0.2f); // Green
        [SerializeField] private Color _healthColorLow = new Color(1f, 0.2f, 0.2f); // Red

        private GameObject _backgroundBar;
        private GameObject _fillBar;
        private LineRenderer _backgroundRenderer;
        private LineRenderer _fillRenderer;

        private Enemy _enemy;
        private float _currentHealthPercent = 1f;

        private void Awake()
        {
            _enemy = GetComponent<Enemy>();
            CreateHealthBar();
        }

        private void LateUpdate()
        {
            // Always face camera and stay above enemy
            UpdatePosition();
        }

        private void CreateHealthBar()
        {
            // Background bar
            _backgroundBar = new GameObject("HealthBar_Background");
            _backgroundBar.transform.SetParent(transform);
            _backgroundBar.transform.localPosition = Vector3.up * _yOffset;

            _backgroundRenderer = _backgroundBar.AddComponent<LineRenderer>();
            _backgroundRenderer.startWidth = _barHeight;
            _backgroundRenderer.endWidth = _barHeight;
            _backgroundRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _backgroundRenderer.startColor = _backgroundColor;
            _backgroundRenderer.endColor = _backgroundColor;
            _backgroundRenderer.sortingOrder = 5;
            _backgroundRenderer.useWorldSpace = false;
            _backgroundRenderer.positionCount = 2;
            _backgroundRenderer.SetPosition(0, Vector3.left * _barWidth * 0.5f);
            _backgroundRenderer.SetPosition(1, Vector3.right * _barWidth * 0.5f);

            // Fill bar
            _fillBar = new GameObject("HealthBar_Fill");
            _fillBar.transform.SetParent(transform);
            _fillBar.transform.localPosition = Vector3.up * _yOffset;

            _fillRenderer = _fillBar.AddComponent<LineRenderer>();
            _fillRenderer.startWidth = _barHeight * 0.8f;
            _fillRenderer.endWidth = _barHeight * 0.8f;
            _fillRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _fillRenderer.startColor = _healthColorHigh;
            _fillRenderer.endColor = _healthColorHigh;
            _fillRenderer.sortingOrder = 6;
            _fillRenderer.useWorldSpace = false;
            _fillRenderer.positionCount = 2;

            UpdateFillBar();
        }

        private void UpdatePosition()
        {
            if (_backgroundBar != null)
            {
                _backgroundBar.transform.localPosition = Vector3.up * _yOffset;
            }
            if (_fillBar != null)
            {
                _fillBar.transform.localPosition = Vector3.up * _yOffset;
            }
        }

        /// <summary>
        /// Update health bar to reflect current health.
        /// </summary>
        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            _currentHealthPercent = Mathf.Clamp01(currentHealth / maxHealth);
            UpdateFillBar();
        }

        private void UpdateFillBar()
        {
            if (_fillRenderer == null)
                return;

            // Update fill width based on health percentage
            float fillWidth = _barWidth * _currentHealthPercent;
            _fillRenderer.SetPosition(0, Vector3.left * _barWidth * 0.5f);
            _fillRenderer.SetPosition(1, Vector3.left * _barWidth * 0.5f + Vector3.right * fillWidth);

            // Color gradient from green to red
            Color healthColor = Color.Lerp(_healthColorLow, _healthColorHigh, _currentHealthPercent);
            _fillRenderer.startColor = healthColor;
            _fillRenderer.endColor = healthColor;
        }

        private void OnDestroy()
        {
            if (_backgroundBar != null)
                Destroy(_backgroundBar);
            if (_fillBar != null)
                Destroy(_fillBar);
        }
    }
}
