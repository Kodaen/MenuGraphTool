using TMPro;
using UnityEngine;

namespace MenuGraphTool
{
    public class ItempInspectionMenu : MenuPage
    {
        [MenuInput]
        private string item;

        // Visual Elements
        [SerializeField] private TextMeshProUGUI _itemNameHolder;

        private void Start()
        {
            if (item == null)
            {
                return;
            }

            _itemNameHolder.text = item;
        }
    }
}