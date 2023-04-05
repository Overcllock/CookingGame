using UnityEngine;

namespace Game.Content.UI
{
    [CreateAssetMenu(menuName = "Content/GUI/Create Database", fileName = "GUIDatabase", order = 1001)]
    public class UIDatabaseScrobject : ScrobjectDatabase
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