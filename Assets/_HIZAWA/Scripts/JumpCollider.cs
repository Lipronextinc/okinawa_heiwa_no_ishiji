// JumpCollider.cs
using SpatialSys.UnitySDK;
using System.Collections;
using System.Collections.Generic;
//using Spatial; // Spatial SDKの名前空間
using UnityEngine;
using SpatialSys.UnitySDK;

namespace Space_1
{
    public class JumpCollider : MonoBehaviour
    {
        [SerializeField] private float jumpHeight = 5f;   // ジャンプの高さ
        [SerializeField] private float jumpDuration = 0.5f; // ジャンプにかかる時間
        private bool isJumping = false;

        private void OnTriggerEnter(Collider other)
        {
            // コライダーが何かに触れた時、ローカルプレイヤーをジャンプさせる
            // Spatial.ioではアバター自身がコライダーに触れたときに反応します
            if (!isJumping)
            {
                Debug.Log("コライダーが検出されました");
                StartCoroutine(PerformJump());
            }
        }
        
        private IEnumerator PerformJump()
        {
            isJumping = true;
            Vector3 startPos = SpatialBridge.actorService.localActor.avatar.position;
            Vector3 apexPos = startPos + Vector3.up * jumpHeight;
            float halfDuration = jumpDuration / 2f;
            float elapsed = 0f;

            // 上昇フェーズ
            while (elapsed < halfDuration)
            {
                float t = elapsed / halfDuration;
                // Spatial.ioのアバターの位置を更新
                SpatialBridge.actorService.localActor.avatar.position = Vector3.Lerp(startPos, apexPos, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // 降下フェーズ
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                float t = elapsed / halfDuration;
                // Spatial.ioのアバターの位置を更新
                SpatialBridge.actorService.localActor.avatar.position = Vector3.Lerp(apexPos, startPos, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            SpatialBridge.actorService.localActor.avatar.position = startPos;
            isJumping = false;
        }
    }
}