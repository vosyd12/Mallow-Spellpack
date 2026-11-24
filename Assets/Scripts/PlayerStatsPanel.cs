using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsPanel : MonoBehaviour
{
    [Serializable]
    private class StatRow
    {
        public PlayerStat Stat;
        public TMP_Text ValueText;
        public Button UpgradeButton;
    }

    [SerializeField] private Player _player;
    [SerializeField] private TMP_Text _goldText;
    [SerializeField] private StatRow[] _rows;

    private void OnEnable()
    {
        _player.ProgressionChanged += Refresh;
        _player.Damaged += Refresh;
        _player.Died += Refresh;
        _player.Revived += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        _player.ProgressionChanged -= Refresh;
        _player.Damaged -= Refresh;
        _player.Died -= Refresh;
        _player.Revived -= Refresh;
    }

    // 버튼이 전달하는 번호는 Inspector의 Rows 배열 순서다.
    public void UpgradeRow(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= _rows.Length)
        {
            return;
        }

        _player.TryUpgradeStat(_rows[rowIndex].Stat);
    }

    private void Refresh()
    {
        PlayerState state = _player.PlayerState;
        _goldText.text = $"{state.Gold} G";

        foreach (StatRow row in _rows)
        {
            int cost = _player.GetStatUpgradeCost(row.Stat);
            string costText = cost > 0 ? $"{cost} G" : "-";

            row.ValueText.text = $"{GetValueText(row.Stat, state)}  |  {costText}";
            row.UpgradeButton.interactable = _player.CanUpgradeStat(row.Stat);
        }
    }

    private string GetValueText(PlayerStat stat, PlayerState state)
    {
        return stat switch
        {
            PlayerStat.MaxHP => $"{state.HP} / {_player.MaxHP}",
            PlayerStat.MaxMP => $"{state.MP} / {_player.MaxMP}",
            PlayerStat.STR => state.STR.ToString(),
            PlayerStat.DEX => state.DEX.ToString(),
            PlayerStat.CON => state.CON.ToString(),
            PlayerStat.INT => state.INT.ToString(),
            PlayerStat.WIS => state.WIS.ToString(),
            PlayerStat.LUK => state.LUK.ToString(),
            PlayerStat.AFF => state.AFF.ToString(),
            _ => "-"
        };
    }
}