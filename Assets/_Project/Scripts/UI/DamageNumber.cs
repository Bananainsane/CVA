using UnityEngine;
using TMPro;

namespace CVA.UI
{
    /// <summary>
    /// Floating damage number that appears when enemies take damage.
    /// </summary>
    public class DamageNumber : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _floatSpeed = 2f;
        [SerializeField] private float _lifetime = 1f;
        [SerializeField] private float _fadeStart = 0.5f;

        private TextMeshPro _text;
        private float _age = 0f;
        private Color _startColor;

        /// <summary>
        /// Initialize and spawn a damage number.
        /// </summary>
        public static void Create(Vector3 position, float damage)
        {
            GameObject go = new GameObject("DamageNumber");
            go.transform.position = position;

            DamageNumber dn = go.AddComponent<DamageNumber>();
            dn.Initialize(damage);
        }

        private void Initialize(float damage)
        {
            // Create TextMeshPro
            _text = gameObject.AddComponent<TextMeshPro>();
            _text.text = Mathf.CeilToInt(damage).ToString();
            _text.fontSize = 3;
            _text.alignment = TextAlignmentOptions.Center;
            _text.color = Color.white;
            _text.sortingOrder = 100;

            // Add slight outline
            _text.outlineWidth = 0.2f;
            _text.outlineColor = Color.black;

            _startColor = _text.color;

            // Random slight offset
            float randomX = Random.Range(-0.3f, 0.3f);
            transform.position += new Vector3(randomX, 0, 0);
        }

        private void Update()
        {
            if (_text == null)
                return;

            _age += Time.deltaTime;

            // Float upward
            transform.position += Vector3.up * _floatSpeed * Time.deltaTime;

            // Fade out
            if (_age > _fadeStart)
            {
                float fadePercent = 1f - ((_age - _fadeStart) / (_lifetime - _fadeStart));
                Color c = _startColor;
                c.a = fadePercent;
                _text.color = c;
            }

            // Destroy when lifetime expires
            if (_age >= _lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
