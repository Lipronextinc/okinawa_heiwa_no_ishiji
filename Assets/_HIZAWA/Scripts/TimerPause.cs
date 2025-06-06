using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;

namespace Space_1
{
    public class TimerPause : MonoBehaviour
    {

        [SerializeField] private TimeAttack _timeAttack;
        [SerializeField] private Collider _collider;
        [SerializeField] private GameObject _item;
        [SerializeField] private ParticleSystem _particle;

        [SerializeField] private SpatialSFX timerStopSfx;  
        [Range(0f, 1.5f)] [SerializeField] private float volume = 1f;
        [Range(0.1f, 2f)] [SerializeField] private float pitch  = 1f;
        
        // Start is called before the first frame update
        [SerializeField] private float _delay = 1.0f;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsLocalAvatar(other))
            {
                /* 効果音再生：Spatial公式 AudioService */
                if (timerStopSfx != null && SpatialBridge.audioService != null)
                {
                    SpatialBridge.audioService
                                .PlaySFX(timerStopSfx, transform.position, volume, pitch); // :contentReference[oaicite:0]{index=0}
                }
                _particle.Play();
                _collider.enabled = false;
                _timeAttack.PauseTimer();
            }
        }

        // ローカルアバター判定ヘルパー
        private bool IsLocalAvatar(Collider other)
        {
            // Unity エディタ（オフライン）でも動かせるよう null チェック
            if (SpatialBridge.actorService == null || 
                SpatialBridge.actorService.localActor == null)
            {
                Debug.Log("エディタ実行時は全部 true 扱い");
                return true;   // エディタ実行時は全部 true 扱い
            }

            IAvatar avatar = SpatialBridge.actorService.localActor.avatar;
            if (avatar == null)
            {
                Debug.Log("ローカルアバターが見つかりません");
                return false;
            }
                
            // コライダーとアバターの位置が近いかをチェック
            float distance = Vector3.Distance(other.transform.position, avatar.position);
            Debug.Log("コライダーとアバターの距離: " + distance);
            return distance < 2.0f; // 2メートル以内なら同じアバターのコライダーと判断
        }
    }
}
