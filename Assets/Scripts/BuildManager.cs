using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    private TurretBlueprint turretToBuild;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one BuildManager in scene!");
            return;
        }

        instance = this;
    }

    public bool CanBuild
    {
        get
        {
            return turretToBuild != null;
        }
    }

    public bool HasMoney
    {
        get
        {
            return turretToBuild != null &&
                   PlayerStats.Money >= turretToBuild.cost;
        }
    }

    public void SelectTurretToBuild(TurretBlueprint turret)
    {
        if (turret == null)
        {
            Debug.LogError("TurretBlueprint is null.");
            return;
        }

        if (turret.prefab == null)
        {
            Debug.LogError(
                "Selected turret blueprint has no prefab assigned."
            );
            return;
        }

        turretToBuild = turret;

        Debug.Log(
            "Turret selected: " +
            turret.prefab.name +
            " | Cost: $" +
            turret.cost
        );
    }

    public TurretBlueprint GetTurretToBuild()
    {
        return turretToBuild;
    }

    public void ClearTurretSelection()
    {
        turretToBuild = null;
    }
}