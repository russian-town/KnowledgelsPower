using System.Collections.Generic;
using System.Threading.Tasks;
using CodeBase.Enemy;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.Logic;
using CodeBase.Logic.EnemySpawners;
using CodeBase.Services;
using CodeBase.StaticData;
using CodeBase.UI.Elements;
using CodeBase.UI.Services.Windows;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace CodeBase.Infrastructure.Factory
{
    public class GameFactory : IGameFactory
    {
        private readonly IAssets _assets;
        private readonly IStaticDataService _staticData;
        private readonly IRandomService _random;
        private readonly IPersistentProgressService _progressService;
        private readonly IWindowService _windowService;

        private GameObject _heroGameObject;

        public List<ISavedProgressReader> ProgressReaders { get; } = new();
        public List<ISavedProgress> ProgressWriters { get; } = new();

        public GameFactory(IAssets assets, IStaticDataService staticData, IRandomService random,
            IPersistentProgressService progressService, IWindowService windowService)
        {
            _assets = assets;
            _staticData = staticData;
            _random = random;
            _progressService = progressService;
            _windowService = windowService;
        }

        public async Task WarmUp()
        {
            await _assets.Load<GameObject>(AssetAddress.Loot);
            await _assets.Load<GameObject>(AssetAddress.Spawner);
        }
        
        public async Task<GameObject> CreateHero(Vector3 at)
        {
            _heroGameObject = await InstantiateRegistered(AssetAddress.HeroPath, at);
            return _heroGameObject;
        }

        public async Task<GameObject> CreateHud()
        {
            var hud = await InstantiateRegistered(AssetAddress.HudPath);
            hud.GetComponentInChildren<LootCounter>()
                .Construct(_progressService.Progress.WorldData);

            foreach (var openWindowButton in hud.GetComponentsInChildren<OpenWindowButton>())
                openWindowButton.Construct(_windowService);

            return hud;
        }


        public async Task<GameObject> CreateMonster(MonsterTypeId typeId, Transform parent)
        {
            var monsterData = _staticData.ForMonster(typeId);
            var prefab = await _assets.Load<GameObject>(monsterData.PrefabReference);
            var monster = Object.Instantiate(prefab, parent.position, Quaternion.identity, parent);

            var health = monster.GetComponent<IHealth>();
            health.Current = monsterData.HP;
            health.Max = monsterData.HP;

            monster.GetComponent<ActorUI>().Construct(health);
            monster.GetComponent<AgentMoveToHero>().Construct(_heroGameObject.transform);
            monster.GetComponent<NavMeshAgent>().speed = monsterData.MoveSpeed;

            var lootSpawner = monster.GetComponentInChildren<LootSpawner>();
            lootSpawner.SetLoot(monsterData.MinLoot, monsterData.MaxLoot);
            lootSpawner.Construct(this, _random);

            var attack = monster.GetComponent<Attack>();
            attack.Construct(_heroGameObject.transform);
            attack.Damage = monsterData.Damage;
            attack.EffectiveDistance = monsterData.EffectiveDistance;
            attack.Cleavage = monsterData.Cleavage;
            attack.AttackCooldown = monsterData.AttackCooldown;

            monster.GetComponent<RotateToHero>()?.Construct(_heroGameObject.transform);
            return monster;
        }

        public async Task<LootPiece> CreateLoot()
        {
            var prefab = await _assets.Load<GameObject>(AssetAddress.Loot);
            var lootPiece = InstantiateRegistered(prefab)
                .GetComponent<LootPiece>();
            
            lootPiece.Construct(_progressService.Progress.WorldData);
            return lootPiece;
        }

        public async Task CreateSpawner(Vector3 at, string spawnerId, MonsterTypeId monsterTypeId)
        {
            var prefab = await _assets.Load<GameObject>(AssetAddress.Spawner);
            var spawner = InstantiateRegistered(prefab, at)
                .GetComponent<SpawnPoint>();
            
            spawner.Construct(this);
            spawner.Id = spawnerId;
            spawner.MonsterTypeId = monsterTypeId;
        }

        public void Cleanup()
        {
            ProgressReaders.Clear();
            ProgressWriters.Clear();
            _assets.Cleanup();
        }

        private void Register(ISavedProgressReader progressReader)
        {
            if (progressReader is ISavedProgress progressWriter)
                ProgressWriters.Add(progressWriter);

            ProgressReaders.Add(progressReader);
        }

        private GameObject InstantiateRegistered(GameObject prefab, Vector3 at)
        {
            var gameObject = Object.Instantiate(prefab, at, Quaternion.identity);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }

        private GameObject InstantiateRegistered(GameObject prefab)
        {
            var gameObject = Object.Instantiate(prefab);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }

        private async Task<GameObject> InstantiateRegistered(string prefabPath, Vector3 at)
        {
            var gameObject = await _assets.Instantiate(prefabPath, at);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }

        private async Task<GameObject> InstantiateRegistered(string prefabPath)
        {
            var gameObject = await _assets.Instantiate(prefabPath);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }

        private void RegisterProgressWatchers(GameObject hero)
        {
            foreach (var progressReader in hero.GetComponentsInChildren<ISavedProgressReader>())
                Register(progressReader);
        }
    }
}