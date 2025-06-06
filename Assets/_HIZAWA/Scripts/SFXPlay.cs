using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;

namespace Space_1
{
    [RequireComponent(typeof(Collider))]
    public class SFXPlay : MonoBehaviour
    {
        [SerializeField] private SpatialSFX sfx;
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

        private void OnTriggerEnter(Collider other)
        {
            if (IsLocalAvatar(other))
            {
                if (sfx != null && SpatialBridge.audioService != null)
                {
                    SpatialBridge.audioService
                                .PlaySFX(sfx, transform.position, volume, pitch); // :contentReference[oaicite:0]{index=0}
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
