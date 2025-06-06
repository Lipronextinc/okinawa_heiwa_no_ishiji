using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSys.UnitySDK;
using Unity.VisualScripting;
using UnityEngine;

public class LeaderboardEntry
{
    public string UserID { get; private set; }
    public string Username { get; private set; } // DisplayName → Username に変更
    public float Stage1GoalTime { get; set; }
    public float Stage2GoalTime { get; set; }
    public float Stage3GoalTime { get; set; }

    private IActor _actor;

    public void Initialize(string userID, string username, float score)
    {
        UserID = userID;
        _actor = SpatialBridge.actorService.actors.Values.FirstOrDefault(actor => actor.userID == userID);
        Username = _actor != null ? _actor.username : username;
        Stage1GoalTime = score;
        Stage2GoalTime = 0f;
        Stage3GoalTime = 0f;
    }

    public void Initialize(string userID, string username, float stage1GoalTime, float stage2GoalTime, float stage3GoalTime)
    {
        UserID = userID;
        _actor = SpatialBridge.actorService.actors.Values.FirstOrDefault(actor => actor.userID == userID);
        Username = _actor != null ? _actor.username : username;
        Stage1GoalTime = stage1GoalTime;
        Stage2GoalTime = stage2GoalTime;
        Stage3GoalTime = stage3GoalTime;
    }

