using TMPro;
using UnityEngine;

namespace MenuGraphTool
{
    public class AttackMenu : MenuPage
    {
        [SerializeField] private TextMeshProUGUI _textField;

        [MenuInput] private Character Character;
        [MenuInput] private float Num;

        private void Start()
        {
            _textField.text = $"{Character.Name} {Num.ToString()}";
        }
    }
}