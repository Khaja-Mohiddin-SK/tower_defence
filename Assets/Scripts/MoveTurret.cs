using UnityEngine;
using UnityEngine.EventSystems;

public class MoveTurret : MonoBehaviour
{
    private FreeTurretPlacement placementManager;

    public int turretNumber;

    public void SetPlacementManager(FreeTurretPlacement manager)
    {
        placementManager = manager;
    }

    public void SetTurretNumber(int number)
    {
        turretNumber = number;
    }

    void Start()
    {
        if (placementManager == null)
        {
            placementManager = Object.FindFirstObjectByType<FreeTurretPlacement>();
        }
    }

    void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (placementManager != null)
        {
            placementManager.StartMovingTurret(gameObject);
        }
    }
}