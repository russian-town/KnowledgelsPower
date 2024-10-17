﻿using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services;
using UnityEngine;
using UnityEngine.AI;

namespace CodeBase.Enemy
{
    public class AgentMoveToHero : Follow
    {
        private const float MinimalDistance = 1f;

        public NavMeshAgent Agent;

        private Transform _heroTransform;
        private IGameFactory _gameFactory;

        private void Start()
        {
            _gameFactory = AllServices.Container.Single<IGameFactory>();

            if (_gameFactory.HeroGameObject != null)
                InitializeHeroTransform();
            else
                _gameFactory.HeroCreated += OnHeroCreated;
        }

        private void Update()
        {
            if(_heroTransform == null)
                return;
            
            if (HeroNotReached())
                Agent.destination = _heroTransform.position;
        }

        private bool HeroNotReached() =>
            Vector3.Distance(Agent.transform.position, _heroTransform.position) >= MinimalDistance;

        private void InitializeHeroTransform() => 
            _heroTransform = _gameFactory.HeroGameObject.transform;

        private void OnHeroCreated()
        {
            InitializeHeroTransform();
            _gameFactory.HeroCreated -= OnHeroCreated;
        }
    }
}