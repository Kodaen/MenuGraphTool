using TMPro;
using UnityEngine;

namespace MenuGraphTool
{
    [MenuOutput("Parameterless")]
    public class AttackMenu : MenuPage
    {
        [SerializeField] private TextMeshProUGUI _textField;

        //[MenuInput] private Character _character;
        [MenuInput] private int _num;

        [MenuOutput("Parameterless")]
        private int _num2;

        //private void Start()
        //{
        //    _textField.text = $"{_character.Name} {_num.ToString()}";
        //}
    }
}