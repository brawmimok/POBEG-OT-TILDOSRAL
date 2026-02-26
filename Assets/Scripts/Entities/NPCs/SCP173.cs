//  - долбиться в стены
// if NavMesh.FindClosestEdge(transform.position, out var hit, NavMesh.AllAreas)
// agent.SetDestination(hit.position);

//  - долбиться в куда-то
//[SerializeField] private float wanderRadius = 10f; // Радиус поиска цели
//[SerializeField] private float waitAfterWalkTime = 5f;      // Пауза между перемещениями
//         if (agent.velocity.sqrMagnitude < 0.01f)
//{
//timer += Time.deltaTime;
//if (timer >= waitAfterWalkTime)
//{
//timer = 0;
//if (NavMesh.SamplePosition(Random.insideUnitSphere* wanderRadius + transform.position, out var hit, wanderRadius, NavMesh.AllAreas))
//{
//agent.SetDestination(hit.position);
//}
//}
//}
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

[RequireComponent(typeof(NavMeshAgent))]
public class SCP173 : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Collider ownCollider;
    [SerializeField] private Collider killZoneCollider;
    private NavMeshAgent agent; // Тут очень много всего нужного

    // Игрок затаргеченный
    private Player targetPlayer;
    public bool noticedByPlayer;
    public bool playerIsWatching;
    [SerializeField] private float triggerSoundRange = 10f;
    [SerializeField] private AudioClip[] noticedByPlayerSound;
    [SerializeField] private int randomMax = 8;

    [Header("Door Limit")]
    [SerializeField] private int maxDoorsCanOpen = 300;

    [Header("Sound Cooldown")]
    [SerializeField] private float soundCooldownTime = 5f;
    private float soundCooldownTimer;
    private bool canPlaySound = true;

    // Перемещение
    private Door traversingDoor;
    private bool traversingLink;
    private Vector3 traversingLinkEndPos;

    [Space]
    [Header("Disappear")]
    private bool disappearTimerWorking;
    private float disappearTimer;
    [SerializeField] private float disappearTimerMax = 35f;

    private void Start()
    {
        maxDoorsCanOpen = Random.Range(0, randomMax + 1);
        disappearTimerWorking = true;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updatePosition = false;
    }

    private void Update()
    {
        if (!canPlaySound)
        {
            soundCooldownTimer += Time.deltaTime;
            if (soundCooldownTimer >= soundCooldownTime)
            {
                canPlaySound = true;
                soundCooldownTimer = 0f;
            }
        }

        // Смотрит гад?
        if (targetPlayer != null)
        {
            agent.isStopped = false;
            Vector3 _vec = transform.position;
            bool playerIsWatchingPast = playerIsWatching;
            playerIsWatching = targetPlayer.PlayerIsWatching(ownCollider);
            playerIsWatching &= !targetPlayer.isBlinking;

            if ((playerIsWatching && !noticedByPlayer) ||
                (playerIsWatchingPast != playerIsWatching &&
                Vector3.Distance(targetPlayer.transform.position, transform.position) <= triggerSoundRange)
                )
            {
                if (canPlaySound)
                {
                    MainMechanics.instance.sfxAudio.PlayOneShot(noticedByPlayerSound[Random.Range(0, noticedByPlayerSound.Length)]);
                    canPlaySound = false;
                    soundCooldownTimer = 0f;
                }
                noticedByPlayer = true;
            }

            if (noticedByPlayer)
            {
                if (playerIsWatching)
                {
                    agent.velocity = Vector3.zero;
                }
                agent.isStopped = playerIsWatching;
                killZoneCollider.enabled = !playerIsWatching; // Киллзона

                // Логика ходилки бродилки
                if (agent.isOnOffMeshLink) // Большие толстые двери
                {
                    if (!traversingLink)
                    {
                        if (agent.currentOffMeshLinkData.owner != null)
                            traversingDoor = ((NavMeshLink)agent.currentOffMeshLinkData.owner).gameObject.GetComponent<Door>();
                        traversingLinkEndPos = agent.currentOffMeshLinkData.endPos + Vector3.up * agent.baseOffset;
                        traversingLink = true;
                    }
                    else
                    {
                        if (traversingDoor.isMoving || !traversingDoor.isDoorOpened)
                        {
                            if (!playerIsWatching && maxDoorsCanOpen > 0 && !traversingDoor.isMoving)
                            {
                                traversingDoor.OpenDoor();
                                maxDoorsCanOpen--;
                            }
                        }
                        else
                        {
                            if (Vector3.Distance(transform.position, traversingLinkEndPos) > 0.1f)
                            {
                                if (!agent.isStopped)
                                {
                                    transform.position = Vector3.MoveTowards(transform.position, traversingLinkEndPos, agent.speed * Time.deltaTime);
                                }
                            }
                            else
                            {
                                agent.CompleteOffMeshLink();
                                traversingLink = false;
                                traversingDoor = null;
                            }
                        }
                    }
                }
                else
                {
                    agent.SetDestination(targetPlayer.transform.position);
                    if (!agent.isStopped)
                    {
                        transform.position = agent.nextPosition;
                        if (agent.velocity.sqrMagnitude > 0.1f)
                            transform.rotation = Quaternion.LookRotation(agent.velocity);
                    }
                }
            }

        }
        // Таймер
        if (disappearTimerWorking)
        {
            if (disappearTimer < disappearTimerMax)
                disappearTimer += Time.deltaTime;
            else
                gameObject.SetActive(false);
        }
        // Контроль анимаций
        //TODO
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var playerScript = other.GetComponent<Player>();
        if (playerScript.alive)
        {
            disappearTimerWorking = false;
            targetPlayer = playerScript;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var playerScript = other.GetComponent<Player>();
        if (playerScript.alive)
        {
            disappearTimerWorking = true;
            noticedByPlayer = false;
            targetPlayer = null;
        }
    }
}