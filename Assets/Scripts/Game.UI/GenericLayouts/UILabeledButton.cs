using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UILabeledButton : UIBaseLayout
    {
        public Button button;
        public float widthWithButton;
        public float widthWithoutButton;

        public TMP_Text label;

        [Header("Optional:")]
        public Image icon;
        public Image background;
        public Image highlight;

        protected override void Reset()
        {
            base.Reset();

            button = GetComponentInChildren<Button>(true);
            label = GetComponentInChildren<TMP_Text>(true);

            if (button == null)
            {
                button = gameObject.AddComponent<Button>();
            }

            if (label == null)
            {
                var tmpContainer = new GameObject("Label");
                tmpContainer.transform.SetParent(this.transform);
                label = tmpContainer.AddComponent<TextMeshProUGUI>();
            }
        }
    }
}