using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;

namespace Space_1
{
    [RequireComponent(typeof(BoxCollider))]
    public class BrokenItem : MonoBehaviour
    {
        [Header("崩壊設定")]
        [Tooltip("プレイヤーが乗ってから崩れるまでの時間（秒）")]
        public float delayToBreak = 0.1f;
        
        [Tooltip("崩れてから復活するまでの時間（秒）")]
        public float delayToRespawn = 15.0f;
        
        [Header("コンポーネント")]
        [Tooltip("アニメーターコントローラー（オプション）")]
        public Animator animator;
        
        [Tooltip("床として機能する子オブジェクト（デフォルトは最初の子オブジェクト）")]
        public Transform floorObject;
        
        private BoxCollider triggerCollider; // 親オブジェクトのトリガーコライダー
        private BoxCollider floorCollider;   // 子オブジェクトの物理コライダー
        private MeshRenderer floorRenderer;  // 子オブジェクトのレンダラー
        private bool isBreaking = false;
        private bool isBroken = false;
        
        // Start is called before the first frame update
        void Start()
        {
            // 親オブジェクトのトリガーコライダーを取得
            triggerCollider = GetComponent<BoxCollider>();
            if (!triggerCollider.isTrigger)
            {
                Debug.LogWarning("親オブジェクトのBoxColliderのisTriggerをtrueに設定してください");
                triggerCollider.isTrigger = true;
            }
            
            // 子オブジェクトの設定
            if (floorObject == null && transform.childCount > 0)
            {
                // 子オブジェクトが指定されていない場合、最初の子を使用
                floorObject = transform.GetChild(0);
            }
            
            if (floorObject != null)
            {
                floorCollider = floorObject.GetComponent<BoxCollider>();
                floorRenderer = floorObject.GetComponent<MeshRenderer>();
                
                if (floorCollider != null && floorCollider.isTrigger)
                {
                    Debug.LogWarning("子オブジェクトのBoxColliderのisTriggerをfalseに設定してください");
                    floorCollider.isTrigger = false;
                }
            }
            else
            {
                Debug.LogError("床として機能する子オブジェクトが見つかりません");
            }
            
            // アニメーターの設定
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!IsLocalAvatar(other)) return;
            // プレイヤーが乗った判定（トリガー用）
            //if (!isBreaking && !isBroken && other.CompareTag("Player"))
            //{
               // StartCoroutine(BreakItem());
            //}

            // アニメーターがある場合は「Fall」トリガーを実行
            if (animator != null)
            {
                animator.SetTrigger("Fall");
                
                // アニメーション実行時間の待機を追加（オプション）
                // この値は実際のアニメーション長に合わせて調整するか、
                // AnimationEvent などで制御するとベター
                //yield return new WaitForSeconds(1.0f);
            }
            
        }
        
        IEnumerator BreakItem()
        {
            if (isBreaking || isBroken) yield break;
            
            isBreaking = true;
            
            // プレイヤーが乗ってから崩れるまで待機
            yield return new WaitForSeconds(delayToBreak);
            
            // // アニメーターがある場合は「Fall」トリガーを実行
            // if (animator != null)
            // {
            //     animator.SetTrigger("Fall");
                
            //     // アニメーション実行時間の待機を追加（オプション）
            //     // この値は実際のアニメーション長に合わせて調整するか、
            //     // AnimationEvent などで制御するとベター
            //     yield return new WaitForSeconds(1.0f);
            // }
            
            // 子オブジェクトを非表示化
            if (floorRenderer != null)
            {
                floorRenderer.enabled = false;
            }
            
            // 子オブジェクトのコライダーを無効化
            if (floorCollider != null)
            {
                floorCollider.enabled = false;
            }
            
            isBroken = true;
            isBreaking = false;
            
            // 復活までの時間
            yield return new WaitForSeconds(delayToRespawn);
            
            // 復活
            RespawnItem();
        }
        
        void RespawnItem()
        {
            // 子オブジェクトを再表示
            if (floorRenderer != null)
            {
                floorRenderer.enabled = true;
            }
            
            // 子オブジェクトのコライダーを有効化
            if (floorCollider != null)
            {
                floorCollider.enabled = true;
            }
            
            isBroken = false;
        }

        public void OnFall()
        {
            Debug.Log("OnFall");
            StartCoroutine(BreakItem());
        }

        // Update is called once per frame
        void Update()
        {
        
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
    }
}
