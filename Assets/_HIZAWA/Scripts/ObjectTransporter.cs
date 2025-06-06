using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;

namespace Space_1
{
    [RequireComponent(typeof(BoxCollider))]
    public class ObjectTransporter : MonoBehaviour
    {
        [Header("移動設定")]
        // StartPositionはGameObjectの初期位置を使用
        private Vector3 startPosition;

        [Tooltip("終了位置")]
        public GameObject endPosition;

        [Tooltip("移動時間（秒）")]
        public float duration = 5.0f;

        [Header("内部変数")]
        private bool isPlayerOnPlatform = false;
        private bool isMoving = false;
        private bool isReturning = false;
        private float playerExitTime = 0f;
        private Vector3 originalPosition;
        private Coroutine moveCoroutine;
        
        // Spatial.io用プレイヤー検出半径
        [Tooltip("プレイヤー検出範囲（半径）")]
        public float detectionRadius = 1.5f;
        
        // 検出対象レイヤー（プレイヤーが所属するレイヤー - AvatarLocal:30）
        [Tooltip("プレイヤーが所属するレイヤー")]
        public LayerMask playerLayer = 1 << 30; // AvatarLocalはレイヤー30

        // プレイヤーとプラットフォームの相対位置
        private Vector3 initialPositionOffset;
        
        // プレイヤーオブジェクトの参照
        private GameObject playerObject = null;
        
        // 前回検出したコライダー
        private Collider detectedCollider = null;
        
        // BoxColliderの参照
        private BoxCollider boxCollider;
        
        // ローカルアバターの参照（検出用）
        private IAvatar localAvatar = null;
        
        // デバッグ用
        [Header("デバッグ")]
        [SerializeField] private bool showDebugLogs = true;
        private float debugTimer = 0f;
        private const float DEBUG_INTERVAL = 1.0f; // 1秒ごとにデバッグ情報を表示

        void Start()
        {
            if (endPosition == null)
            {
                Debug.LogError("終了位置が設定されていません。ObjectTransporterが正しく機能しません。");
                enabled = false;
                return;
            }

            // 初期位置をGameObjectの設置位置として保存
            startPosition = transform.position;
            originalPosition = startPosition;
            
            // BoxColliderの参照を取得
            boxCollider = GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                Debug.LogError("BoxColliderが見つかりません。このコンポーネントにはBoxColliderが必要です。");
                enabled = false;
                return;
            }
            
            // BoxColliderがトリガーかどうか確認(トリガーでなければ警告)
            if (!boxCollider.isTrigger)
            {
                ////Debug.LogWarning("BoxColliderのisTriggerがfalseです。トリガーとして機能させるにはtrueに設定してください。");
                boxCollider.isTrigger = true;
            }
            
            //DebugLog("ObjectTransporterが初期化されました。起点: " + startPosition + " 終点: " + endPosition.transform.position);
        }

        void Update()
        {
            // ローカルアバターの参照を取得
            UpdateLocalAvatarReference();
            
            // プレイヤーの位置を直接チェック
            CheckPlayerPosition();
            
            // プレイヤーが離れた後の動作
            if (!isPlayerOnPlatform && !isReturning && isMoving)
            {
                playerExitTime += Time.deltaTime;
                
                // プレイヤーが離れてから2秒経過したら元の位置に戻る
                if (playerExitTime >= 2.0f)
                {
                    isReturning = true;
                    if (moveCoroutine != null)
                    {
                        StopCoroutine(moveCoroutine);
                    }
                    moveCoroutine = StartCoroutine(MoveToPosition(startPosition, duration));
                    DebugLog("プレイヤーが離れたため、元の位置に戻ります");
                }
            }
            
            // デバッグ情報の表示
            if (showDebugLogs)
            {
                debugTimer += Time.deltaTime;
                if (debugTimer >= DEBUG_INTERVAL)
                {
                    debugTimer = 0f;
                    if (localAvatar != null)
                    {
                        DebugLog("ローカルアバター位置: " + localAvatar.position + " プラットフォーム位置: " + transform.position);
                        DebugLog("プレイヤー乗車状態: " + isPlayerOnPlatform + " 移動中: " + isMoving + " 戻り中: " + isReturning);
                    }
                }
            }
        }
        
        // ローカルアバターの参照を更新
        private void UpdateLocalAvatarReference()
        {
            if (localAvatar == null && SpatialBridge.actorService != null && 
                SpatialBridge.actorService.localActor != null && 
                SpatialBridge.actorService.localActor.avatar != null)
            {
                localAvatar = SpatialBridge.actorService.localActor.avatar;
                DebugLog("ローカルアバターを参照しました");
            }
        }
        
        // プレイヤーの位置を直接チェック
        private void CheckPlayerPosition()
        {
            if (localAvatar == null) return;
            
            // ローカルアバターの位置がBoxCollider内にあるかチェック
            bool isInside = IsPointInsideBoxCollider(localAvatar.position);
            
            // プレイヤーが新たにBoxCollider内に入った場合
            if (!isPlayerOnPlatform && isInside)
            {
                OnPlayerEnter();
            }
            // プレイヤーがBoxCollider外に出た場合
            else if (isPlayerOnPlatform && !isInside)
            {
                OnPlayerExit();
            }
        }
        
