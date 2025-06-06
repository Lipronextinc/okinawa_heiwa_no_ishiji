using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space_1
{
    public class AutoRotate : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(0, 0, speed * Time.deltaTime);
        }
    }
}
