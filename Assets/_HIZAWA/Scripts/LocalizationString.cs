using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space_1
{
    public class LocalizationString
    {
        public string en;
        public string ja;

        public string GetLocalizedString(string language)
        {
            return language == "en" ? en : ja;
        }
    }
}
