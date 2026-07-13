using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    private GameObject turretToBuild;

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
        get { return turretToBuild != null; }
    }

    public void SelectTurretToBuild(GameObject turretPrefab)
    {
        turretToBuild = turretPrefab;
        Debug.Log("Turret selected: " + turretPrefab.name);
    }

    public GameObject GetTurretToBuild()
    {
        return turretToBuild;
    }

    public void ClearTurretSelection()
    {
        turretToBuild = null;
    }
}