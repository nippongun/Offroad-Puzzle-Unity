using UnityEngine;
using UnityEngine.Animations.Rigging;
using StarterAssets;
using System.Collections;
using Weapons;

public class PlayerAimController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StarterAssetsInputs input;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject aimCamera;
    [SerializeField] private GameObject aimReticle;
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Rig[] aimRigs;
    [SerializeField] private Rig[] mainRigs;
    [SerializeField] private LayerMask aimColliderMask;

    [Header("Aim Settings")]
    [SerializeField][Range(0f, 1f)] private float aimSpeed = 0.2f;
    [SerializeField][Range(0f, 1000f)] private float aimDistance = 100f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;

    private Animator _animator;
    private bool _hasAnimator;
    private int _animIDAiming;
    private ThirdPersonController _thirdPersonController;

    [Header("Equip Settings")]
    private Weapon _currentWeapon;
    private bool isEquipped = true;
    private float equipTime = 0.2f;
    private float lastEquidTime = 0f;

    private void Awake()
    {
        _hasAnimator = TryGetComponent(out _animator);
        AssignAnimationIDs();
        input = GetComponent<StarterAssetsInputs>();
        _thirdPersonController = GetComponent<ThirdPersonController>();
        _currentWeapon = GetComponentInChildren<Weapon>();
        Debug.Log($"PlayerAimController Awake end. Current weapon: {(_currentWeapon != null ? _currentWeapon.name : "null")}");
    }

    private void Update()
    {
        UpdateAimingStatus();
        HandleAiming();
        if (input.reload)
        {
            _currentWeapon.Reload();
        }
        if (input.equip)
        {
            ToggleEquip();
        }
    }

    private void UpdateAimingStatus()
    {
        _hasAnimator = TryGetComponent(out _animator);
    }

    private void HandleAiming()
    {
        Vector3 mouseWorldPosition = GetMouseWorldPosition(out Transform hitTransform);

        if (input.aimValue)
        {
            EnterAimMode(mouseWorldPosition);
            if (input.fire && _currentWeapon != null)
            {
                _currentWeapon.Fire(hitTransform);
            }
        }
        else
        {
            ExitAimMode();
        }
    }
    private Vector3 GetMouseWorldPosition(out Transform hitTransform)
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, aimDistance, aimColliderMask))
        {
            debugTransform.position = raycastHit.point;
            hitTransform = raycastHit.transform;
            return raycastHit.point;
        }
        hitTransform = null;
        return Vector3.zero;
    }

    private void EnterAimMode(Vector3 mouseWorldPosition)
    {
        SetCameraState(false, true);
        _thirdPersonController.SetRotateOnMove(false);
        StartCoroutine(ShowReticle());
        if (_hasAnimator)
        {
            //_animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
        }

        RotateTowardsTarget(mouseWorldPosition);
        UpdateAimRigs(true);
    }

    private void ExitAimMode()
    {
        SetCameraState(true, false);
        aimReticle.SetActive(false);
        //_animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        _thirdPersonController.SetRotateOnMove(true);
        UpdateAimRigs(false);
    }

    private void SetCameraState(bool mainActive, bool aimActive)
    {
        mainCamera.SetActive(mainActive);
        aimCamera.SetActive(aimActive);
    }

    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 worldAimTarget = targetPosition;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotationSpeed);
    }

    private void UpdateAimRigs(bool isAiming)
    {
        if (!isEquipped) return;
        float weightChange = Time.deltaTime / aimSpeed * (isAiming ? 1 : -1);
        foreach (var aimRig in aimRigs)
        {
            aimRig.weight = Mathf.Clamp01(aimRig.weight + weightChange);
        }
    }

    private void ToggleEquip()
    {
        if (Time.time - lastEquidTime < equipTime) return;
        float weightChange = isEquipped ? -1 : 1;
        foreach (var rig in mainRigs)
        {
            rig.weight += weightChange;
        }
        lastEquidTime = Time.time;
        isEquipped = !isEquipped;
    }

    private void AssignAnimationIDs()
    {
        _animIDAiming = Animator.StringToHash("Aiming");
    }

    private IEnumerator ShowReticle()
    {
        yield return new WaitForSeconds(0.25f);
        aimReticle.SetActive(true);
    }
}