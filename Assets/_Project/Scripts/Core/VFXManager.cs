using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

namespace CVA.Core
{
    /// <summary>
    /// Centralized VFX management system with object pooling.
    /// Handles hit effects, death effects, and other particle systems.
    /// </summary>
    public class VFXManager : MonoBehaviour
    {
        private static VFXManager _instance;

        [Header("VFX Prefabs")]
        [SerializeField] private GameObject _hitEffectPrefab;
        [SerializeField] private GameObject _deathEffectPrefab;
        [SerializeField] private GameObject _levelUpEffectPrefab;

        [Header("Pooling Settings")]
        [SerializeField] private int _defaultPoolSize = 20;
        [SerializeField] private int _maxPoolSize = 100;

        // Object pools
        private Dictionary<GameObject, ObjectPool<VFXPoolable>> _effectPools;

        #region Unity Lifecycle

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            InitializePools();
        }

        #endregion

        #region Initialization

        private void InitializePools()
        {
            _effectPools = new Dictionary<GameObject, ObjectPool<VFXPoolable>>();

            // We'll create pools on-demand when effects are first requested
        }

        private ObjectPool<VFXPoolable> GetOrCreatePool(GameObject prefab)
        {
            if (prefab == null)
                return null;

            if (!_effectPools.ContainsKey(prefab))
            {
                _effectPools[prefab] = new ObjectPool<VFXPoolable>(
                    createFunc: () => CreatePooledEffect(prefab),
                    actionOnGet: (obj) => obj.OnSpawnedFromPool(),
                    actionOnRelease: (obj) => obj.OnReturnedToPool(),
                    actionOnDestroy: (obj) => Destroy(obj.gameObject),
                    collectionCheck: true,
                    defaultCapacity: _defaultPoolSize,
                    maxSize: _maxPoolSize
                );
            }

            return _effectPools[prefab];
        }

        private VFXPoolable CreatePooledEffect(GameObject prefab)
        {
            GameObject obj = Instantiate(prefab);
            VFXPoolable poolable = obj.GetComponent<VFXPoolable>();

            if (poolable == null)
            {
                poolable = obj.AddComponent<VFXPoolable>();
            }

            obj.transform.parent = transform;
            return poolable;
        }

        #endregion

        #region Static API

        /// <summary>
        /// Play hit effect at position.
        /// </summary>
        public static void Hit(Vector3 position, Color? color = null)
        {
            if (_instance == null)
                return;

            _instance.PlayHitEffect(position, color);
        }

        /// <summary>
        /// Play death effect at position.
        /// </summary>
        public static void Death(Vector3 position, Color? color = null)
        {
            if (_instance == null)
                return;

            _instance.PlayDeathEffect(position, color);
        }

        /// <summary>
        /// Play level up effect at position.
        /// </summary>
        public static void LevelUp(Vector3 position)
        {
            if (_instance == null)
                return;

            _instance.PlayLevelUpEffect(position);
        }

        #endregion

        #region Effect Playback

        private void PlayHitEffect(Vector3 position, Color? color)
        {
            if (_hitEffectPrefab != null)
            {
                PlayEffect(_hitEffectPrefab, position, color);
            }
            else
            {
                // Fallback: create simple particle effect
                CreateSimpleParticleEffect(position, color ?? Color.white, 0.2f, 10);
            }
        }

        private void PlayDeathEffect(Vector3 position, Color? color)
        {
            if (_deathEffectPrefab != null)
            {
                PlayEffect(_deathEffectPrefab, position, color);
            }
            else
            {
                // Fallback: create simple particle effect
                CreateSimpleParticleEffect(position, color ?? Color.red, 0.5f, 20);
            }
        }

        private void PlayLevelUpEffect(Vector3 position)
        {
            if (_levelUpEffectPrefab != null)
            {
                PlayEffect(_levelUpEffectPrefab, position, Color.yellow);
            }
            else
            {
                // Fallback
                CreateSimpleParticleEffect(position, Color.yellow, 1f, 30);
            }
        }

        private void PlayEffect(GameObject prefab, Vector3 position, Color? color)
        {
            ObjectPool<VFXPoolable> pool = GetOrCreatePool(prefab);
            if (pool == null)
                return;

            VFXPoolable effect = pool.Get();
            effect.transform.position = position;

            // Apply color if specified
            if (color.HasValue)
            {
                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    var main = ps.main;
                    main.startColor = color.Value;
                }
            }

            // Auto-return to pool after duration
            float duration = GetEffectDuration(effect.gameObject);
            StartCoroutine(ReturnToPoolAfterDelay(effect, pool, duration));
        }

        private System.Collections.IEnumerator ReturnToPoolAfterDelay(VFXPoolable effect, ObjectPool<VFXPoolable> pool, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (effect != null && pool != null)
            {
                pool.Release(effect);
            }
        }

        private float GetEffectDuration(GameObject effectPrefab)
        {
            ParticleSystem ps = effectPrefab.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                return ps.main.duration + ps.main.startLifetime.constantMax;
            }

            return 1f; // Default duration
        }

        #endregion

        #region Fallback Effects

        private void CreateSimpleParticleEffect(Vector3 position, Color color, float duration, int particleCount)
        {
            GameObject effectGO = new GameObject("SimpleVFX");
            effectGO.transform.position = position;

            SimpleParticleEffect effect = effectGO.AddComponent<SimpleParticleEffect>();
            effect.Initialize(color, duration, particleCount);
        }

        #endregion
    }

    /// <summary>
    /// Wrapper component to make GameObjects work with ObjectPool.
    /// </summary>
    public class VFXPoolable : MonoBehaviour, IPoolable
    {
        public void OnSpawnedFromPool()
        {
            gameObject.SetActive(true);

            // Restart particle system if present
            ParticleSystem ps = GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Clear();
                ps.Play();
            }
        }

        public void OnReturnedToPool()
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Simple particle effect fallback when no prefab is assigned.
    /// </summary>
    public class SimpleParticleEffect : MonoBehaviour
    {
        private struct Particle
        {
            public Vector3 position;
            public Vector3 velocity;
            public float lifetime;
            public float maxLifetime;
        }

        private Particle[] _particles;
        private Color _color;
        private float _duration;

        public void Initialize(Color color, float duration, int count)
        {
            _color = color;
            _duration = duration;
            _particles = new Particle[count];

            for (int i = 0; i < count; i++)
            {
                _particles[i].position = transform.position;
                _particles[i].velocity = Random.insideUnitCircle.normalized * Random.Range(2f, 5f);
                _particles[i].lifetime = 0f;
                _particles[i].maxLifetime = duration;
            }

            Destroy(gameObject, duration);
        }

        private void Update()
        {
            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i].lifetime += Time.deltaTime;
                _particles[i].position += _particles[i].velocity * Time.deltaTime;
                _particles[i].velocity *= 0.95f; // Drag
            }
        }

        private void OnDrawGizmos()
        {
            if (_particles == null)
                return;

            for (int i = 0; i < _particles.Length; i++)
            {
                float alpha = 1f - (_particles[i].lifetime / _particles[i].maxLifetime);
                Gizmos.color = new Color(_color.r, _color.g, _color.b, alpha);
                Gizmos.DrawSphere(_particles[i].position, 0.1f);
            }
        }
    }
}
