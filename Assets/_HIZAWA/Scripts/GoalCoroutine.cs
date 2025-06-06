using SpatialSys.UnitySDK;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Space_1
{
    public class GoalCoroutine : MonoBehaviour
    {
        public TextMeshProUGUI _consoleText;
        [SerializeField] private GameObject _teleport1;
        [SerializeField] private GameObject _teleport2;
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Goal");
            // _consoleText.text = "Goal!";
            // Invoke("TeleportStart", 5);
            // _teleport1.SetActive(false);
            // _teleport2.SetActive(false);
        }
        void TeleportStart()
        {
            _consoleText.text = "";
            IAvatar localAvatar = SpatialBridge.actorService.localActor.avatar;
            localAvatar.position = new Vector3(-41, 1, 9);
        }
    }
}
