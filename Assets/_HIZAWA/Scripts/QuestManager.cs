using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;


namespace Space_1
{
    public class QuestManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ResetQuestData(Collider other)
        {
            if (SpatialBridge.userWorldDataStoreService == null)
            {
                Debug.LogWarning("User World Data Store Service is not initialized.");
                return;
            }

            if(!IsLocalAvatar(other))
            {
                return; // ローカルアバター以外のコライダーは無視
            }
            
            SpatialBridge.userWorldDataStoreService.ClearAllVariables();
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
