using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;   // IAvatar / SpatialBridge
using Unity.VisualScripting;

namespace Space_1
{
    [RequireComponent(typeof(BoxCollider))]
    public class SuperJump : MonoBehaviour
    {

        [Header("Jump Settings")]
        [Tooltip("上向きに加える力 (N ≒ m/s²)。8〜15 で『大ジャンプ』っぽくなります")]
        [SerializeField] private float jumpForce = 12f;

        [Tooltip("ローカルプレイヤーだけに作用するか")]
        [SerializeField] private bool localPlayerOnly = true;

        [Header("Sound Settings")]
        [Tooltip("再生したい SpatialSFX アセット")]
        [SerializeField] private SpatialSFX jumpSfx;           // ★ AudioClip ではなく SpatialSFX!
        [Range(0f, 1.5f)] [SerializeField] private float volume = 1f;
        [Range(0.1f, 2f)] [SerializeField] private float pitch  = 1f;


        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /* -------------------- Trigger -------------------- */
        private void OnTriggerEnter(Collider other)
        {
            if (localPlayerOnly && !IsLocalAvatar(other))
                return;

            // アバターを取得
            IAvatar avatarToJump = null;
            
            // ローカルプレイヤーのアバターを取得
            if (SpatialBridge.actorService != null && SpatialBridge.actorService.localActor != null)
            {
                avatarToJump = SpatialBridge.actorService.localActor.avatar;
            }

            if (avatarToJump != null)
            {
                // 垂直方向に力を加える
                avatarToJump.AddForce(transform.up * jumpForce);
                
                /* 効果音再生：Spatial公式 AudioService */
                if (jumpSfx != null && SpatialBridge.audioService != null)
                {
                    SpatialBridge.audioService
                                .PlaySFX(jumpSfx, transform.position, volume, pitch); // :contentReference[oaicite:0]{index=0}
                }
            }
        }

        /* ------------ ローカルアバター判定ヘルパー ------------ */
        private static bool IsLocalAvatar(Collider col)
        {
            var actorSvc = SpatialBridge.actorService;
            if (actorSvc == null || actorSvc.localActor == null)
                return true;                          // エディタ実行時は全部 OK

            // コライダーとローカルアバターの位置で判定
            if (actorSvc.localActor.avatar != null)
            {
                float distance = Vector3.Distance(col.transform.position, actorSvc.localActor.avatar.position);
                return distance < 2.0f; // 2メートル以内なら同じアバターのコライダーと判断
            }
            
            return false;
        }
    }
}
