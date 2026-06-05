using System;
using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshLink))]
[RequireComponent(typeof(BoxCollider))]
public class Door : PepusBehaviour
{
    [Header("Door Settings")]
    public bool startDoorOpen = false;
    public bool NpcCanOpen = true;

    [SerializeField] private DoorInfo doorInfo;
    [SerializeField] private AudioSource audioSource;

    [Space]

    private Vector3 initialPosition;  // Исходная позиция двери
    private Vector3 targetPosition;   // Конечная позиция двери
    private NavMeshLink navMeshLink;

    [NonSerialized] public bool isDoorOpened;
    [NonSerialized] public bool isMoving;

    private const int walkableAreaType = 0;
    private const int closedDoorAreaType = 3;
    //private NavMeshObstacle _obstacle;

    private void Start()
    {
        navMeshLink = GetComponent<NavMeshLink>();
        initialPosition = transform.position;  // Запоминаем стартовую позицию двери
        targetPosition = initialPosition +
            
             transform.TransformDirection( Vector3.Scale(doorInfo.openOffset, GetComponent<BoxCollider>().size));  // Вычисляем конечную позицию двери


        if (startDoorOpen)
        {
            navMeshLink.area = walkableAreaType;
            transform.position = targetPosition;
            isDoorOpened = true;
        }
        navMeshLink.enabled = NpcCanOpen || isDoorOpened;
    }
    public void ToggleDoor(bool shouldPlaySound = true)
    {
        if (isDoorOpened)
            CloseDoor(shouldPlaySound);
        else
            OpenDoor(shouldPlaySound);
    }

    public void OpenDoor(bool shouldPlaySound = true)
    {
        if (!isMoving && !isDoorOpened)
        {
            StartCoroutine(MoveDoor(targetPosition));
            if (shouldPlaySound && doorInfo.openSounds.Length > 0) audioSource.PlayOneShot(doorInfo.openSounds[Random.Range(0, doorInfo.openSounds.Length)]);
            isDoorOpened = true;
            navMeshLink.enabled = true;
            navMeshLink.area = walkableAreaType;
        }
    }

    public void CloseDoor(bool shouldPlaySound = true)
    {
        if (!isMoving && isDoorOpened)
        {
            transform.position = initialPosition;
            navMeshLink.area = closedDoorAreaType;
            transform.position = targetPosition;
            StartCoroutine(MoveDoor(initialPosition));
            if (shouldPlaySound && doorInfo.closeSounds.Length > 0) audioSource.PlayOneShot(doorInfo.closeSounds[Random.Range(0, doorInfo.closeSounds.Length)]);
            isDoorOpened = false;
            navMeshLink.enabled = NpcCanOpen;
        }
    }
    private IEnumerator MoveDoor(Vector3 destination)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < doorInfo.moveDuration)
        {
            elapsed += Time.deltaTime;

            // Математическое сглаживание (ускорение в начале, замедление в конце)
            float smoothT = Mathf.SmoothStep(0f, 1f, elapsed / doorInfo.moveDuration);

            transform.position = Vector3.Lerp(startPosition, destination, smoothT);
            yield return null;
        }

        transform.position = destination;
        isMoving = false;
    }
}
