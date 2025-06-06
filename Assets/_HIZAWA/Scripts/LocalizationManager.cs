using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Space_1
{
    public class LocalizationManager : MonoBehaviour
    {
        public TMP_Dropdown dropdown;
        public TutorialCoroutine tutorialCoroutine;
        [SerializeField] private TextMeshProUGUI languageText;
        [SerializeField] private TextMeshProUGUI selectbuttonText;

        [SerializeField] private TextMeshProUGUI leaderboardText;

        void Start()
        {
            DropdownValueChanged(dropdown); //初期設定

            dropdown.onValueChanged.AddListener(delegate //ドロップダウンの値が変わったら
            {
                DropdownValueChanged(dropdown);
            });
        }

        void DropdownValueChanged(TMP_Dropdown change) //ドロップダウンの値が変わったら
        {
            int index = change.value;
            string language = "";

            switch (index)
            {
                case 0:
                    language = "ja";
                    break;
                case 1:
                    language = "en";
                    break;
                default:
                    language = "ja";
                    break;
            }

            tutorialCoroutine.SetLanguage(language);
            languageText.text = LocalizationList.languageMenu.GetLocalizedString(language);
            selectbuttonText.text = LocalizationList.languageSet.GetLocalizedString(language);

            //
            leaderboardText.text = LocalizationList.leaderboard.GetLocalizedString(language);
        }
    }
}