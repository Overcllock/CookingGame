using Game.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Windows
{
    public class HUDLayout : UIBaseWindowLayout
    {
        public Button pauseButton;

        [Space]
        public TMP_Text overallGuestsLabel;
        public TMP_Text currentGuestsLabel;

        public AnimatedSlider currentGuestsSlider;
        
        [Space]
        public TMP_Text timeLabel;
    }
}