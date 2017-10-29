using UnityEngine;
using System.Collections;

public enum CameraState
{
    Idle = 0,
    ZoomOut,
    ZoomIn
}

/// <summary>
/// Quick camera script to help set resolution for an orthographic camera, it also has some earthquake stuff in there
/// </summary>
public class CameraController : MonoBehaviour
{

    #region Inspector Variables
    /**
     * The target size of the view port.
     */
    public Vector2 targetViewportSizeInPixels = new Vector2(1280.0f, 768.0f);
    /**
     * Snap movement of the camera to pixels.
     */
    //public bool lockToPixels = true;
    /**
     * The number of target pixels in every Unity unit.
     */
    public float pixelsPerUnit = 100.0f;
    public bool pixelperfect = false;
    public bool followObject = true; //Camera tracking
    public GameObject objecttofollow;//Object we want to follow
    public CameraState cameraState; //State camera is in so we know what it's doing ( force it to do something else )

    //Cache camera sizes to store for later use
    [HideInInspector]
    public float camerasize, _cachecamerasize, camerasizeminimum, camerasizemaximum, camerabirdseye;

    //Camera shake intensity ( needs to follow an object in order for it to vibrate unfortunately :( )
    public float shakeDecay = 0.002f, shakeIntensity = 0.3f;
    float cache_shakeIntensity = 0.3f;

    public Vector3 shakePosition, currentVelocity;
    #endregion

    #region Public Variables
    public Transform cacheselfTransform { get; set;} 
    public bool isActive { get; set; }
    public Camera MainCamera
    {
        get
        {
            return _camera;
        }
    }
    #endregion

    #region Private Variables
    private Camera _camera;
    private int _currentScreenWidth = 0;
    private int _currentScreenHeight = 0;
    private float _pixelLockedPPU = 32.0f;
    private Vector2 _winSize;

    //Timers
    Task taskCameraShake;
    #endregion


    #region Unity Functions
    void Awake()
    {
        cacheselfTransform = transform; //Store so we don't have to keep on using the Unity quick reference.
    }

    void Start()
    {
        isActive = true;
        SetCamera();
    }

    void FixedUpdate()
    {
        if (isActive)
        {
            if (followObject)
            {
                FollowObject();
            }
        }
    }

    void LateUpdate()
    {
        //Clamp position to be pixel perfect
        if (followObject)
        {
            if(pixelperfect)
            {
                Vector2 newPosition = new Vector2(cacheselfTransform.position.x, cacheselfTransform.position.y);
                float nextX = Mathf.Round(_pixelLockedPPU * newPosition.x);
                float nextY = Mathf.Round(_pixelLockedPPU * newPosition.y);
                cacheselfTransform.position = new Vector3(nextX / _pixelLockedPPU, nextY / _pixelLockedPPU, transform.position.z);
            }
        }
    }
    #endregion

    #region State Changing
    public void SetCamera()
    {
        _camera = GetComponent<Camera>();
        if(!_camera)
        {
            Debug.LogError("Camera Controller needs to be attached to a camera!");
            return;
        }

        _camera.orthographic = true;

        ResizeCamToTargetSize();
        cameraState = CameraState.Idle;

        //Shake intensities
        cache_shakeIntensity = shakeIntensity;

    }
    #endregion

    #region Custom Functions
    public void SwitchOn()
    {
        isActive = true;
    }

    public void SwitchOff()
    {
        isActive = false;
    }

    /// <summary>
    /// Sets the camera size according to the resolution of the view port
    /// </summary>
    public void ResizeCamToTargetSize()
    {
        if (!_camera)
            return;

        if (_currentScreenWidth != Screen.width || _currentScreenHeight != Screen.height)
        {
            // check our target size here to see how much we want to scale this camera
            float percentageX = Screen.width / targetViewportSizeInPixels.x;
            float percentageY = Screen.height / targetViewportSizeInPixels.y;
            float targetSize = 0.0f;
            if (percentageX > percentageY)
            {
                targetSize = percentageY;
            }
            else
            {
                targetSize = percentageX;
            }
            int floored = Mathf.FloorToInt(targetSize);
            if (floored < 1)
            {
                floored = 1;
            }
            // now we have our percentage let's make the viewport scale to that
            //float camSize = ((Screen.height / 2) / floored) / pixelsPerUnit;
            float camSize = (Screen.height / 2f) / pixelsPerUnit;
            camerasizemaximum = camSize;// / 2f; //(Desired height / 2 ) / Pixeltounits
            camerabirdseye = camerasizemaximum * 2f;
            camerasizeminimum = camerasizemaximum / 4f;
            camerasize = _cachecamerasize = camerasizemaximum;

            _camera.orthographicSize = camerasizemaximum;
            _pixelLockedPPU = floored * pixelsPerUnit;
        }
        _winSize = new Vector2(Screen.width, Screen.height);
    }


