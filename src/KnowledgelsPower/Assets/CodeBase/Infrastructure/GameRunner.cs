using UnityEngine;

namespace CodeBase.Infrastructure
{
    public class GameRunner : MonoBehaviour
    {
        public GameBootstrapper GameBootstrapperPrefab;
        
        private void Awake()
        {
            var gameBootstrapper = FindObjectOfType<GameBootstrapper>();

            if (gameBootstrapper == null)
                Instantiate(GameBootstrapperPrefab);
        }
    }
}