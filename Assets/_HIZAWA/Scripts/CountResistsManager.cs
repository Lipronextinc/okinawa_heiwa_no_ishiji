using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SpatialSys.UnitySDK;

namespace Space_1
{
    public class CountResistsManager : MonoBehaviour
    {
        [SerializeField] private int currentResistCount = 0;
        [SerializeField] private int maxResistCount = 30;
        [SerializeField] private TextMeshProUGUI currentResistCountText;
        [SerializeField] private TextMeshProUGUI maxResistCountText;
        public int ResistCounter = 0;
        private Coroutine explanationCoroutine = null;
        private string language;

        public GameObject skillupItemGroup;

        public Transform stage2_Transform;
        public Transform stage3_Transform;

        public int stage2_ResistCount = 15;
        public int stage3_ResistCount = 25;




        // ローカルアバターの参照（検出用）
        private IAvatar localAvatar = null;

        private void Start()
        {
            maxResistCountText.text = " / " + maxResistCount.ToString();
        }

        void Update()
        {
            // ローカルアバターの参照を取得
            UpdateLocalAvatarReference();

            CheckResistCount();
            
            // プレイヤーの位置を直接チェック
            //CheckPlayerPosition();
        }

        public void GetNewResist()
        {
            
                currentResistCount++;
                currentResistCountText.text = currentResistCount.ToString();

                // TODO: コインを取得したら、コインを減らす
            
        }


        public void CompletedTutorialGetNewResist()
        {
            
                currentResistCount+= 5;
                currentResistCountText.text = currentResistCount.ToString();

                // TODO: コインを取得したら、コインを減らす
            
        }
        


        private void CheckResistCount()
        {
            if (currentResistCount >= stage3_ResistCount)
            {
                skillupItemGroup.SetActive(true);
                skillupItemGroup.transform.position = stage3_Transform.position;
                skillupItemGroup.transform.rotation = stage3_Transform.rotation;



            }
            else if (currentResistCount >= stage2_ResistCount)
            {
                skillupItemGroup.SetActive(true);
                // ステージ3のトランスフォームをアクティブにする
                skillupItemGroup.transform.position = stage2_Transform.position;
                skillupItemGroup.transform.rotation = stage2_Transform.rotation;


            }
            else
            {
                skillupItemGroup.SetActive(false);
            }
        }


        // ローカルアバター判定ヘルパー
        private bool IsLocalAvatar()
        {
            // Unity エディタ（オフライン）でも動かせるよう null チェック
            if (SpatialBridge.actorService == null || 
                SpatialBridge.actorService.localActor == null)
            {
                return true;   // エディタ実行時は全部 true 扱い
            }

            // ローカルアバターの参照を更新
            ////UpdateLocalAvatarReference();
            
            if (localAvatar == null)
            {
                return false;
            }

            return false;    
            // コライダーとアバターの位置が近いかをチェック
            //float distance = Vector3.Distance(other.transform.position, localAvatar.position);
            //return distance < 2.0f; // 2メートル以内なら同じアバターのコライダーと判断
        }


        // ローカルアバターの参照を更新
        private void UpdateLocalAvatarReference()
        {
            if (localAvatar == null && SpatialBridge.actorService != null && 
                SpatialBridge.actorService.localActor != null && 
                SpatialBridge.actorService.localActor.avatar != null)
            {
                localAvatar = SpatialBridge.actorService.localActor.avatar;
            }
        }
        
    }
}
