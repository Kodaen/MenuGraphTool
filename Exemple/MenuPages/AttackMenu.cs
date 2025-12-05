using TMPro;
using UnityEngine;

namespace MenuGraphTool
{
    public class AttackMenu : MenuPage
    {
        [SerializeField] private TextMeshProUGUI _textField;

        [MenuInput] private Character _character;
        [MenuInput] private int _num;

        private void Start()
        {
            _textField.text = $"{_character.Name} {_num.ToString()}";
        }
    }
}