    void FollowObject()
    {
        //This script will follow the object according to a min and max range,
        //It will stop scrolling to the left if it goes passed it's minimum range
        //and will stop scrolling to the right it goes passed it's maximum range
        Vector3 newposition = Vector3.zero;
        if (_winSize.x != Screen.width || _winSize.y != Screen.height)
        {
            ResizeCamToTargetSize();
        }

        switch (cameraState)
        {
            default:
                if (_camera) //We have a camera to use
                {
                    Transform transformtofollow = cacheselfTransform; //Set the transform to ourself as default

                    if (followObject && objecttofollow) //We are following an object and we have a valid object to follow
                    {
                        transformtofollow = objecttofollow.transform; //set the default transform to the object we want to follow
                    }


                    if(pixelperfect)
                    {
                        //Pixel perfect
                        Vector2 newPosition = new Vector2(transformtofollow.position.x, transformtofollow.position.y);
                        newPosition += new Vector2(shakePosition.x, shakePosition.y);
                        float nextX = Mathf.Round(_pixelLockedPPU * newPosition.x);
                        float nextY = Mathf.Round(_pixelLockedPPU * newPosition.y);
                        cacheselfTransform.position = new Vector3(nextX / _pixelLockedPPU, nextY / _pixelLockedPPU, cacheselfTransform.transform.position.z);
                    }
                    else
                    {
                        //Not pixel perfect
                        Vector2 newPosition = new Vector2(transformtofollow.position.x, transformtofollow.position.y);
                        newPosition += new Vector2(shakePosition.x, shakePosition.y); //Apply camera shake
                        cacheselfTransform.position = new Vector3(newPosition.x, newPosition.y, cacheselfTransform.position.z);
                    }
                }
                break;
        }
    }

    public void InitiateShake(float shakePower = 0f, float overridedecay = 0f)
    {
        if (taskCameraShake != null)
        {
            StopShake();
        }
        taskCameraShake = new Task(Shake(shakePower, overridedecay));
        //StartCoroutine(Shake(shakePower, overridedecay));
    }

    public void ContinousShake(float shakePower = 0f, float overridedecay = 0f)
    {
        if (taskCameraShake != null)
        {
            StopShake();
        }
        taskCameraShake = new Task(LoopShake(shakePower, overridedecay));
    }

    public void StopShake()
    {
        if (taskCameraShake != null)
        {
            taskCameraShake.Stop();
        }

        shakePosition = Vector3.zero;
        shakeIntensity = 0f;
    }

    public void StopShakeOverTime(float time)
    {
        if (taskCameraShake != null)
        {
            taskCameraShake.Stop();
        }

        //Lerp shake position back to vector zero
        taskCameraShake = new Task(SlowShakeOverTime(time));
    }

    IEnumerator SlowShakeOverTime(float time)
    {
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            shakeIntensity = Mathf.Lerp(shakeIntensity, 0f, (elapsedTime / time));
            shakePosition = Vector3.zero;
            shakePosition = Vector3.zero + Random.insideUnitSphere * shakeIntensity;
            elapsedTime += Time.deltaTime;
            yield return 0;
        }

        shakeIntensity = 0f;
        shakePosition = Vector3.zero;
    }

    public IEnumerator LoopShake(float shakeintensity = 0f, float overridedecay = 0f)
    {
        float shakeint = shakeintensity;
        if (shakeint <= 0)
        {
            shakeint = cache_shakeIntensity;
        }

        float decay = shakeDecay;
        if (overridedecay > 0)
        {
            decay = overridedecay;
        }

        shakeIntensity = shakeint;
        shakePosition = Vector3.zero;
        shakePosition = Vector3.zero + Random.insideUnitSphere * shakeIntensity;
        yield return 0;

        taskCameraShake = new Task(LoopShake(shakeintensity, overridedecay));
    }

    public IEnumerator Shake(float shakeintensity = 0f, float overridedecay = 0f)
    {
        float shakeint = shakeintensity;
        if (shakeint <= 0)
        {
            shakeint = cache_shakeIntensity;
        }

        float decay = shakeDecay;
        if (overridedecay > 0)
        {
            decay = overridedecay;
        }

        shakeIntensity = shakeint;
        shakePosition = Vector3.zero;
        while (shakeIntensity > 0)
        {
            shakePosition = Vector3.zero + Random.insideUnitSphere * shakeIntensity;

            shakeIntensity -= decay;
            yield return 0;
        }

        shakePosition = Vector3.zero;
        shakeIntensity = 0f;
    }

    public void InitiateZoom(float speed, CameraState statetomoveto)
    {
        switch (statetomoveto)
        {
            case CameraState.ZoomIn:
                StartCoroutine(ZoomIn(speed));
                break;
            case CameraState.ZoomOut:
                StartCoroutine(ZoomOut(speed));
                break;
            case CameraState.Idle:
                ResetZoom();
                break;
        }
    }

    void ResetZoom()
    {
        //Reset camera
        cameraState = CameraState.Idle;
        camerasize = _cachecamerasize;
        MainCamera.orthographicSize = _cachecamerasize;
    }

    public IEnumerator ZoomIn(float speed)
    {
        while (camerasize > camerasizeminimum)
        {
            cameraState = CameraState.ZoomIn;
            float nextsize = camerasize - (speed * Time.deltaTime);
            if (nextsize < camerasizeminimum)
            {
                nextsize = camerasizeminimum;
            }
            camerasize = nextsize;
            MainCamera.orthographicSize = camerasize;

            yield return 0;
        }

        cameraState = CameraState.Idle;
    }

    public IEnumerator ZoomOut(float speed, CameraState statetype = CameraState.ZoomOut)
    {
        switch (statetype)
        {
            case CameraState.ZoomOut:
                while (camerasize < camerasizemaximum)
                {
                    cameraState = CameraState.ZoomOut;
                    float nextsize = camerasize + (speed * Time.deltaTime);
                    if (nextsize > camerasizemaximum)
                    {
                        nextsize = camerasizemaximum;
                    }
                    camerasize = nextsize;
                    MainCamera.orthographicSize = camerasize;

                    yield return 0;
                }
                break;
        }
    }
    #endregion
}
