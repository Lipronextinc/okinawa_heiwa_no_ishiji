using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;

namespace Space_1
{
    public class FowardFrontSprite : MonoBehaviour
    {
        [SerializeField] private bool onlyHorizontal = false;
        
        private Transform cameraTransform;

        // Start is called before the first frame update
        void Start()
        {
            // Spatialのカメラを参照する
            UpdateCameraReference();
        }

        // Update is called once per frame
        void Update()
        {
            // 毎フレームカメラ参照を確認・更新
            if (cameraTransform == null)
            {
                UpdateCameraReference();
            }
            
            if (cameraTransform != null)
            {
                if (onlyHorizontal)
                {
                    // カメラの位置から自身の位置を引いて方向ベクトルを作成
                    Vector3 direction = cameraTransform.position - transform.position;
                    // Y成分を0にしてY軸のみの回転に制限
                    direction.y = 0;
                    
                    if (direction != Vector3.zero)
                    {
                        transform.rotation = Quaternion.LookRotation(direction);
                    }
                }
                else
                {
                    // 常にカメラの方を向く
                    transform.LookAt(cameraTransform);
                }
            }
        }

        private void UpdateCameraReference()
        {
            // Spatialのカメラシステムを使用
            if (SpatialBridge.actorService != null && 
                SpatialBridge.actorService.localActor != null && 
                SpatialBridge.actorService.localActor.avatar != null)
            {
                // アバターのカメラを参照
                ////cameraTransform = SpatialBridge.actorService.localActor.avatar.cameraTransform;
            }
            else
            {
                // フォールバック：エディタ実行時などはメインカメラを使用
                //cameraTransform = Camera.main?.transform;
            }
        }
    }
}