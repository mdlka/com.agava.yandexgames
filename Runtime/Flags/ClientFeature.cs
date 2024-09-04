using System;
using UnityEngine.Scripting;

namespace Agava.YandexGames
{
    [Serializable]
    public class ClientFeature
    {
        [field: Preserve] public string name;
        [field: Preserve] public string value;
    }
}