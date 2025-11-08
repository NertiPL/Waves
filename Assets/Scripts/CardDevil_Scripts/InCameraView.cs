using UnityEngine;

public enum TypesOfInteractables
{
    FlashBomb,
    Door,
    Jar
}
public class InCameraView : MonoBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] TypesOfInteractables typeOfInteractable;
    Plane[] cameraFrustum;
    Bounds bounds;
    float shrinkFactor = 0.33f;


    private void Awake()
    {
        bounds = GetComponent<Collider>().bounds;
    }

    private void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Camera>();
    }

    private void FixedUpdate()
    {
        Matrix4x4 customProj = Matrix4x4.Perspective(
            camera.fieldOfView * shrinkFactor,
            camera.aspect,
            camera.nearClipPlane,
            camera.farClipPlane
        );

        Matrix4x4 viewMatrix = camera.worldToCameraMatrix;
        Matrix4x4 customVP = customProj * viewMatrix;
        cameraFrustum = GeometryUtility.CalculateFrustumPlanes(customVP);

        if(GeometryUtility.TestPlanesAABB(cameraFrustum, bounds))
        {
            transform.GetComponent<Outline>().enabled = true;
        }
        else
        {
            transform.GetComponent<Outline>().enabled = false;
        }
        //info goes to UI and Player. UI shows popup and player waits for an interaciton to remove bomb object if it is it
        EventBus<InteractEvent>.Raise(new InteractEvent
        {
            showInteract = GeometryUtility.TestPlanesAABB(cameraFrustum, bounds),
            interactableObject = this.gameObject,
            type = typeOfInteractable
        });
    }
}
