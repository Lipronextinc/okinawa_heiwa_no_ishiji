using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpatialSys.UnitySDK;

namespace Space_1
{
    public class ScoreCell : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _rankText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _1stTimeText;
        [SerializeField] private TextMeshProUGUI _2ndTimeText;
        [SerializeField] private TextMeshProUGUI _GoalTimeText;
       
        public List<LeaderboardEntry> LeaderboardEntries { get; private set; } = new List<LeaderboardEntry>();

        public void SetData(LeaderboardEntry entry)
        {
            _rankText.text = "1";//entry.Rank.ToString();
            _nameText.text = entry.Username;
            _1stTimeText.text = TimeSpan.FromSeconds(entry.Stage1GoalTime).ToString(@"mm\:ss\:ff");
            _2ndTimeText.text = TimeSpan.FromSeconds(entry.Stage2GoalTime).ToString(@"mm\:ss\:ff");
            _GoalTimeText.text = TimeSpan.FromSeconds(entry.Stage3GoalTime).ToString(@"mm\:ss\:ff");
        }
    
        public void SetData(LeaderboardEntry entry, int rank)
        {
            _rankText.text = rank.ToString();
            _nameText.text = entry.Username;
            _1stTimeText.text = TimeSpan.FromSeconds(entry.Stage1GoalTime).ToString(@"mm\:ss\:ff");
            _2ndTimeText.text = TimeSpan.FromSeconds(entry.Stage2GoalTime).ToString(@"mm\:ss\:ff");
            _GoalTimeText.text = TimeSpan.FromSeconds(entry.Stage3GoalTime).ToString(@"mm\:ss\:ff");
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
