using System.Collections.Generic;
using System.Linq;
using CodeBase.StaticData;
using CodeBase.StaticData.Windows;
using CodeBase.UI.Services.Windows;
using UnityEngine;

namespace CodeBase.Services
{
    public class StaticDataService : IStaticDataService
    {
        private const string StaticDataMonsterPath = "StaticData/Monster";
        private const string StaticDataLevelsPath = "StaticData/Levels";
        private const string StaticDataWindowsPath = "StaticData/UI/WindowData";
        
        private Dictionary<MonsterTypeId, MonsterStaticData> _monsters;
        private Dictionary<string, LevelStaticData> _levels;
        private Dictionary<WindowId,WindowConfig> _windowConfigs;

        public void Load()
        {
            _monsters = Resources
                .LoadAll<MonsterStaticData>(StaticDataMonsterPath)
                .ToDictionary(x => x.MonsterTypeId, x => x);

            _levels = Resources
                .LoadAll<LevelStaticData>(StaticDataLevelsPath)
                .ToDictionary(x => x.LevelKey, x => x);

            _windowConfigs = Resources
                .Load<WindowStaticData>(StaticDataWindowsPath)
                .Configs
                .ToDictionary(x => x.WindowId, x => x);
        }

        public MonsterStaticData ForMonster(MonsterTypeId typeId) =>
            _monsters.TryGetValue(typeId, out MonsterStaticData staticData)
                ? staticData
                : null;

        public LevelStaticData ForLevel(string sceneKey) =>
            _levels.TryGetValue(sceneKey, out LevelStaticData staticData)
                ? staticData
                : null;

        public WindowConfig ForWindow(WindowId id) =>
            _windowConfigs.TryGetValue(id, out WindowConfig windowConfig)
                ? windowConfig
                : null;
    }
}