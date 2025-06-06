using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;
using TMPro;

namespace Space_1
{

    [RequireComponent(typeof(Collider))]
    public class RegistryTimesocre : MonoBehaviour
    {
        [SerializeField] private Leaderboard leaderboardEntry;
        [SerializeField] private TimeAttack timeAttack;
        [SerializeField] private int target_stage = 1;
        [SerializeField] private ParticleSystem _particleSystem;

        private bool is_registered = false;
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
            if (!IsLocalAvatar(other)) return;

            if (is_registered) return;

            

            _particleSystem.Play();

            float time = timeAttack.GetTime();

            if (target_stage == 1 && !is_registered)  
            {
                leaderboardEntry.AddStage1Score(time);
            }
            else if (target_stage == 2 && !is_registered)
            {
                leaderboardEntry.AddStage2Score(time);
            }
            else if (target_stage == 3 && !is_registered)
            {
                leaderboardEntry.AddStage3Score(time);
            }

            is_registered = true;


            //this.gameObject.GetComponent<Collider>().enabled = false;
            
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
