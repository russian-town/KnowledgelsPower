using CodeBase.Data;
using CodeBase.Infrastructure.Factory;
using CodeBase.Services;
using UnityEngine;

namespace CodeBase.Enemy
{
    public class LootSpawner : MonoBehaviour
    {
        public EnemyDeath EnemyDeath;
        
        private IGameFactory _factory;
        private IRandomService _random;
        private int _lootMin;
        private int _lootMax;

        public void Construct(IGameFactory factory, IRandomService random)
        {
            _factory = factory;
            _random = random;
        }

        private void Start() => 
            EnemyDeath.Happened += SpawnLoot;

        public void SetLoot(int min, int max)
        {
            _lootMin = min;
            _lootMax = max;
        }
        
        private async void SpawnLoot()
        {
            EnemyDeath.Happened -= SpawnLoot;
            var loot = await _factory.CreateLoot();
            loot.transform.position = transform.position;
            var lootItem = GenerateLoot();
            loot.Initialize(lootItem);
        }

        private Loot GenerateLoot()
        {
            return new Loot()
            {
                Value = _random.Next(_lootMin, _lootMax)
            };
        }
    }
}