using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Services.IAP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Windows.Shop
{
    public class ShopItem : MonoBehaviour
    {
        public Button ByItemButton;
        public TextMeshProUGUI PriceText;
        public TextMeshProUGUI QuantityText;
        public TextMeshProUGUI AvailableItemsLeftText;
        public Image Icon;

        private IIAPService _iapService;
        private IAssets _assets;
        private ProductDescription _productDescription;

        public void Construct(
            IIAPService iapService,
            IAssets assets,
            ProductDescription productDescription)
        {
            _iapService = iapService;
            _assets = assets;
            _productDescription = productDescription;
        }

        public async void Initialize()
        {
            ByItemButton.onClick.AddListener(ByItemButtonClicked);
            PriceText.text = _productDescription.Config.Price;

            QuantityText.text = _productDescription
                .Config
                .Quantity
                .ToString();

            AvailableItemsLeftText.text = _productDescription
                .AvailablePurchaseLeft
                .ToString();

            Icon.sprite = await _assets.Load<Sprite>(_productDescription.Config.Icon);
        }

        private void ByItemButtonClicked() =>
            _iapService.StartPurchase(_productDescription.Id);
    }
}