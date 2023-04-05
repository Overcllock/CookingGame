using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Windows
{
    public class GameFieldGuestWidgetLayout : UIBaseLayout
    {
        public Image icon;

        [Space]
        public Image[] recipesIcons;

        [Space]
        public Slider timeSlider;
    }
}