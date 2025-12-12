using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MenuGraphTool
{
    public class InformationPopupMenu : MenuPage
    {
        #region Fields
        [MenuInput]
        private string _information;

        // Visual Elements
        [SerializeField] private TextMeshProUGUI _informationLabel;
        #endregion Fields


        #region Methods
        private void Start()
        {
            if (_information == null)
            {
                return;
            }

            _informationLabel.text = _information;
        }
        #endregion Methods
    }
}