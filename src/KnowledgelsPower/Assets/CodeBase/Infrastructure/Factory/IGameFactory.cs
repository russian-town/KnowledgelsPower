using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public interface IGameFactory : IService
    {
        GameObject CreateHero(GameObject initialPoint);
        void CreateHud();
    }
}