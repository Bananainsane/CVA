namespace CVA.Core
{
    /// <summary>
    /// Interface for objects that can be used with Unity's ObjectPool system.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// Called when object is taken from the pool (spawned).
        /// </summary>
        void OnSpawnedFromPool();

        /// <summary>
        /// Called when object is returned to the pool (despawned).
        /// </summary>
        void OnReturnedToPool();
    }
}
