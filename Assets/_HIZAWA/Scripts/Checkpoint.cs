using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;
using SpatialSys.UnitySDK.Internal;      // 内部APIアクセス用


namespace Space_1
{
    [RequireComponent(typeof(Collider))]
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private SpatialSFX sfx;
        [Range(0f, 1.5f)] [SerializeField] private float volume = 1f;
        [Range(0.1f, 2f)] [SerializeField] private float pitch  = 1f;
        
        [SerializeField]　GameObject target_obj;

         [SerializeField]　GameObject savepoint_obj;

         private bool is_registered = false;

        void Start(){}

        // Update is called once per frame
        void Update(){}

        private void OnTriggerEnter(Collider other)
        {
            if (!IsLocalAvatar(other)) return;

            if(is_registered) return;

            savepoint_obj.SetActive(true);
            target_obj.transform.position = transform.position;
            is_registered = true;

            if (sfx != null && SpatialBridge.audioService != null)
                {
                    SpatialBridge.audioService
                                .PlaySFX(sfx, transform.position, volume, pitch); // :contentReference[oaicite:0]{index=0}
                }
            //this.gameObject.SetActive(false);
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