        // ポイントがBoxCollider内にあるかチェック
        private bool IsPointInsideBoxCollider(Vector3 point)
        {
            // ワールド座標からローカル座標へ変換
            Vector3 localPoint = transform.InverseTransformPoint(point);
            
            // BoxColliderのローカル空間でのサイズと中心位置
            Vector3 halfSize = boxCollider.size / 2f;
            Vector3 center = boxCollider.center;
            
            // 点がBoxの中にあるかチェック
            return (localPoint.x >= center.x - halfSize.x && localPoint.x <= center.x + halfSize.x &&
                    localPoint.y >= center.y - halfSize.y && localPoint.y <= center.y + halfSize.y &&
                    localPoint.z >= center.z - halfSize.z && localPoint.z <= center.z + halfSize.z);
        }
        
        // プレイヤーが入った時の処理
        private void OnPlayerEnter()
        {
            if (isMoving || isReturning)
            {
                DebugLog("移動中または戻り中のため、新たな移動を開始しません。");
                return;
            }

            DebugLog("プレイヤーが乗りました");

            // プレイヤーとプラットフォームの相対位置を記録
            initialPositionOffset = localAvatar.position - transform.position;
            
            isPlayerOnPlatform = true;
            playerExitTime = 0f;
            isReturning = false;

            // 既に動いていたら一度停止する
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            // 目的地へ移動開始
            moveCoroutine = StartCoroutine(MoveToPosition(endPosition.transform.position, duration));
            isMoving = true;
        }
        
        // プレイヤーが出た時の処理
        private void OnPlayerExit()
        {
            DebugLog("プレイヤーが降りました");
            isPlayerOnPlatform = false;
            playerExitTime = 0f;
        }
        
        // トリガー判定用のイベント（物理的なコライダー検出のバックアップとして）
        private void OnTriggerEnter(Collider other)
        {
            DebugLog("トリガーに入りました: " + other.gameObject.name);
            
            // Spatial.ioのローカルアバターかどうかを確認
            if (IsLocalAvatar(other))
            {
                DebugLog("ローカルアバターがトリガーに入りました");
                OnPlayerEnter();
            }
        }
        
        // トリガーから出た時のイベント
        private void OnTriggerExit(Collider other)
        {
            DebugLog("トリガーから出ました: " + other.gameObject.name);
            
            // Spatial.ioのローカルアバターかどうかを確認
            if (IsLocalAvatar(other) && isPlayerOnPlatform)
            {
                DebugLog("ローカルアバターがトリガーから出ました");
                OnPlayerExit();
            }
        }
        
        // 指定位置へ滑らかに移動するコルーチン
        private IEnumerator MoveToPosition(Vector3 targetPosition, float moveDuration)
        {
            Vector3 startPos = transform.position;
            float elapsedTime = 0f;
            
            DebugLog("移動開始: " + startPos + " → " + targetPosition);

            while (elapsedTime < moveDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / moveDuration);
                transform.position = Vector3.Lerp(startPos, targetPosition, t);
                
                yield return null;
            }

            transform.position = targetPosition;
            DebugLog("移動完了: " + targetPosition);

            // 戻る動作の場合、状態をリセット
            if (isReturning)
            {
                isMoving = false;
                isReturning = false;
                DebugLog("元の位置に戻りました");
            }
        }
        
        // ギズモの描画（BoxColliderの検出範囲の可視化）
        private void OnDrawGizmosSelected()
        {
            // 初期化前の場合は取得
            if (boxCollider == null)
            {
                boxCollider = GetComponent<BoxCollider>();
                if (boxCollider == null) return;
            }
            
            Gizmos.color = Color.yellow;
            // BoxColliderの範囲を表示
            Gizmos.matrix = Matrix4x4.TRS(
                transform.position + boxCollider.center,
                transform.rotation,
                transform.lossyScale
            );
            Gizmos.DrawWireCube(Vector3.zero, boxCollider.size);
        }
        
        // ローカルアバター判定ヘルパー
        private bool IsLocalAvatar(Collider other)
        {
            // Unity エディタ（オフライン）でも動かせるよう null チェック
            if (SpatialBridge.actorService == null || 
                SpatialBridge.actorService.localActor == null)
            {
                DebugLog("エディタ実行時は全部 true 扱い");
                return true;   // エディタ実行時は全部 true 扱い
            }

            // ローカルアバターの参照を更新
            UpdateLocalAvatarReference();
            
            if (localAvatar == null)
            {
                DebugLog("ローカルアバターが見つかりません");
                return false;
            }
                
            // コライダーとアバターの位置が近いかをチェック
            float distance = Vector3.Distance(other.transform.position, localAvatar.position);
            DebugLog("コライダーとアバターの距離: " + distance);
            return distance < 2.0f; // 2メートル以内なら同じアバターのコライダーと判断
        }
        
        // デバッグログを表示
        private void DebugLog(string message)
        {
            if (showDebugLogs)
            {
                //Debug.Log("[ObjectTransporter] " + message);
            }
        }
    }
}
