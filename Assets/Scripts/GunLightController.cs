using UnityEngine;

public class GunLightController : MonoBehaviour
{
    [Header("Light")]
    public Light gunLight;
    public KeyCode toggleKey = KeyCode.F;
    public float revealRange = 18f;
    [Range(5f, 80f)]
    public float revealHalfAngle = 26f;
    public LayerMask obstructionMask = ~0;

    public bool IsOn => gunLight == null || gunLight.enabled;

    private void Start()
    {
        if (gunLight == null)
        {
            gunLight = GetComponentInChildren<Light>();
        }

        ConfigureLight();
        UpdateUI();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        if (Input.GetKeyDown(toggleKey) && gunLight != null)
        {
            gunLight.enabled = !gunLight.enabled;
            UpdateUI();
        }
    }

    public bool IsTargetLit(Transform target)
    {
        if (!IsOn || target == null)
        {
            return false;
        }

        Vector3 origin = transform.position;
        Vector3 toTarget = target.position - origin;
        float distance = toTarget.magnitude;

        if (distance > revealRange)
        {
            return false;
        }

        if (Vector3.Angle(transform.forward, toTarget.normalized) > revealHalfAngle)
        {
            return false;
        }

        if (Physics.Raycast(origin, toTarget.normalized, out RaycastHit hit, distance, obstructionMask, QueryTriggerInteraction.Ignore))
        {
            return hit.transform == target || hit.transform.IsChildOf(target) || target.IsChildOf(hit.transform);
        }

        return true;
    }

    private void ConfigureLight()
    {
        if (gunLight == null)
        {
            return;
        }

        gunLight.type = LightType.Spot;
        gunLight.range = revealRange;
        gunLight.spotAngle = revealHalfAngle * 2f;
        gunLight.intensity = 4.5f;
    }

    private void UpdateUI()
    {
        if (GameManager.Instance != null && GameManager.Instance.uiManager != null)
        {
            GameManager.Instance.uiManager.UpdateLightText(IsOn);
        }
    }
}
