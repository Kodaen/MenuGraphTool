using TMPro;
using UnityEngine;

namespace MenuGraphTool
{
    public class AttackMenu : MenuPage
    {
        [SerializeField] private TextMeshProUGUI _textField;

        [MenuInput] private Character Character;

        private void Start()
        {
            _textField.text = Character.Name;
        }
    }
}