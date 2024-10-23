using CodeBase.Data;
using CodeBase.Enemy;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.StaticData;
using UnityEngine;

namespace CodeBase.Logic.EnemySpawners
{
    public class SpawnPoint : MonoBehaviour, ISavedProgress
    {
        public MonsterTypeId MonsterTypeId;

        [SerializeField] private bool _slain;

        private IGameFactory _factory;
        private EnemyDeath _enemyDeath;

        public string Id { get; set; }
        public bool Slain => _slain;

        public void Construct(IGameFactory factory) => 
            _factory = factory;

        public void LoadProgress(PlayerProgress progress)
        {
            if (progress.KillData.CrearedSpawners.Contains(Id))
                _slain = true;
            else
                Spawn();
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            if (_slain)
                progress.KillData.CrearedSpawners.Add(Id);
        }

        private async void Spawn()
        {
            var monster = await _factory.CreateMonster(MonsterTypeId, transform);
            _enemyDeath = monster.GetComponent<EnemyDeath>();
            _enemyDeath.Happened += OnHappened;
        }

        private void OnHappened()
        {
            if (_enemyDeath != null)
                _enemyDeath.Happened -= OnHappened;

            _slain = true;
        }
    }
}