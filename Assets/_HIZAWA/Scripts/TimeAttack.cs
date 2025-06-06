using UnityEngine;
using TMPro;
using SpatialSys.UnitySDK;
using System.Collections;

namespace Space_1
{
    public class TimeAttack : MonoBehaviour
    {
        // 分、秒、ミリ秒それぞれのTextMeshProUGUI
        [SerializeField] private TextMeshProUGUI _minuteText; 
        [SerializeField] private TextMeshProUGUI _secondText;
        [SerializeField] private TextMeshProUGUI _millisecondText;
        
        [SerializeField] private Leaderboard _leaderboard;
        [SerializeField] private Collider _startCollider;
        [SerializeField] private Collider _goalCollider;

        [SerializeField] private Collider[] _restColliders;
        
        private float TimerCount;
        private bool isTimer;
        private bool isInRestArea = false; // 休憩エリア内にいるかどうかのフラグ


        private void Start()
        {
            // 初期表示の更新
            UpdateTimerDisplay();

            // スタート、ゴールのコライダーに TriggerListener を追加し、イベントを設定
            var startTrigger = _startCollider.gameObject.AddComponent<TriggerListener>();
            startTrigger.OnTriggerEnterEvent += StartTimer;
            var goalTrigger = _goalCollider.gameObject.AddComponent<TriggerListener>();
            goalTrigger.OnTriggerEnterEvent += StopTimerAndSubmitScore;

            // 休憩エリアのコライダーに TriggerListener を追加し、イベントを設定
            foreach (var restCollider in _restColliders)
            {
                if (restCollider == null) continue; // Nullチェックを追加
                var restTrigger = restCollider.gameObject.AddComponent<TriggerListener>();
                restTrigger.OnTriggerEnterEvent += EnterRestArea;
                restTrigger.OnTriggerExitEvent += ExitRestArea; // Exitイベントも設定
            }
        }

        private void Update()
        {
            // タイマーが動いていて、かつ休憩エリアにいない場合にカウントアップ
            if (isTimer && !isInRestArea)
            {
                TimerCount += Time.deltaTime;
                UpdateTimerDisplay();
            }
        }

        private void StartTimer(Collider other)
        {
             if (!IsLocalAvatar(other)) return;
            // 必要であればここで other がプレイヤーかどうかのチェックを追加
            Debug.Log("Start");
            UpdateTimerDisplay();

            if (!isTimer)
            {
                 isTimer = true;
                 TimerCount = 0f;
                 isInRestArea = false; // スタート時にリセット
            }            
            
        }

        private void StopTimerAndSubmitScore(Collider other)
        {
            if (!IsLocalAvatar(other)) return;
            // 必要であればここで other がプレイヤーかどうかのチェックを追加
            if (isTimer)
            {
                Debug.Log("Goal");
                isTimer = false;
                //Score = TimerCount; // 最終スコアを記録
                // TimerCount = 0f; // ゴール時にリセットするかどうかは仕様による

                // リーダーボードへスコア送信
                _leaderboard.AddScore(
                    SpatialBridge.actorService.localActor.userID,
                    SpatialBridge.actorService.localActor.username,
                    TimerCount
                );
                Debug.Log($"Score {TimerCount:F3} submitted to leaderboard.");
                
                // スタート地点のオブジェクトの色を緑に変更 (例: 色変更する場合)
                // 必要であれば Renderer の存在チェックなどを追加
                var startRenderer = _startCollider.GetComponent<Renderer>();
                if (startRenderer != null)
                {
                    startRenderer.material.SetColor("_BaseColor", Color.green); // シェーダーに "_BaseColor" がある場合
                }
            }
        }

        // 休憩エリアに入った時の処理
        private void EnterRestArea(Collider other)
        {
            Debug.Log("Enter Rest Area");
            if (!IsLocalAvatar(other)) return;
            isInRestArea = true;
            
        }

        // 休憩エリアから出た時の処理
        private void ExitRestArea(Collider other)
        {   
            Debug.Log("Exit Rest Area");
            if (!IsLocalAvatar(other)) return;
            isInRestArea = false;
            UpdateTimerDisplay();
        }
        
        // タイマーの経過時間(TimerCount)を分、秒、ミリ秒（＝小数第2位：センチ秒）に分解して各TMPに表示
        private void UpdateTimerDisplay()
        {
            int minutes = Mathf.FloorToInt(TimerCount / 60f);
            int seconds = Mathf.FloorToInt(TimerCount % 60f);
            int centiseconds = Mathf.FloorToInt((TimerCount % 1f) * 100f);
            
            _minuteText.text = minutes.ToString("00");
            _secondText.text = seconds.ToString("00");
            _millisecondText.text = centiseconds.ToString("00");
        }
        
        /* コライダーがローカルアバターか判定するヘルパー */
        private bool IsLocalAvatar(Collider other) {
            // Unity エディタ（オフライン）でも動かせるよう null チェック
            if (SpatialBridge.actorService == null || 
                SpatialBridge.actorService.localActor == null)
                return true;   // エディタ実行時は全部 true 扱い

            // localActor.avatar を使用して、その位置情報で判定
            IAvatar localAvatar = SpatialBridge.actorService.localActor.avatar;
            if (localAvatar == null)
                return false;
                
            // コライダーとアバターの位置が近いかをチェック
            // 同じオブジェクトでなくてもコライダーがプレイヤーに関連していれば
            // ある程度近い位置にあるはず
            float distance = Vector3.Distance(other.transform.position, localAvatar.position);
            return distance < 2.0f; // 2メートル以内なら同じアバターのコライダーと判断
        }

        public float GetTime()
        {
            return TimerCount;
        }

        public void PauseTimer()
        {
            if (!isTimer) return;
            isTimer = false;
            Debug.Log("Timer paused for 5 seconds.");
            StartCoroutine(ResumeTimerAfterDelay(5f));
        }

        private IEnumerator ResumeTimerAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            isTimer = true;
            Debug.Log("Timer resumed after " + delay + " seconds.");
        }

        public void ResumeTimer()
        {
            // Implementation of ResumeTimer method
        }
    }

    // コライダーのトリガーイベント通知用のクラス
    public class TriggerListener : MonoBehaviour
    {
        public delegate void TriggerEventHandler(Collider other);
        public event TriggerEventHandler OnTriggerEnterEvent;
        public event TriggerEventHandler OnTriggerExitEvent; // Exitイベント

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterEvent?.Invoke(other);
        }

        private void OnTriggerExit(Collider other) // OnTriggerExitメソッド
        {
            OnTriggerExitEvent?.Invoke(other);
        }
    }


}
