using System.Collections.Generic;
using System.Threading.Tasks;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Services.IAP;
using CodeBase.Infrastructure.Services.PersistentProgress;
using UnityEngine;

namespace CodeBase.UI.Windows.Shop
{
    public class ShopItemsContainer : MonoBehaviour
    {
        private const string ShopItemPath = "ShopItem";

        public GameObject[] ShopUnavailableObjects;
        public Transform Parent;
        
        private readonly List<GameObject> _shopItems = new();
        
        private IIAPService _iapService;
        private IPersistentProgressService _progressService;
        private IAssets _assets;

        public void Construct(IIAPService iapService, IPersistentProgressService progressService, IAssets assets)
        {
            _iapService = iapService;
            _progressService = progressService;
            _assets = assets;
        }

        public void Initialize() =>
            RefreshAvailableItems();

        public void Subscribe()
        {
            _iapService.Initialized += RefreshAvailableItems;
            _progressService.Progress.PurchaseData.Changed += RefreshAvailableItems;
        }

        public void Cleanup()
        {
            _iapService.Initialized -= RefreshAvailableItems;
            _progressService.Progress.PurchaseData.Changed -= RefreshAvailableItems;
        }

        private async void RefreshAvailableItems()
        {
            UpdateShopUnavailableObjects();

            if (!_iapService.IsInitialized)
                return;

            ClearShopItems();

            await FillShopItems();
        }

        private void ClearShopItems()
        {
            foreach (var shopItem in _shopItems)
                Destroy(shopItem);
        }

        private async Task FillShopItems()
        {
            foreach (var productDescription in _iapService.Products())
            {
                var shopItemObject = await _assets.Instantiate(ShopItemPath, Parent);
                var shopItem = shopItemObject.GetComponent<ShopItem>();
                shopItem.Construct(_iapService, _assets, productDescription);
                shopItem.Initialize();
                _shopItems.Add(shopItemObject);
            }
        }


        private void UpdateShopUnavailableObjects()
        {
            foreach (var shopUnavailableObject in ShopUnavailableObjects)
                shopUnavailableObject.SetActive(!_iapService.IsInitialized);
        }
    }
}