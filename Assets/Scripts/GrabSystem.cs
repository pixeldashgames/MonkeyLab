using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabSystem : MonoBehaviour
{
    private const int UIUpdatesPerSecond = 20;
    private const int GrabSeparationUpdatesPerSecond = 20;

    private const float MaxGrabSeparation = 2f;

    [SerializeField] private Transform grabPoint;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject grabPrompt;
    [SerializeField] private GameObject dropPrompt;
    [SerializeField] private GameObject throwPrompt;

    [SerializeField] private string grabablesTag;
    [SerializeField] private float minThrowForce = 7;
    [SerializeField] private float maxThrowForce = 15;
    [SerializeField] private float timeToChargeThrow = 3;
    [SerializeField] private float grabMoveVelocityScale = 0.8f;
    [SerializeField] private float grabDistance;

    private Rigidbody _grabbedBody;
    private StarterAssetsInput _input;
    private bool _isChargingThrow;
    private bool _isGrabbing;
    private Rigidbody _lookedGrabable;
    private bool _lookingAtGrabable;
    private float _throwChargeStartedTime;

    private void Awake()
    {
        _input = new StarterAssetsInput();
        _input.Enable();

        _input.Player.Interact.performed += InteractOnPerformed;
        _input.Player.PressThrow.performed += PressThrowOnPerformed;
        _input.Player.ReleaseThrow.performed += ReleaseThrowOnPerformed;

        StartCoroutine(CheckForGrabSeparation());
        StartCoroutine(CheckForGrabables());
    }

    private void FixedUpdate()
    {
        if (!_isGrabbing)
            return;

        if (_grabbedBody == null)
        {
            Drop();
            return;
        }

        _grabbedBody.angularVelocity = Vector3.zero;
        _grabbedBody.velocity = (grabPoint.position - _grabbedBody.position) * grabMoveVelocityScale;
    }

    private void ReleaseThrowOnPerformed(InputAction.CallbackContext obj)
    {
        if (!_isGrabbing || !_isChargingThrow)
            return;

        _isChargingThrow = false;
        var ratio = Mathf.Clamp01((Time.time - _throwChargeStartedTime) / timeToChargeThrow);
        ThrowGrabbed(Mathf.Lerp(minThrowForce, maxThrowForce, ratio));
    }

    private void PressThrowOnPerformed(InputAction.CallbackContext obj)
    {
        if (!_isGrabbing)
            return;

        _isChargingThrow = true;
        _throwChargeStartedTime = Time.time;
    }


    private void ThrowGrabbed(float force)
    {
        if (!_isGrabbing)
            return;

        Drop();
        _grabbedBody.AddForce(mainCamera.transform.forward * force, ForceMode.Impulse);
    }

    private void InteractOnPerformed(InputAction.CallbackContext obj)
    {
        if (!_lookingAtGrabable && !_isGrabbing)
            return;

        if (_isGrabbing)
        {
            Drop();
            return;
        }

        _isGrabbing = true;
        _lookingAtGrabable = false;
        _grabbedBody = _lookedGrabable;
    }

    private void Drop()
    {
        _isGrabbing = false;
    }

    private IEnumerator CheckForGrabSeparation()
    {
        while (true)
        {
            if (_isGrabbing)
                if (_grabbedBody == null ||
                    Vector3.Distance(_grabbedBody.position, grabPoint.position)
                    > MaxGrabSeparation)
                    Drop();

            yield return new WaitForSecondsRealtime(1f / GrabSeparationUpdatesPerSecond);
        }
    }

    private IEnumerator CheckForGrabables()
    {
        while (true)
        {
            if (_isGrabbing)
            {
                if (grabPrompt.activeSelf)
                {
                    grabPrompt.SetActive(false);
                    dropPrompt.SetActive(true);
                    throwPrompt.SetActive(true);
                }

                yield return null;
                continue;
            }

            if (dropPrompt.activeSelf)
            {
                dropPrompt.SetActive(false);
                throwPrompt.SetActive(false);
            }

            var ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out var hitInfo, grabDistance))
            {
                if (!string.IsNullOrEmpty(hitInfo.transform.tag) && !hitInfo.transform.CompareTag(grabablesTag))
                {
                    if (_lookingAtGrabable)
                    {
                        grabPrompt.SetActive(false);
                        _lookingAtGrabable = false;
                    }

                    yield return null;
                    continue;
                }

                if (!_lookingAtGrabable)
                {
                    _lookingAtGrabable = true;
                    grabPrompt.SetActive(true);
                }

                _lookedGrabable = hitInfo.rigidbody;
            }
            else if (_lookingAtGrabable)
            {
                grabPrompt.SetActive(false);
                _lookingAtGrabable = false;
            }

            yield return new WaitForSecondsRealtime(1f / UIUpdatesPerSecond);
        }
    }
}