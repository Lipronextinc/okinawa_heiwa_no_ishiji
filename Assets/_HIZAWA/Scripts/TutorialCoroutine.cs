using SpatialSys.UnitySDK;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Space_1
{
    public class TutorialCoroutine : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI uiText;
        [SerializeField] private Collider[] tutorialColliders;
        [SerializeField] private Image image;
        [SerializeField] private Sprite[] sprites_web; //platformがwebのとき
        [SerializeField] private Sprite[] sprites_moblie; //platformがmoblieのとき

        private bool isTutorialCompleted = false; //チュートリアルを完了しているか
        private bool isFirstSavePointOpened = false; //セーブポイントを開けたか
        private bool isDashInstructionStarted = false; //ダッシュを判定するか
        private Coroutine movementCoroutine;
        private string platform;

        //アバターの動き検出関連
        private bool IsWalking;
        private bool IsRunning;
        private int countWalk = 0;
        private int countRun = 0;
        private int countJump = 0;
        private float _walkTime = 0f;
        private float _maxRunSpeed = 0f;

        public string currentLanguage = "ja"; //初期設定
        private bool languageSelected = false;

        void Start()
        {
            platform = SpatialBridge.actorService.localActor.platform.ToString(); //ユーザーのプラットフォームを取得

            for (int i = 0; i < tutorialColliders.Length; i++)
            {
                var triggerListener = tutorialColliders[i].gameObject.AddComponent<TriggerListener>();
                int index = i;
                triggerListener.OnTriggerEnterEvent += (other) => OnTutorialTrigger(other, index);
            }

            SpatialBridge.actorService.localActor.avatar.onLanded += onLanded; //着地したとき呼び出す

            StartCoroutine(TutorialSequence()); //コルーチンスタート
        }

        public void SetLanguage(string language)
        {
            currentLanguage = language;
            Debug.Log(currentLanguage);
        }

        public void LanguageButtonPressed()
        {
            languageSelected = true;
        }

        void onLanded()
        {
            countJump++;
        }

        IEnumerator TutorialSequence()
        {
            yield return new WaitUntil(() => languageSelected); //言語設定が完了するまで待機
            //Step 1;チュートリアル開始
            uiText.text = LocalizationList.title.GetLocalizedString(currentLanguage);
            Debug.Log(LocalizationList.title.GetLocalizedString(currentLanguage));
            yield return new WaitForSeconds(3f);

            //Step 2;(歩行判定を使用)
            uiText.text = LocalizationList.tutorial_0.GetLocalizedString(currentLanguage);
            Debug.Log(LocalizationList.tutorial_0.GetLocalizedString(currentLanguage));
            image.enabled = true;
            if (platform == "Web")
            {
                image.sprite = sprites_web[0];
            }
            else if (platform == "Mobile")
            {
                image.sprite = sprites_moblie[0];
            }
            else
            {
                //do not use sprites
            }
            movementCoroutine = StartCoroutine(CheckMovement());
            yield return new WaitUntil(() => countWalk >= 1);
            uiText.text = LocalizationList.ok.GetLocalizedString(currentLanguage);
            yield return new WaitForSeconds(1f);

            //Step 3;(走行判定を使用)
            uiText.text = LocalizationList.tutorial_1.GetLocalizedString(currentLanguage);
            if (platform == "Web")
            {
                image.sprite = sprites_web[1];
            }
            else if (platform == "Mobile")
            {
                image.sprite = sprites_moblie[1];
            }
            else
            {
                //do not use sprites
            }
            isDashInstructionStarted = true;
            yield return new WaitUntil(() => countRun >= 1);
            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
                movementCoroutine = null;
            }
            uiText.text = LocalizationList.ok.GetLocalizedString(currentLanguage);
            yield return new WaitForSeconds(1f);
            isDashInstructionStarted = false;

            //Step 4;(既存のジャンプ判定を使用)
            countJump = 0; //Jumpの回数をリセット
            uiText.text = LocalizationList.tutorial_2.GetLocalizedString(currentLanguage);
            if (platform == "Web")
            {
                image.sprite = sprites_web[2];
            }
            else if (platform == "Mobile")
            {
                image.sprite = sprites_moblie[2];
            }
            else
            {
                //do not use sprites
            }
            yield return new WaitUntil(() => countJump >= 2);
            uiText.text = LocalizationList.ok.GetLocalizedString(currentLanguage);
            image.enabled = false;
            yield return new WaitForSeconds(1f);

            uiText.text = LocalizationList.tutorial_3.GetLocalizedString(currentLanguage);
            yield return new WaitForSeconds(2f);
            uiText.text = "";

            //Step 5:セーブポイント
            yield return new WaitUntil(() => isTutorialCompleted);
            uiText.text = LocalizationList.tutorial_4.GetLocalizedString(currentLanguage);
            yield return new WaitForSeconds(1f);
            uiText.text = LocalizationList.tutorial_5.GetLocalizedString(currentLanguage);
            yield return new WaitForSeconds(2f);
            uiText.text = LocalizationList.tutorial_6.GetLocalizedString(currentLanguage);
            yield return new WaitUntil(() => isFirstSavePointOpened);
            uiText.text = LocalizationList.tutorial_7.GetLocalizedString(currentLanguage);
            yield return new WaitForSeconds(3f);
            uiText.text = LocalizationList.tutorial_8.GetLocalizedString(currentLanguage);
            yield return new WaitForSeconds(2f);
            uiText.text = "";

            yield break;
        }

        private void OnTutorialTrigger(Collider other, int index) //チュートリアルコースはコライダーでチュートリアルを管理
        {
            if (index < LocalizationList.tutorialcolliderMessages.Length)
            {
                uiText.text = LocalizationList.tutorialcolliderMessages[index].GetLocalizedString(currentLanguage);
            }
            else if (index == LocalizationList.tutorialcolliderMessages.Length) //最後のコライダー
            {
                isTutorialCompleted = true;
                Debug.Log(isTutorialCompleted);
            }
        }

        public void IsSavePointOpened() //UnityEvent()から呼び出し
        {
            isFirstSavePointOpened = true;
        }

        IEnumerator CheckMovement()
        {
            while (true)
            {
                yield return null;

                var velocity = SpatialBridge.actorService.localActor.avatar.velocity;
                float absVelocityX = Mathf.Abs(velocity.x);
                float absVelocityZ = Mathf.Abs(velocity.z);

                //歩行判定
                if (absVelocityX > 1.0f && absVelocityX < 6.7f || absVelocityZ > 1.0f && absVelocityZ < 6.7f)
                {
                    if (!IsWalking)
                    {
                        IsWalking = true;
                    }
                    _walkTime += Time.deltaTime;

                    if (_walkTime >= 2.0f)
                    {
                        countWalk++;
                        _walkTime = 0f;
                        IsWalking = false;
                    }
                }
                else
                {
                    IsWalking = false;
                }

                //走行判定(isDashInstructionStartedがTrueのとき)
                if (isDashInstructionStarted && (absVelocityX >= 6.6f || absVelocityZ >= 6.6f))
                {
                    if (!IsRunning)
                    {
                        IsRunning = true;
                        _maxRunSpeed = absVelocityX;
                    }
                    else
                    {
                        _maxRunSpeed = Mathf.Max(_maxRunSpeed, absVelocityX);
                    }
                }
                else if (isDashInstructionStarted && IsRunning && absVelocityX < _maxRunSpeed * 0.8f)
                {
                    countRun++;
                    IsRunning = false;
                }
                else if (isDashInstructionStarted && !(absVelocityX >= 6.6f || absVelocityZ >= 6.6f))
                {
                    IsRunning = false;
                }
            }
        }
    }
}