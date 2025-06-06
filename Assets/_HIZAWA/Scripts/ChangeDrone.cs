using SpatialSys.UnitySDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space_1
{
    public class ChangeDrone: MonoBehaviour
    {
        [SerializeField] private GameObject droneD;
        [SerializeField] private GameObject droneN;
        [SerializeField] private GameObject droneR;
        private int gearIndex = 0;
        private string[] gears = { "D", "N", "R" };

        [SerializeField] private SpatialSFX sfx;
        [Range(0f, 1.5f)][SerializeField] private float volume = 1f;
        [Range(0.1f, 2f)][SerializeField] private float pitch = 1f;
        
        //Gearを切り替えることで表示されるドローンを変更する
        public void GearChange()
        {
            gearIndex = (gearIndex + 1) % 3;
            droneD.SetActive(gearIndex == 0);
            droneN.SetActive(gearIndex == 1);
            droneR.SetActive(gearIndex == 2);

            //効果音再生
            SpatialBridge.audioService.PlaySFX(sfx, transform.position, volume, pitch);
        }
    }
}
