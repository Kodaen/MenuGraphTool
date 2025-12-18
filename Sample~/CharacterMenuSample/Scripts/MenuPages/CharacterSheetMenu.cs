using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MenuGraphTool
{
    public class CharacterSheetMenu : MenuPage
    {
        #region Consts
        private const string STAT_INFOS = "StatsInfos";
        #endregion Consts

        #region Fields
        [MenuInput]
        private Character _character;

        [MenuOutput(STAT_INFOS)]
        private string _selectedStatInfos;

        // Visual Elements
        [SerializeField] private TextMeshProUGUI _characterNameLabel;

        [SerializeField] private Button _HPButton;
        [SerializeField] private Button _AttackButton;
        [SerializeField] private Button _DefenseButton;

        [SerializeField] private TextMeshProUGUI _HPLabel;
        [SerializeField] private TextMeshProUGUI _AttackLabel;
        [SerializeField] private TextMeshProUGUI _DefenseLabel;
        #endregion Fields


        #region Methods
        private void Start()
        {
            if (_character == null)
            {
                // Warning message
                return;
            }
            
            _characterNameLabel.text = _character.Name;
            _HPLabel.text = _character.HP.Key.ToString();
            _AttackLabel.text = _character.Attack.Key.ToString();
            _DefenseLabel.text = _character.Defense.Key.ToString();
        }

        private void OnEnable()
        {
            _HPButton.onClick.AddListener(OnHPButtonClicked);
            _AttackButton.onClick.AddListener(OnAttackButtonClicked);
            _DefenseButton.onClick.AddListener(OnDefenseButtonClicked);
        }

        private void OnDisable()
        {
            _HPButton.onClick.RemoveListener(OnHPButtonClicked);
            _AttackButton.onClick.RemoveListener(OnAttackButtonClicked);
            _DefenseButton.onClick.RemoveListener(OnDefenseButtonClicked);
        }
        #endregion Methods

        private void OnHPButtonClicked()
        {
            _selectedStatInfos = _character.HP.Value;
            OpenNextMenu(STAT_INFOS);
        }

        private void OnAttackButtonClicked()
        {
            _selectedStatInfos = _character.Attack.Value;
            OpenNextMenu(STAT_INFOS);
        }

        private void OnDefenseButtonClicked()
        {
            _selectedStatInfos = _character.Defense.Value;
            OpenNextMenu(STAT_INFOS);
        }
    }
}