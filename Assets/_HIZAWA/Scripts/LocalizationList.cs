using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Space_1
{
    public static class LocalizationList
    {
        //言語設定メニュー
        public static LocalizationString languageMenu = new LocalizationString
        {
            en = "Welcome to this Space.\r\nPlease Select Language.",
            ja = "本スペースへようこそ！\r\n言語を選んでください。"
        };

        public static LocalizationString languageSet = new LocalizationString
        {
            en = "Select",
            ja = "決定"
        };

        //チュートリアル
        public static LocalizationString title = new LocalizationString
        {
            en = "GAME INSTRACTIONS",
            ja = "操作チュートリアルを始めます。"
        };

        public static LocalizationString ok = new LocalizationString
        {
            en = "GOOD",
            ja = "    ○"
        };

        public static LocalizationString tutorial_0 = new LocalizationString
        {
            en = "Walk around the space.",
            ja = "スペース内を歩き回ってみましょう。"
        };

        public static LocalizationString tutorial_1 = new LocalizationString
        {
            en = "Dash around the space.",
            ja = "スペース内を走ってみましょう。"
        };

        public static LocalizationString tutorial_2 = new LocalizationString
        {
            en = "Jump (twice)",
            ja = "ジャンプしてみましょう。(2回）"
        };

        public static LocalizationString tutorial_3 = new LocalizationString
        {
            en = "Now, you master basic movements.\nLet's start the course!",
            ja = "これで基本的な動きをマスターしました。\n実際にコースで遊んでみましょう！"
        };

        public static LocalizationString[] tutorialcolliderMessages = new LocalizationString[]
        {
            new LocalizationString { en = "Jump and Move to the next car.", ja = "ジャンプして次の車に飛び移りましょう。" },
            new LocalizationString { en = "Jump while dashing, You can reach further.", ja = "走りながら飛ぶとさらに遠くに飛べます。" },
            new LocalizationString { en = "You can Jump while Jumping and go further [Double Jump]", ja = "飛んでいるとき、もう１段階飛ぶことができます。[ダブルジャンプ]" },
            new LocalizationString { en = "Dash and Double Jump is the best way to move further.", ja = "走りながらダブルジャンプが一番遠くまで飛ぶ方法です。" },
            new LocalizationString { en = "", ja = "" }
        };

        public static LocalizationString tutorial_4 = new LocalizationString
        {
            en = "Well Done!",
            ja = "チュートリアルコース完走です！"
        };

        public static LocalizationString tutorial_5 = new LocalizationString
        {
            en = "In this space, you can use save points.",
            ja = "このスペースでは、セーブポイントを使用できます。"
        };

        public static LocalizationString tutorial_6 = new LocalizationString
        {
            en = "Let's open the save point on the left.",
            ja = "左側のセーブポイントを開きましょう。"
        };

        public static LocalizationString tutorial_7 = new LocalizationString
        {
            en = "Now, you will come back here if you fall out.",
            ja = "これでコースから落ちてもここに戻ってきます。"
        };

        public static LocalizationString tutorial_8 = new LocalizationString
        {
            en = "Climb up and start your time attack!",
            ja = "上に登ってタイムアタックを始めましょう！"
        };

        public static LocalizationString[] coinNames = new LocalizationString[]
        {
            new LocalizationString { en = "Metal thin film chip resistors URG series", ja = "金属皮膜チップ抵抗器 URGシリーズ" },
            new LocalizationString { en = "High Current chip jumpers Yja series", ja = "高電流表面実装形ジャンパーチップ Yjaシリーズ" },
            new LocalizationString { en = "High Frequency Chip Resistors RFD series", ja = "高周波チップ抵抗器 RFDシリーズ" },
            new LocalizationString { en = "Low resistance chip resistors RL series", ja = "低抵抗チップ抵抗器（短辺電極） RLシリーズ" },
            new LocalizationString { en = "", ja = "" }
        };

        public static LocalizationString[] coinExplanations = new LocalizationString[]
        {
            new LocalizationString { en = "The tightest resistance tolerance: +/-0.01%. The smallest temperature coefficient of resistance: ±1ppm.", 
                                     ja = "超高精度の抵抗値許容差：±0.01%、究極の抵抗温度係数：±1ppm/℃、無機質保護膜の採用による長期安定性。" },
            new LocalizationString { en = "Simplify power line change, looping and circuit design when changing current Less than 0.3mΩ within operating temperature range.", 
                                     ja = "電流変更時による電源ライン切替え・ループ・回路設計簡易化を実現。使用温度範囲内での抵抗値は0.3m Ω以下。" },
            new LocalizationString { en = "A resistor that takes advantage of the characteristics of thin films, and supports a wide range of frequencies from DC to 67 GHz.", 
                                     ja = "薄膜の特性を生かした抵抗器で、DC から 67GHz までの幅広い周波数に対応、小型の 0603 サイズ。" },
            new LocalizationString { en = "Innovative structure that takes consideration of heat dissipation suppress the surface temperature enabling the small sizes reducing the influence of heat on surrounding components.", 
                                     ja = "放熱、熱分散を考慮した独自構造により表面温度上昇を押さえ、小型形状を実現し、周辺部分への影響を軽減。" },
            new LocalizationString { en = "", ja = "" }
        };

        public static LocalizationString leaderboard = new LocalizationString
        {
            en = "This is the Leaderboard.\r\nWhen you enter the gate, the time attack starts.",
            ja = "リーダーボードでは、タイムアタック上位ユーザーのランキングを見ることができます。"
        };
    }
}
