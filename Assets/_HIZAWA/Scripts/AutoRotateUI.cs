using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space_1
{
    public class AutoRotateUI : MonoBehaviour
    {
        [SerializeField] private float speed = 2f;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(0, speed * Time.deltaTime, 0);
        }
    }
}
