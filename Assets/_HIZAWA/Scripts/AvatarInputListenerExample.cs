using UnityEngine;
using SpatialSys.UnitySDK;
namespace Space_1
{
    public class AvatarInputListenerExample : MonoBehaviour, IAvatarInputActionsListener

    {
        private int jumpCount = 0;
        private const int maxJumps = 3; // 最大ジャンプ回数（3回まで）

        private void Start()
        {
            Debug.Log($"ジャンプの取得を実行します");
            // アバターの入力キャプチャを開始
            SpatialBridge.inputService.StartAvatarInputCapture(true, true, true, true, this);
        }

        public void OnAvatarJumpInput(InputPhase inputPhase)
        {
            Debug.Log($"ジャンプを実行しました。現在のジャンプ回数: {jumpCount}");
            if (jumpCount < maxJumps)
            {
                jumpCount++;
                // IAvatar の参照を取得
                IAvatar localAvatar = SpatialBridge.actorService.localActor.avatar;
                // プレイヤーが [W] キーを1フレーム押しているのと同等の処理
                localAvatar.Move(Vector3.up);
                // ジャンプ処理の実行
                localAvatar.Jump();
                
                SpatialBridge.coreGUIService.DisplayToastMessage($"ジャンプ {jumpCount} 回目!");
            }
            else
            {
                SpatialBridge.coreGUIService.DisplayToastMessage("ジャンプ回数の上限に達しました。");
            }
        }

        // 地面に着地したらジャンプ回数をリセット
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                jumpCount = 0;
                SpatialBridge.coreGUIService.DisplayToastMessage("着地: ジャンプ回数をリセットしました。");
            }
        }

        // 追加: IAvatarInputActionsListener と IInputActionsListener の未使用メソッド
        public void OnAvatarMoveInput(InputPhase inputPhase, Vector2 moveDirection)
        {
            // 未使用のため何もしません
        }

        public void OnAvatarSprintInput(InputPhase inputPhase)
        {
            // 未使用のため何もしません
        }

        public void OnAvatarActionInput(InputPhase inputPhase)
        {
            // 未使用のため何もしません
        }

        public void OnAvatarAutoSprintToggled(bool toggled)
        {
            // 未使用のため何もしません
        }

        public void OnInputCaptureStarted(InputCaptureType captureType)
        {
            // 未使用のため何もしません
        }

        public void OnInputCaptureStopped(InputCaptureType captureType)
        {
            // 未使用のため何もしません
        }
    } 

}