using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpatialSys.UnitySDK;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LeaderboardView : MonoBehaviour
{
    [SerializeField] private Leaderboard _leaderboard;
    [SerializeField] private TMP_Text _leaderboardText;
    [SerializeField] private Button _addButton;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _autoFillButton;
    [SerializeField] private int _viewEntries = 10;
    [SerializeField] private GameObject scoreCellPrefab;
    [SerializeField] private Transform scoreCellContainer;

    private void Start()
    {
        _addButton.onClick.AddListener(AddScore);
        _resetButton.onClick.AddListener(ResetLeaderboard);
        //_autoFillButton.onClick.AddListener(Generate1000Entries);
        _leaderboard.OnLeaderboardChanged += UpdateLeaderboardText;
        UpdateLeaderboardText();
    }

    private void AddScore()
    {
        _leaderboard.AddScore(UnityEngine.Random.Range(0, 100));
    }

    private void ResetLeaderboard()
    {
        _leaderboard.ResetScores();
    }

    private void UpdateLeaderboardText()
    {
        // Clear existing ScoreCells in the container
        foreach (Transform child in scoreCellContainer)
        {
            Destroy(child.gameObject);
        }
        
        List<LeaderboardEntry> entries = _leaderboard.LeaderboardEntries.Take(_viewEntries).ToList();
        for (int i = 0; i < entries.Count; i++)
        {
            LeaderboardEntry entry = entries[i];
            GameObject scoreCell = Instantiate(scoreCellPrefab, scoreCellContainer);
            // Use the overloaded SetData to also pass the rank (i+1)
            scoreCell.GetComponent<Space_1.ScoreCell>().SetData(entry, i + 1);
        }
    }

    // private void Generate1000Entries()
    // {
    //     _leaderboard.ResetScores();
    //     for (int i = 0; i < 1000; i++)
    //     {
    //         int index = Random.Range(0, 99999);
    //         _leaderboard.AddScore(index.ToString(), $"Test User {index}", 0);
    //     }
    // }
}