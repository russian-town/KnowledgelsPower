using System;
using CodeBase.Data;
using CodeBase.Infrastructure.Services.PersistentProgress;
using UnityEngine;

namespace CodeBase.Hero
{
    public class HeroHealth : MonoBehaviour, ISavedProgress
    {
        public HeroAnimator Animator;

        private State _state;

        public float Current
        {
            get => _state.CurrentHP;
            set
            {
                if (_state.CurrentHP != value)
                {
                    _state.CurrentHP = value;
                    HealthChanged?.Invoke();
                }
            }
        }

        public float Max
        {
            get => _state.MaxHP;
            set => _state.MaxHP = value;
        }

        public event Action HealthChanged;

        public void LoadProgress(PlayerProgress progress)
        {
            _state = progress.HeroState;
            HealthChanged?.Invoke();
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            progress.HeroState.CurrentHP = Current;
            progress.HeroState.MaxHP = Max;
        }

        public void TakeDamage(float damage)
        {
            if (Current <= 0f)
                return;

            Current -= damage;
            Animator.PlayHit();
        }
    }
}