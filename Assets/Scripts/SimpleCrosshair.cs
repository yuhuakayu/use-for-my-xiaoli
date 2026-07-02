using UnityEngine;
using UnityEngine.UI;

public class SimpleCrosshair : MonoBehaviour
{
    public Image crosshairImage;
    public Color normalColor = Color.white;
    public Color litMonsterColor = Color.red;
    public float checkRange = 35f;
    public GunLightController gunLight;

    private Camera aimCamera;

    private void Start()
    {
        aimCamera = Camera.main;

        if (gunLight == null)
        {
            gunLight = FindObjectOfType<GunLightController>();
        }
    }

    private void Update()
    {
        if (crosshairImage == null || aimCamera == null)
        {
            return;
        }

        bool hasLitMonster = false;
        Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, checkRange, ~0, QueryTriggerInteraction.Ignore))
        {
            FogMonster monster = hit.collider.GetComponentInParent<FogMonster>();
            hasLitMonster = monster != null && gunLight != null && gunLight.IsTargetLit(monster.RevealPoint);
        }

        crosshairImage.color = hasLitMonster ? litMonsterColor : normalColor;
    }
}
