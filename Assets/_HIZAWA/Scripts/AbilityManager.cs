using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;
using SpatialSys.UnitySDK.Internal;

namespace Space_1
{

    /* 使う側は  AbilityManager.Instance.UseCharge(AbilityType.SuperJump);  のように呼ぶだけ */
    public enum AbilityType { SuperJump, Hover, Dash }
    public class AbilityManager : MonoBehaviour, IAvatarInputActionsListener
    {

        
    




        /* Inspector から初期値を調整できる */
        [Header("Initial Charges (per session)")]
        public int initialSuperJump = 0;
        public int initialHover     = 0;

        /* ---- 内部 ---- */
        private const string KEY_SUPERJUMP = "supjump_ct";
        private const string KEY_HOVER     = "hover_ct";

        private readonly Dictionary<AbilityType, int> _cache = new();
        public static AbilityManager Instance { get; private set; }

        public event Action<AbilityType,int> onChargeChanged; // （任意）UI 更新用

        private bool _isCapturingJump = false;
        int jumpCount = 0;
        int maxJumpCount = 100;
        float jumpImpulse = 1000f;

        
        /* --------------------- ライフサイクル --------------------- */
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(this); return; }
        }

        private void Start()
        {
            var local = SpatialBridge.actorService?.localActor;
            var avatar = SpatialBridge.actorService.localActor.avatar;
            if (local == null) return; // エディタ実行時はここをスキップ

            
            //EnvironmentSettingsOverrides.Modify(s => s.avatarJumpCount  = 5);
            // var ctl = SpatialBridge.actorService?.localActor.controlSettings;
            // ctl.maximumJumpCount = 4;      // defalut = 2
            // ctl.jumpHeightMeters = 3f;    // defalut = 1.5
            // ctl.gravityMultiplier = 0.5f; // defalut = 1
            // SpatialBridge.actorService?.localActor.controlSettings = ctl;   // <<<<<< 必ず丸ごと再代入

            
            SpatialBridge.inputService.StartAvatarInputCapture(
                movement: false,
                jump: false,
                sprint: false,
                actionButton: false,
                listener: this        // ← AbilityManager が IAvatarInputActionsListener
            );

            /* 未定義なら初期値をセット */
            InitIfMissing(local, KEY_SUPERJUMP, initialSuperJump);
            InitIfMissing(local, KEY_HOVER,     initialHover);

            RefreshCacheFromProps();

            /* ほかスクリプトで書き換えられたとき用 */
            //local.onCustomPropertiesChanged += _ => RefreshCacheFromProps();
            // 着地イベントでジャンプ回数をリセット
            local.avatar.onLanded +=OnAvatarLanded;

            avatar.maxJumpCount = 50; // defalut = 2 
            avatar.jumpHeight = 300f;  // 高く跳ぶ    defalut = 1.5
            avatar.gravityMultiplier = 0.1f; // 低重力  defalut = 1.5
            avatar.runSpeed = 100f;        // defalut = 6.875
            avatar.walkSpeed = 100f;      // defalut = 3

            Debug.Log($" maxJumpCount: {avatar.maxJumpCount}, jumpHeight: {avatar.jumpHeight}, gravityMultiplier: {avatar.gravityMultiplier}, runSpeed: {avatar.runSpeed}, walkSpeed: {avatar.walkSpeed}");

            Debug.Log($"AbilityManager の初期化を実行します");
        }

        private static void InitIfMissing(ILocalActor local, string key, int value)
        {
            if (!local.customProperties.ContainsKey(key))
                local.SetCustomProperty(key, value);
        }

        private void RefreshCacheFromProps()
        {
            var props = SpatialBridge.actorService.localActor.customProperties;
            _cache[AbilityType.SuperJump] = props.TryGetValue(KEY_SUPERJUMP, out var sj) ? (int)sj : 0;
            _cache[AbilityType.Hover]     = props.TryGetValue(KEY_HOVER,     out var hv) ? (int)hv : 0;
        }

        /* --------------------- パブリック API --------------------- */
        public int GetCharges(AbilityType type) => _cache.TryGetValue(type, out var v) ? v : 0;

        public void AddCharges(AbilityType type, int amount = 1)
        { SetCharges(type, GetCharges(type) + amount); }

        public bool UseCharge(AbilityType type, int amount = 1)
        {
            if (GetCharges(type) < amount) return false;
            SetCharges(type, GetCharges(type) - amount);
            return true;
        }

        // チャージを設定する
        private void SetCharges(AbilityType type, int newValue)
        {
            string key = type == AbilityType.SuperJump ? KEY_SUPERJUMP : KEY_HOVER;
            SpatialBridge.actorService.localActor.SetCustomProperty(key, newValue);
            _cache[type] = newValue;
            onChargeChanged?.Invoke(type, newValue);

            // SuperJump チャージが 0→1／1→0 になったらキャプチャを切り替え
            if (type == AbilityType.SuperJump)
                EnsureJumpCapture(newValue > 0);
        }

        
        private void Update(){}

        public void AddSuperJumpCharge()
        {
            AddCharges(AbilityType.SuperJump);
        }

        public void AddHoverCharge()
        {
            AddCharges(AbilityType.Hover);
        }

        public void AddDashCharge() 
        {
            AddCharges(AbilityType.Dash);
        }
        

        public void OnAvatarJumpInput(InputPhase inputPhase)
        {
            if (inputPhase != InputPhase.OnPressed) return;

            IAvatar localAvatar = SpatialBridge.actorService.localActor.avatar;
            localAvatar.Jump();
            jumpCount++; 

            
            if (jumpCount < maxJumpCount) 
            {
                localAvatar.AddForce(Vector3.up * jumpImpulse);
                localAvatar.Jump();
                Debug.Log($"標準ジャンプを実行しました。現在のジャンプ回数:{jumpCount} / {maxJumpCount} ");

                
                return;
                //localAvatar.velocity.y = jumpImpulse; 
                
                
            }else{
                SpatialBridge.actorService.localActor.avatar.Jump(); 
            }
            
            // チャージを 1 消費して 3 段目以降を上乗せ
            // if (UseCharge(AbilityType.SuperJump))
            // {
            //     SpatialBridge.actorService.localActor.avatar.Jump();   // 手動ジャンプ&#8203;:contentReference[oaicite:4]{index=4}
            //     SpatialBridge.coreGUIService.DisplayToastMessage(
            //         $"SuperJump! 残り {GetCharges(AbilityType.SuperJump)}");
            // }

            // // 残数 0 になったら自動的に標準ジャンプへ戻す
            // if (GetCharges(AbilityType.SuperJump) == 0)
            //     EnsureJumpCapture(false);
        }

        // 着地時にカウントを0に戻す
        private void OnAvatarLanded()
        {
            jumpCount = 0;
        }

        // 追加: IAvatarInputActionsListener と IInputActionsListener の未使用メソッド
        public void OnAvatarMoveInput(InputPhase inputPhase, Vector2 moveDirection)
        {
             Debug.Log("移動 ");
        }

        public void OnAvatarSprintInput(InputPhase inputPhase)
        {
            // 未使用のため何もしません
            Debug.Log("スプリント");
        }

        public void OnAvatarActionInput(InputPhase inputPhase)
        {
            // 未使用のため何もしません
            Debug.Log("アバターアクション");
        }

        public void OnAvatarAutoSprintToggled(bool toggled)
        {
            // 未使用のため何もしません
            Debug.Log("自動スプリント？ ");
        }

        public void OnInputCaptureStarted(InputCaptureType captureType)
        {
            //Debug.Log("ユーザー入力をオーバーライドします");
        }

        public void OnInputCaptureStopped(InputCaptureType captureType)
        {
            //Debug.Log("ユーザー入力をオーバーライドを停止します");
        }


        /* --------------------- ジャンプキャプチャ --------------------- */
        private void EnsureJumpCapture(bool needCapture)
        {
            Debug.Log("ジャンプキャプチャ: " + needCapture);
            if (needCapture == _isCapturingJump) return;

            if (needCapture)
            {
                SpatialBridge.inputService.StartAvatarInputCapture(
                    movement:false, jump:true, sprint:false, actionButton:false, listener:this);
            }
            else
            {
                SpatialBridge.inputService.ReleaseInputCapture(this);           // ← キャプチャ解除&#8203;:contentReference[oaicite:3]{index=3}
            }
            _isCapturingJump = needCapture;
        }


        private void OnDestroy()
        {
            if (_isCapturingJump)
                SpatialBridge.inputService.ReleaseInputCapture(this);
        }



    }

    public class EnvironmentSettingsOverrides : MonoBehaviour
    {
        public static SpatialEnvironmentSettingsOverrides GetEnvOverrides()
        => UnityEngine.Object.FindObjectOfType<SpatialEnvironmentSettingsOverrides>();

        public static void Modify(System.Action<EnvironmentSettings> mutator)
        {
            var env = GetEnvOverrides();
            if (env == null) return;

            // var es = env.environmentSettings;            // 上位コピー
            // var acs = es.avatarControlSettings;          // サブコピー

            // acs.maximumJumpCount = 4;                // ★ここを書き換え
            // es.avatarControlSettings = acs;              // サブを戻す
            // env.environmentSettings = es;                     // 丸ごと再代入
        }
    }
}
