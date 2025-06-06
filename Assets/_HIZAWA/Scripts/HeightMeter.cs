using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpatialSys.UnitySDK;

namespace Space_1
{
    public class HeightMeter : MonoBehaviour
    {
        //[SerializeField] private Slider heightSlider;
        [SerializeField] private TextMeshProUGUI heightText;

        
        [Header("Leaderboard Transform")]
        [SerializeField] private Transform target_leaderboard;

        [Header("Stage Transforms")]
        [SerializeField] private Transform stage_01_transform;
        [SerializeField] private Transform stage_02_transform; 
        [SerializeField] private Transform stage_03_transform;
        [SerializeField] private Transform stage_g_transform;
        // Start is called before the first frame update

        [Header("Turning Point Transforms")]
        [SerializeField] private float stage_02_height = 0;
        [SerializeField] private float stage_03_height = 0;
        [SerializeField] private float stage_g_height = 0;


        float maxheight = 0;
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            IAvatar localAvatar = SpatialBridge.actorService.localActor.avatar;

            if(localAvatar == null) {
                localAvatar = SpatialBridge.actorService.localActor.avatar;
            }

            if(localAvatar.position.y > maxheight){

                if(maxheight < 1201){
                    maxheight = localAvatar.position.y;
                }

                ////.value = maxheight;
            }
            heightText.text = maxheight.ToString("F0");// + "m";


            // 追加: target_leaderboard の Y 座標でステージに合わせて移動
            float leaderboardY = target_leaderboard.position.y;

            if (localAvatar.position.y >= stage_g_height)
            {
                target_leaderboard.position = stage_g_transform.position;
                target_leaderboard.rotation = stage_g_transform.rotation;

            }
            else if (localAvatar.position.y >= stage_03_height)
            {
                target_leaderboard.position = stage_03_transform.position;
                target_leaderboard.rotation = stage_03_transform.rotation;
            }
            else if (localAvatar.position.y >= stage_02_height)
            {
                target_leaderboard.position = stage_02_transform.position;
                target_leaderboard.rotation = stage_02_transform.rotation;
            }
            else
            {
                target_leaderboard.position = stage_01_transform.position;
                target_leaderboard.rotation = stage_01_transform.rotation;
            }
            

        
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
