using CodeBase.Data;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.Logic;
using CodeBase.Services.Input;
using UnityEngine;

namespace CodeBase.Hero
{
    public class HeroAttack : MonoBehaviour, ISavedProgressReader
    {
        private const string EnemiesLayerName = "Hittable";

        public HeroAnimator Animator;
        public CharacterController CharacterController;
        
        private readonly Collider[] _hits = new Collider[3];

        private int _layerMask;
        private IInputService _input;
        private Stats _stats;

        private void Awake()
        {
            _input = AllServices.Container.Single<IInputService>();
            _layerMask = 1 << LayerMask.NameToLayer(EnemiesLayerName);
        }

        private void Update()
        {
            if (CanAttack())
                StartAttack();
        }

        private void OnAttack()
        {
            var hitsCount = Hit();
            
            if (hitsCount > 0)
            {
                for (var i = 0; i < hitsCount; i++)
                {
                    _hits[i].transform.parent
                        .GetComponent<IHealth>()
                        .TakeDamage(_stats.Damage);
                }
            }
        }

        private void OnAttackEnded() { }

        public void LoadProgress(PlayerProgress progress) =>
            _stats = progress.HeroStats;

        private Vector3 StartPoint() =>
            new(transform.position.x, CharacterController.center.y / 2f, transform.position.z);

        private int Hit() =>
            Physics.OverlapSphereNonAlloc(StartPoint() + transform.forward, _stats.DamageRadius, _hits, _layerMask);

        private void StartAttack() =>
            Animator.PlayAttack();

        private bool CanAttack() =>
            _input.IsAttackButtonUp() && !Animator.IsAttacking;
    }
}