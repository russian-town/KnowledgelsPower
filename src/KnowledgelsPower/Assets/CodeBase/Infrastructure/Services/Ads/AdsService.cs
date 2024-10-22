using System;
using UnityEngine;
using UnityEngine.Advertisements;

namespace CodeBase.Infrastructure.Services.Ads
{
    public class AdsService : IAdsService, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener 
    {
        private const string AndroidGameId = "5717397";
        private const string IOSGameId = "5717396";
        private const string RewardedVideoPlacementId = "Rewarded_Android";

        private string _gameId;
        private Action _onVideoFinished;

        public bool IsRewardedVideoReady { get; private set; }
        public int Reward => 13;

        public event Action RewardedVideoReady;

        public void Initialize()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    _gameId = AndroidGameId;
                    break;
                case RuntimePlatform.IPhonePlayer:
                    _gameId = IOSGameId;
                    break;
                case RuntimePlatform.WindowsEditor:
                    _gameId = AndroidGameId;
                    break;
            }
            
            Advertisement.Initialize(_gameId, true, this);
        }

        public void OnInitializationComplete()
        {
            IsRewardedVideoReady = true;
            Debug.Log("OnInitializationComplete");
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message) => 
            Debug.Log($"OnInitializationFailed {message}");

        public void ShowRewardedVideo(Action onVideoFinished)
        {
            Advertisement.Show(RewardedVideoPlacementId, this);
            _onVideoFinished = onVideoFinished;
        }
        
        
        public void OnUnityAdsAdLoaded(string placementId)
        {
            Debug.Log($"OnUnityAdsAdLoaded {placementId}");

            if (placementId == RewardedVideoPlacementId)
            {
                IsRewardedVideoReady = true;
                RewardedVideoReady?.Invoke();
            }
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) => 
            Debug.Log($"OnUnityAdsFailedToLoad {message}");

        public void OnUnityAdsShowStart(string placementId) => 
            Debug.Log($"OnUnityAdsShowStart {placementId}");

        public void OnUnityAdsShowClick(string placementId) => 
            Debug.Log($"OnUnityAdsShowClick {placementId}");

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            switch (showCompletionState)
            {
                case UnityAdsShowCompletionState.SKIPPED:
                    Debug.LogError($"OnUnityAdsShowComplete {showCompletionState}");
                    break;
                case UnityAdsShowCompletionState.COMPLETED:
                    _onVideoFinished?.Invoke();
                    break;
                case UnityAdsShowCompletionState.UNKNOWN:
                    Debug.LogError($"OnUnityAdsShowComplete {showCompletionState}");
                    break;
                default:
                    Debug.LogError($"OnUnityAdsShowComplete {showCompletionState}");
                    break;
            }

            IsRewardedVideoReady = false;
            _onVideoFinished = null;
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) => 
            Debug.Log($"OnUnityAdsShowFailure {message}");
    }
}