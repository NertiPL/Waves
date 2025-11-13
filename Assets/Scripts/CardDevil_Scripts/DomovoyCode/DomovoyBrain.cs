using UnityEngine;
using UnityEngine.AI;

public class DomovoyBrain : MonoBehaviour
{
    [Header("Domovoy")]

    [Header("Actions")]
    public bool isRoaming=false;
    public bool isChasingSound=false;
    public bool isChasingPlayer=false;

    [Header("Player")]
    [SerializeField] GameObject player;

    [Header("Values")]
    [SerializeField] int moveSpeed;
    [SerializeField] int rotationSpeed;

    Transform myTransform;
    NavMeshAgent domovoyLogic;
    LineRenderer lineRenderer;

    EventBinding<MakeSoundEvent> soundEventBinding;

    private void OnEnable()
    {

        soundEventBinding = new EventBinding<MakeSoundEvent>(HandleSoundEvent);
        EventBus<MakeSoundEvent>.Register(soundEventBinding);
    }

    private void OnDisable()
    {
        EventBus<MakeSoundEvent>.Deregister(soundEventBinding);
    }

    void Awake()
    {
        myTransform = transform;
    }

    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        domovoyLogic = GetComponent<NavMeshAgent>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

    }

    private void Update()
    {
        //domovoyLogic.SetDestination(player.transform.position);

        if (domovoyLogic.hasPath)
        {
            DrawPath();
        }
        else
        {
            lineRenderer.positionCount = 0; 
        }
    }


    void HandleSoundEvent()
    {
        domovoyLogic.SetDestination(player.transform.position);
    }

    void DrawPath()
    {
        lineRenderer.positionCount = domovoyLogic.path.corners.Length;
        lineRenderer.SetPositions(domovoyLogic.path.corners);
    }


}
