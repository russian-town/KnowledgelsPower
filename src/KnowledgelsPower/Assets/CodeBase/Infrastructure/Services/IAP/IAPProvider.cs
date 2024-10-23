using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace CodeBase.Infrastructure.Services.IAP
{
    public class IAPProvider : IDetailedStoreListener
    {
        private const string IAPConfigsPath = "IAP/products";

        private IStoreController _controller;
        private IExtensionProvider _extensions;
        private IAPService _iapService;

        public Dictionary<string, ProductConfig> Configs { get; private set; }
        public Dictionary<string, Product> Products { get; private set; }

        public event Action Initialized;

        public bool IsInitialized => _controller != null && _extensions != null;

        public void Initialize(IAPService iapService)
        {
            _iapService = iapService;
            Configs = new Dictionary<string, ProductConfig>();
            Products = new Dictionary<string, Product>();
            Load();
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var config in Configs.Values)
                builder.AddProduct(config.Id, config.ProductType);

            UnityPurchasing.Initialize(this, builder);
        }

        public void StartPurchase(string productId) =>
            _controller.InitiatePurchase(productId);

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _controller = controller;
            _extensions = extensions;

            foreach (var product in _controller.products.all)
                Products.Add(product.definition.id, product);
            
            Initialized?.Invoke();
            Debug.Log("UnityPurchasing initialization success.");
        }

        public void OnInitializeFailed(InitializationFailureReason error) =>
            Debug.Log($"UnityPurchasing initialization: {error}.");

        public void OnInitializeFailed(InitializationFailureReason error, string message) =>
            Debug.Log($"UnityPurchasing initialization: {error}. {message}");

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            Debug.Log($"Process purchase success {purchaseEvent.purchasedProduct.definition.id}.");
            return _iapService.ProcessPurchase(purchaseEvent.purchasedProduct);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription) =>
            Debug.LogError(
                $"Product {product.definition.id} purchase failed." +
                $"Purchase failure description {failureDescription}." +
                $"Transaction id {product.transactionID}.");

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) =>
            Debug.LogError(
                $"Product {product.definition.id} purchase failed." +
                $"Purchase failure reason {failureReason}." +
                $"Transaction id {product.transactionID}.");

        private void Load() =>
            Configs = Resources.Load<TextAsset>(IAPConfigsPath)
                .text
                .AsDeserialized<ProductConfigWrapper>()
                .Configs
                .ToDictionary(x => x.Id, x => x);
    }
}