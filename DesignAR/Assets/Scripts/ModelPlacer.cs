using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARPlacementController : MonoBehaviour
{
    [Header("References")]
    public GameObject placementIndicatorPrefab;   // small indicator prefab
    public GameObject objectToPlacePrefab;        // if using editor-imported prefab

    private ARRaycastManager raycastManager;
    private GameObject placementIndicator;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    void Start()
    {
        if (placementIndicatorPrefab != null)
        {
            placementIndicator = Instantiate(placementIndicatorPrefab);
            placementIndicator.SetActive(false);
        }
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        // single touch to place
        if (placementPoseIsValid && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                PlaceObject();
            }
        }
    }

    void UpdatePlacementPose()
    {
        // we'll use raycast from screen center for indicator
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        hits.Clear();
        raycastManager.Raycast(screenCenter, hits, TrackableType.Planes | TrackableType.FeaturePoint);
        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            // Align model with camera forward (so it faces user)
            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    void UpdatePlacementIndicator()
    {
        if (placementIndicator == null) return;

        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    void PlaceObject()
    {
        GameObject prefabToUse = objectToPlacePrefab;

        if (prefabToUse == null)
        {
            Debug.LogWarning("No prefab assigned to place.");
            return;
        }

        Instantiate(prefabToUse, placementPose.position, placementPose.rotation);
    }
}