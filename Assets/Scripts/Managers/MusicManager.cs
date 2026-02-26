//using UnityEngine;

//public class MusicManager : MonoBehaviour
//{
//    [Header("Triggers")]
//    [SerializeField] private GameObject lczTrigger;
//    [SerializeField] private GameObject hczTrigger;
//    [SerializeField] private GameObject ozTrigger;

//    [Space]
//    [Header("Musics")]
//    [SerializeField] private AudioSource lczMusic;
//    [SerializeField] private AudioSource hczMusic;
//    [SerializeField] private AudioSource ozMusic;

//    private bool _playerInLCZ;
//    private bool _playerInHCZ;
//    private bool _playerInOZ;

//    private bool playerInTrigger = false;

//    private void OnTriggerEnter(Collider other)
//    {
//        print("OnTriggerEnter");

//        if (other.CompareTag("Player"))
//        {
//            print("CompareTag");
//            if (other.gameObject == lczTrigger)
//            {
//                print("Я вошёл в LCZ"); 
//                _playerInLCZ = true;
//                lczMusic.Play();
//            }
//            else if (other.gameObject == hczTrigger)
//            {
//                Debug.Log("Я вошёл в HCZ");
//                _playerInLCZ = true;
//                hczMusic.Play();
//            }
//            else if (other.gameObject == ozTrigger)
//            {
//                _playerInOZ = true;
//                ozMusic.Play();
//            }
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            if (other.gameObject == lczTrigger)
//            {
//                _playerInLCZ = false;

//                hczMusic.Stop();
//                ozMusic.Stop();

//                Debug.Log("Вышел с LCZ");
//            }
//            else if (other.gameObject == hczTrigger)
//            {
//                _playerInHCZ = false;

//                lczMusic.Stop();
//                ozMusic.Stop();

//                Debug.Log("Вышел с HCZ");
//            }
//            else if (other.gameObject == ozTrigger)
//            {
//                _playerInOZ = false;

//                lczMusic.Stop();
//                hczMusic.Stop();
//            }
//        }
//    }
//}
