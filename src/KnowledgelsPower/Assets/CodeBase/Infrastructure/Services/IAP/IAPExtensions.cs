using UnityEngine;

namespace CodeBase.Infrastructure.Services.IAP
{
    public static class IAPExtensions
    {
        public static T AsDeserialized<T>(this string text) => 
            JsonUtility.FromJson<T>(text);
    }
}