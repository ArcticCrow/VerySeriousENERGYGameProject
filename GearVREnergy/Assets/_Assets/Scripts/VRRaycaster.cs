using UnityEngine;
using UnityEngine.Events;

public class VRRaycaster : MonoBehaviour
{

    [System.Serializable]
    public class Callback : UnityEvent<Ray, RaycastHit> { }

    public static VRRaycaster instance;

    public LineRenderer lineRenderer = null;
    public float maxRayDistance = 500.0f;
    public LayerMask excludeLayers;
    public VRRaycaster.Callback raycastHitCallback;

    public Gradient normalGradient;
    public Gradient illuminationGradient;
	public Gradient badIlluminationGradient;

    bool illuminated = false;
	bool badIllumination = false;


    [Tooltip("Should the pointer be hidden when not over interactive objects.")]
    public bool darkenByDefault = true;

    [Tooltip("Time after leaving interactive object before pointer fades.")]
    public float illuminationTimeoutPeriod = 1;

    [Tooltip("Time after mouse pointer becoming inactive before pointer unfades.")]
    public float darkenTimeoutPeriod = 0.1f;
    
    /// <summary>
    /// Last time code requested the pointer be shown. Usually when pointer passes over interactive elements.
    /// </summary>
    private float lastIlluminationRequestTime;
    /// <summary>
    /// Last time pointer was requested to be hidden. Usually mouse pointer activity.
    /// </summary>
    private float lastDarkenRequestTime;

    public float highlightStrength
    {
        get
        {
            // It's possible there are reasons to show the cursor - such as it hovering over some UI - and reasons to hide
            // the cursor - such as another input method (e.g. mouse) being used. We take both of these in to account.


            float strengthFromIlluminationRequest;
            if (darkenByDefault)
            {
                // fade the cursor out with time
                strengthFromIlluminationRequest = Mathf.Clamp01(1 - (Time.time - lastIlluminationRequestTime) / illuminationTimeoutPeriod);
            }
            else
            {
                // keep it fully visible
                strengthFromIlluminationRequest = 1;
            }

            // Now consider factors requesting pointer to be hidden
            float strengthFromDarkenRequest;

            strengthFromDarkenRequest = (lastDarkenRequestTime + darkenTimeoutPeriod > Time.time) ? 0 : 1;


            // Hide requests take priority
            return Mathf.Min(strengthFromIlluminationRequest, strengthFromDarkenRequest);
        }
    }

    void Awake()
    {
        if (instance == null) instance = this;
        if (lineRenderer == null)
        {
            Debug.LogWarning("Assign a line renderer in the inspector!");
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.widthMultiplier = 0.02f;
        }
        lineRenderer.colorGradient = normalGradient;
    }

    void Update()
    {
        Transform pointer = GameManager.instance.Pointer;
        if (pointer == null || !GameManager.instance.pointerIsRemote)
        {
            lineRenderer.enabled = false;
            return;
        }
        else
        {
            lineRenderer.enabled = true;
        }

        Ray laserPointer = new Ray(pointer.position, pointer.forward);

        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, laserPointer.origin);
            lineRenderer.SetPosition(1, laserPointer.origin + laserPointer.direction * maxRayDistance);
        }


        RaycastHit hit;
        if (Physics.Raycast(laserPointer, out hit, maxRayDistance, ~excludeLayers))
        {
            if (lineRenderer != null)
            {
                lineRenderer.SetPosition(1, hit.point);
            }

            if (raycastHitCallback != null)
            {
                raycastHitCallback.Invoke(laserPointer, hit);
            }
        }

        // Should we show or hide the gaze cursor?
        if (highlightStrength == 0 && illuminated)
        {
            Darken();
        }
        else if (highlightStrength > 0 && !illuminated)
        {
            Illuminate();
        }
    }

    public void RequestIllumination()
    {
        Illuminate();
        lastIlluminationRequestTime = Time.time;
    }

	public void RequestBadIllumination()
	{
		badIllumination = true;
		RequestIllumination();
	}

    public void RequestDarken()
    {
        Darken();
        lastDarkenRequestTime = Time.time;
    }

    void Darken()
    {
        lineRenderer.colorGradient = normalGradient;
        illuminated = false;
    }

    void Illuminate()
    {
		if (!badIllumination)
		{
			lineRenderer.colorGradient = illuminationGradient;
		}
		else
		{
			lineRenderer.colorGradient = badIlluminationGradient;
			badIllumination = false;
		}
		illuminated = true;
    }
}