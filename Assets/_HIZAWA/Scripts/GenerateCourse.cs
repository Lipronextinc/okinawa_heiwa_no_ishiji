using SpatialSys.UnitySDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space_1
{
    public class GenerateCourse : MonoBehaviour
    {
        public GameObject[] Train;

        void Start()
        {
            int number = Random.Range(0, Train.Length);
            Debug.Log(number);
            //Instantiate(Train[number], transform.position, transform.rotation);
            for(int i = 0; i < Train.Length; i++)
            {
                if (i != number)
                {
                    Train[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
