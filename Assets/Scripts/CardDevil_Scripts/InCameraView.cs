using UnityEngine;
using System;

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
    [SerializeField] double distanceToInteract=2;
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

        Vector3 difference = new Vector3(
            camera.transform.position.x - transform.position.x,
            camera.transform.position.y - transform.position.y,
            camera.transform.position.z - transform.position.z);

        double dX = (double)(new decimal(difference.x));
        double dY = (double)(new decimal(difference.y));
        double dZ = (double)(new decimal(difference.z));

        double distance = Math.Sqrt(
            Math.Pow(dX, 2f) +
            Math.Pow(dY, 2f) +
            Math.Pow(dZ, 2f));

        if (GeometryUtility.TestPlanesAABB(cameraFrustum, bounds) && distance<distanceToInteract)
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
            showInteract = GeometryUtility.TestPlanesAABB(cameraFrustum, bounds) && distance < distanceToInteract,
            interactableObject = this.gameObject,
            type = typeOfInteractable
        });
    }
}