    /// <summary>
    /// Only available if user is in the space right now
    /// </summary>
    public ActorProfilePictureRequest GetProfilePicture()
    {
        return _actor?.GetProfilePicture();
    }
}

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private GameObject scoreCellContainer;
    [SerializeField] private GameObject scoreCellPrefab;
    public List<LeaderboardEntry> LeaderboardEntries { get; private set; } = new List<LeaderboardEntry>();

    [SerializeField] private string leaderboardVariableName = "leaderboard";
    [SerializeField] private int _maxLeaderboardEntries = 10;
    [SerializeField] private SpatialSyncedObject _syncedObject;
    private VariableDeclarations _objectVars;
    private bool _dirty = false;

    public Action OnLeaderboardChanged;

    private void Awake()
    {
        _objectVars = ObjectVariables.Declarations(gameObject, false, true);

        _syncedObject.onVariableChanged += HandleVariableChanged;

        LoadLeaderboard();
    }

    private void OnDestroy()
    {
        if (_syncedObject != null)
        {
            _syncedObject.onVariableChanged -= HandleVariableChanged;
        }

        _objectVars = null;
        LeaderboardEntries = null;
    }

    private void HandleVariableChanged(string variableName, object newValue)
    {
        if (variableName == leaderboardVariableName)
        {
            DeserializeLeaderboard((string)newValue);
            OnLeaderboardChanged?.Invoke();
        }
    }

    private void LoadLeaderboard()
    {
        string leaderboardString = (string)_objectVars.Get(leaderboardVariableName);

        DeserializeLeaderboard(leaderboardString);
    }

    private void SaveLeaderboard()
    {
        if (!_syncedObject.isLocallyOwned)
            _syncedObject.TakeoverOwnership();

        _objectVars.Set(leaderboardVariableName, SerializeLeaderboard());
    }

    private void DeserializeLeaderboard(string leaderboardString)
    {
        LeaderboardEntries.Clear();

        if (string.IsNullOrEmpty(leaderboardString))
        {
            return;
        }

        string[] leaderboardEntries = leaderboardString.Split('\n');

        foreach (string leaderboardEntry in leaderboardEntries)
        {
            string[] entryParts = leaderboardEntry.Split('\t');

            if (entryParts.Length != 5)
            {
                continue;
            }

            string userID = entryParts[0];
            string username = entryParts[1]; // displayName → username に変更
            if (!float.TryParse(entryParts[2], out float stage1GoalTime)) { continue; }
            if (!float.TryParse(entryParts[3], out float stage2GoalTime)) { continue; }
            if (!float.TryParse(entryParts[4], out float stage3GoalTime)) { continue; }

            LeaderboardEntry newEntry = new LeaderboardEntry();
            newEntry.Initialize(userID, username, stage1GoalTime, stage2GoalTime, stage3GoalTime);

            LeaderboardEntries.Add(newEntry);
        }
        SortLeaderboard();
    }

    private string SerializeLeaderboard()
    {
        string leaderboardString = "";

        foreach (var entry in LeaderboardEntries)
        {
            leaderboardString += $"{entry.UserID}\t{entry.Username}\t{entry.Stage1GoalTime:F3}\t{entry.Stage2GoalTime:F3}\t{entry.Stage3GoalTime:F3}\n";
        }

        return leaderboardString;
    }

    public void AddScore(float score)
    {
        AddScore(SpatialBridge.actorService.localActor.userID, SpatialBridge.actorService.localActor.username, score);
    }

    public void AddScore(string userID, string username, float score)
    {
        LeaderboardEntry existingEntry = LeaderboardEntries.Find(e => e.UserID == userID);
        if (existingEntry == null)
        {
            LeaderboardEntry newEntry = new LeaderboardEntry();
            newEntry.Initialize(userID, username, score);
            LeaderboardEntries.Add(newEntry);
            Debug.Log($"[ Leaderboard ] Added new entry for user {userID} with stage1 score {score:F3}");
        }
        else
        {
            if (score > 0 && (existingEntry.Stage1GoalTime == 0 || score < existingEntry.Stage1GoalTime))
            {
                Debug.Log($"[ Leaderboard ] Updating user {userID} stage1 score from {existingEntry.Stage1GoalTime:F3} to {score:F3}");
                existingEntry.Stage1GoalTime = score;
            }
            else
            {
                Debug.Log($"[ Leaderboard ] No update for user {userID} stage1 score: existing score {existingEntry.Stage1GoalTime:F3}, new score {score:F3}");
            }
        }

        SortLeaderboard();
        _dirty = true;
        OnLeaderboardChanged?.Invoke();
    }

    public void AddScore(string username, float stage1GoalTime, float stage2GoalTime, float stage3GoalTime)
    {
        string userID = SpatialBridge.actorService.localActor.userID;
        LeaderboardEntry entry = LeaderboardEntries.Find(e => e.UserID == userID);
        if (entry == null)
        {
            entry = new LeaderboardEntry();
            entry.Initialize(userID, username, stage1GoalTime, stage2GoalTime, stage3GoalTime);
            LeaderboardEntries.Add(entry);
            Debug.Log($"[ Leaderboard ] Added new entry for user {userID} with scores: stage1 {stage1GoalTime:F3}, stage2 {stage2GoalTime:F3}, stage3 {stage3GoalTime:F3}");
        }
        else
        {
            if (stage1GoalTime > 0 && (entry.Stage1GoalTime == 0 || stage1GoalTime < entry.Stage1GoalTime))
            {
                Debug.Log($"[ Leaderboard ] Updating user {userID} stage1 score from {entry.Stage1GoalTime:F3} to {stage1GoalTime:F3}");
                entry.Stage1GoalTime = stage1GoalTime;
            }
            if (stage2GoalTime > 0 && (entry.Stage2GoalTime == 0 || stage2GoalTime < entry.Stage2GoalTime))
            {
                Debug.Log($"[ Leaderboard ] Updating user {userID} stage2 score from {entry.Stage2GoalTime:F3} to {stage2GoalTime:F3}");
                entry.Stage2GoalTime = stage2GoalTime;
            }
            if (stage3GoalTime > 0 && (entry.Stage3GoalTime == 0 || stage3GoalTime < entry.Stage3GoalTime))
            {
                Debug.Log($"[ Leaderboard ] Updating user {userID} stage3 score from {entry.Stage3GoalTime:F3} to {stage3GoalTime:F3}");
                entry.Stage3GoalTime = stage3GoalTime;
            }
        }

        SortLeaderboard();
        _dirty = true;
        OnLeaderboardChanged?.Invoke();
    }

    public void AddStage1Score(float stage1GoalTime)
    {
        AddScore(SpatialBridge.actorService.localActor.displayName, stage1GoalTime, 0f, 0f);
    }

    public void AddStage2Score(float stage2GoalTime)
    {
        AddScore(SpatialBridge.actorService.localActor.displayName, 0f, stage2GoalTime, 0f);
    }

    public void AddStage3Score(float stage3GoalTime)
    {
        AddScore(SpatialBridge.actorService.localActor.displayName, 0f, 0f, stage3GoalTime);
    }

    private void SortLeaderboard()
    {
        LeaderboardEntries.Sort(new LeaderboardEntryComparer());

        if (LeaderboardEntries.Count > _maxLeaderboardEntries)
        {
            LeaderboardEntries = LeaderboardEntries.Take(_maxLeaderboardEntries).ToList();
        }
    }

    public int GetPlace()
    {
        return GetPlace(SpatialBridge.actorService.localActor.userID);
    }

    public int GetPlace(string userID)
    {
        return LeaderboardEntries.ToList().FindIndex(entry => entry.UserID == userID);
    }

    private void LateUpdate()
    {
        if (_dirty)
        {
            SaveLeaderboard();
            _dirty = false;
        }
    }

    public void ResetScores()
    {
        Debug.Log("[ Leaderboard ] Resetting leaderboard scores...");
        LeaderboardEntries.Clear();
        SaveLeaderboard();
        OnLeaderboardChanged?.Invoke();
    }


    public class LeaderboardEntryComparer : IComparer<LeaderboardEntry>
    {
        public int Compare(LeaderboardEntry one, LeaderboardEntry two)
        {
            float scoreOne = one.Stage3GoalTime == 0 ? float.MaxValue : one.Stage3GoalTime;
            float scoreTwo = two.Stage3GoalTime == 0 ? float.MaxValue : two.Stage3GoalTime;
            int comp = scoreOne.CompareTo(scoreTwo);
            if (comp != 0)
                return comp;
            return one.UserID.CompareTo(two.UserID);
        }
    }
}
