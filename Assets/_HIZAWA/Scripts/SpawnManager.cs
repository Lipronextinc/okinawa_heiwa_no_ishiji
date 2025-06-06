using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space_1
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private GameObject MenuCanvas;
        [SerializeField] private GameObject SpawnPlane;
        public float hideDelay = 0.8f; //被アクティブまでの待機時間
        public float displayDelay = 3.0f; //表示までの待機時間

        private float timer;
        private bool isdisplayed = false;

        private void Start()
        {
            // 初期化
            timer = 0.0f;
            MenuCanvas.SetActive(false);
            isdisplayed = false;
        }

        private void Update()
        {
            timer += Time.deltaTime; // 経過時間をカウント
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
