using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;

namespace Space_1
{
    public class PrazmaTrigger : MonoBehaviour
    {

        [SerializeField] private GameObject prazmaObject;

        [SerializeField] private ParticleSystem prazma;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        void OnTriggerEnter(Collider other)
        {
            if (IsLocalAvatar(other))
            {
                if (!prazmaObject.activeSelf){
                    prazmaObject.SetActive(true);

                    if (!prazma.isPlaying){
                        prazma.Play();
                    }
                }
            }
        }
        

        void OnTriggerExit(Collider other)
        {
            if (IsLocalAvatar(other))
            {
                if (prazma.isPlaying){
                    prazma.Stop();
                }

                if (prazmaObject.activeSelf){
                    prazmaObject.SetActive(false);                    
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
