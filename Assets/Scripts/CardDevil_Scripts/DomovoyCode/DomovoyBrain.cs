using UnityEngine;
using UnityEngine.AI;

public class DomovoyBrain : MonoBehaviour
{
    [Header("Actions")]
    public bool isRoaming=false;
    public bool isChasingSound=false;
    public bool isChasingPlayer=false;

    [Header("Player")]
    [SerializeField] GameObject player;
    [SerializeField] bool isPlayerInChaseArea=false;
    [SerializeField] bool isPlayerInHearArea = false;
    [SerializeField] bool isPlayerRunning = false;

    [Header("Values")]
    [SerializeField] int moveSpeed;
    [SerializeField] int rotationSpeed;
    [SerializeField] float chaseRange;
    //[SerializeField] float acc;

    NavMeshAgent domovoyLogic;
    LineRenderer lineRenderer;

    EventBinding<MakeSoundEvent> soundEventBinding;
    EventBinding<PlayerInHearAreaEvent> hearAreaEventBinding;
    EventBinding<PlayerInChaseAreaEvent> chaseAreaEventBinding;
    EventBinding<PlayerIsRunningEvent> playerisRunningEventBinding;

    private void OnEnable()
    {
        soundEventBinding = new EventBinding<MakeSoundEvent>(HandleSoundEvent);
        EventBus<MakeSoundEvent>.Register(soundEventBinding);

        hearAreaEventBinding = new EventBinding<PlayerInHearAreaEvent>(HandleHearAreaTrigger);
        EventBus<PlayerInHearAreaEvent>.Register(hearAreaEventBinding);

        chaseAreaEventBinding = new EventBinding<PlayerInChaseAreaEvent>(HandleChaseAreaTrigger);
        EventBus<PlayerInChaseAreaEvent>.Register(chaseAreaEventBinding);

        playerisRunningEventBinding = new EventBinding<PlayerIsRunningEvent>(HandlePlayerRunningEvent);
        EventBus<PlayerIsRunningEvent>.Register(playerisRunningEventBinding);
    }

    private void OnDisable()
    {
        EventBus<MakeSoundEvent>.Deregister(soundEventBinding);

        EventBus<PlayerInHearAreaEvent>.Deregister(hearAreaEventBinding);

        EventBus<PlayerInChaseAreaEvent>.Deregister(chaseAreaEventBinding);

        EventBus<PlayerIsRunningEvent>.Deregister(playerisRunningEventBinding);
    }

    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        domovoyLogic = GetComponent<NavMeshAgent>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        domovoyLogic.acceleration = 5*moveSpeed;
        domovoyLogic.speed = moveSpeed;
        domovoyLogic.angularSpeed = rotationSpeed;
        domovoyLogic.stoppingDistance = 0.1f;
        domovoyLogic.autoBraking = true;

    }

    private void Update()
    {
        //domovoyLogic.SetDestination(player.transform.position);

        if (SeePlayer() || isChasingPlayer)
        {
            HandleChase();
        }

        DomovoyRunningHear();

        if (domovoyLogic.hasPath)
        {
            DrawPath();
            if (isChasingSound && transform.position == domovoyLogic.pathEndPosition)
            {
                isChasingSound = false;
            }
        }
        else
        {
            lineRenderer.positionCount = 0; 
        }
    }

    void HandlePlayerRunningEvent(PlayerIsRunningEvent e)
    {
        isPlayerRunning = e.isPlayerRunning;
    }

    void DomovoyRunningHear()
    {
        if (isPlayerRunning && isPlayerInHearArea)
        {
            domovoyLogic.SetDestination(player.transform.position);
        }
    }

    bool SeePlayer()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if(Physics.Raycast(ray, out RaycastHit hit, chaseRange))
        {
            if(hit.transform.tag == "Player")
            {
                Debug.Log("SEE");
                return true;
            }
        }
        return false;
    }

    void HandleChase()
    {
        isChasingPlayer = true;
        domovoyLogic.SetDestination(player.transform.position);

        if (!isPlayerInChaseArea)
        {
            isChasingPlayer=false;
            Debug.Log("stopchase");
        }
    }


    void HandleSoundEvent(MakeSoundEvent e)
    {
        domovoyLogic.SetDestination(e.positionOfSound);
        isChasingSound = true;
    }

    void DrawPath()
    {
        lineRenderer.positionCount = domovoyLogic.path.corners.Length;
        lineRenderer.SetPositions(domovoyLogic.path.corners);
    }

    void HandleHearAreaTrigger(PlayerInHearAreaEvent e)
    {
        isPlayerInHearArea = e.isPlayerIn;
    }

    void HandleChaseAreaTrigger(PlayerInChaseAreaEvent e)
    {
        isPlayerInChaseArea = e.isPlayerIn;
    }


}
