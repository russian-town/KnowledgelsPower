using CodeBase.Infrastructure.Services;
using CodeBase.StaticData;

namespace CodeBase.Services
{
    public interface IStaticDataService : IService
    {
        void LoadMonsters();
        MonsterStaticData ForMonster(MonsterTypeId typeId);
    }
}