using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space_1
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private GameObject MenuCanvas;
        [SerializeField] private GameObject SpawnPlane;
        public float hideDelay = 0.8f; //��A�N�e�B�u�܂ł̑ҋ@����
        public float displayDelay = 3.0f; //�\���܂ł̑ҋ@����

        private float timer;
        private bool isdisplayed = false;

        private void Start()
        {
            // ������
            timer = 0.0f;
            MenuCanvas.SetActive(false);
            isdisplayed = false;
        }

        private void Update()
        {
            timer += Time.deltaTime; // �o�ߎ��Ԃ��J�E���g
            if(!isdisplayed )
            {
                if (timer >= hideDelay)
                {
                    SpawnPlane.SetActive(false);
                }
                if (timer >= displayDelay)
                {
                    MenuCanvas.SetActive(true);
                    isdisplayed = true;
                }
            }
        }
    }
}
