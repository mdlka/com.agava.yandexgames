using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace Agava.YandexGames
{
    public static class Flags
    {
        private static Action<Dictionary<string, string>> s_onGetFlagsSuccessCallback;
        private static Action<string> s_onGetFlagsErrorCallback;

        public static void Get(Dictionary<string, string> defaultFlags = null, ClientFeature[] clientFeatures = null, 
            Action<Dictionary<string, string>> onSuccessCallback = null, Action<string> onErrorCallback = null)
        {
            s_onGetFlagsSuccessCallback = onSuccessCallback;
            s_onGetFlagsErrorCallback = onErrorCallback;
            
            string defaultFlagsJson = defaultFlags == null ? "{}" : defaultFlags.ToJson();
            string clientFeaturesJson = clientFeatures == null ? "[]" : JsonUtility.ToJson(clientFeatures);
            
            GetFlags(defaultFlagsJson, clientFeaturesJson, OnGetFlagsSuccessCallback, OnGetFlagsErrorCallback);
        }

        [DllImport("__Internal")]
        private static extern void GetFlags(string defaultFlags, string clientFeatures, Action<string> onSuccessCallback, Action<string> onErrorCallback);

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnGetFlagsSuccessCallback(string flagsJson)
        {
            if (YandexGamesSdk.CallbackLogging)
                Debug.Log($"{nameof(Flags)}.{nameof(OnGetFlagsSuccessCallback)} called. {nameof(flagsJson)}={flagsJson}");

            s_onGetFlagsSuccessCallback?.Invoke(flagsJson.DictionaryFromJson());
        }
        
        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnGetFlagsErrorCallback(string errorMessage)
        {
            if (YandexGamesSdk.CallbackLogging)
                Debug.Log($"{nameof(Flags)}.{nameof(OnGetFlagsErrorCallback)} invoked, {nameof(errorMessage)} = {errorMessage}");

            s_onGetFlagsErrorCallback?.Invoke(errorMessage);
        }
    }
}