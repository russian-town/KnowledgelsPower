using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CodeBase.Infrastructure.AssetManagement
{
    public class AssetProvider : IAssets
    {
        private readonly Dictionary<string, AsyncOperationHandle> _completeCache = new();
        private readonly Dictionary<string, List<AsyncOperationHandle>> _handles = new();

        public void Initialize() => 
            Addressables.InitializeAsync();

        public async Task<T> Load<T>(AssetReference assetReference) where T : class
        {
            if (_completeCache.TryGetValue(assetReference.AssetGUID, out AsyncOperationHandle completedHandle))
                return completedHandle.Result as T;

            return await RunWithCacheOnComplete(
                Addressables.LoadAssetAsync<T>(assetReference),
                cacheKey: assetReference.AssetGUID);
        }

        public async Task<T> Load<T>(string address) where T : class
        {
            if (_completeCache.TryGetValue(address, out AsyncOperationHandle completedHandle))
                return completedHandle.Result as T;

            return await RunWithCacheOnComplete(
                Addressables.LoadAssetAsync<T>(address),
                cacheKey: address);
        }

        public Task<GameObject> Instantiate(string address) => 
            Addressables.InstantiateAsync(address).Task;

        public Task<GameObject> Instantiate(string address, Vector3 at) => 
            Addressables.InstantiateAsync(address, at, Quaternion.identity).Task;

        public void Cleanup()
        {
            foreach (var resourceHandles in _handles.Values)
            foreach (var handle in resourceHandles)
                Addressables.Release(handle);
            
            _completeCache.Clear();
            _handles.Clear();
        }

        private async Task<T> RunWithCacheOnComplete<T>(AsyncOperationHandle<T> handle, string cacheKey) where T : class
        {
            handle.Completed += completeHandle => { _completeCache[cacheKey] = completeHandle; };
            AddHandle(cacheKey, handle);
            return await handle.Task;
        }

        private void AddHandle<T>(string key, AsyncOperationHandle<T> handle) where T : class
        {
            if (!_handles.TryGetValue(key, out List<AsyncOperationHandle> resourceHandles))
                _handles[key] = resourceHandles = new List<AsyncOperationHandle>();

            resourceHandles.Add(handle);
        }
    }
}