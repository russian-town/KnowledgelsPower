using System;
using System.Collections;
using UnityEngine;

namespace CodeBase.Enemy
{
    public class EnemyDeath : MonoBehaviour
    {
        public EnemyHealth Health;
        public EnemyAnimator Animator;
        public AgentMoveToHero AgentMoveToHero;
        public GameObject DeathFx;

        public event Action Happened;

        private void Start() =>
            Health.HealthChanged += OnHealthChanged;

        private void OnDestroy() =>
            Health.HealthChanged -= OnHealthChanged;

        private void OnHealthChanged()
        {
            if (Health.Current <= 0f)
                Die();
        }

        private void Die()
        {
            Health.HealthChanged -= OnHealthChanged;
            Animator.PlayDeath();
            AgentMoveToHero.enabled = false;
            SpawnDeathFx();
            StartCoroutine(DestroyTimer());
            Happened?.Invoke();
        }

        private void SpawnDeathFx() =>
            Instantiate(DeathFx, transform.position, Quaternion.identity);

        private IEnumerator DestroyTimer()
        {
            yield return new WaitForSeconds(3f);
            Destroy(gameObject);
        }
    }
}