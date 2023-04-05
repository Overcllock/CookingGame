using UnityEngine;

namespace Game.Content.UI
{
    [System.Serializable]
    public class UIModuleSettings : ContentModuleSettings
    {
        public Color positiveColor;
        public Color negativeColor;
        public Color neutralColor;

        public Color lightPositiveColor;
        public Color lightNegativeColor;
        public Color lightNeutralColor;
        
        public Color moneyPositiveColor;
        public Color moneyNegativeColor;
    }
}