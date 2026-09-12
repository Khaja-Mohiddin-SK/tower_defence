using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FreeTurretPlacement : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public GameObject previewPrefab;
    public TowerPathGenerator pathGenerator;
    public LayerMask placementPlaneLayer;

    [Header("Materials")]
    public Material validMaterial;
    public Material invalidMaterial;

    [Header("Placement Settings")]
    public float gridSize = 1f;
    public float turretHeightOffset = 0f;
    public float pathBlockedDistance = 1.0f;
    public float minDistanceBetweenTurrets = 1.2f;

    private GameObject previewObject;
    private Renderer previewRenderer;

    private bool currentPositionIsValid;
    private Vector3 currentPlacementPosition;

    private readonly List<Vector3> placedTurretPositions =
        new List<Vector3>();

    private GameObject turretBeingMoved;
    private Vector3 originalTurretPosition;

    private bool ignoreNextClickAfterMoveStart = false;
    private Collider movedTurretCollider;

    private int nextTurretNumber = 1;

    void Start()
    {
        CreatePreview();
    }

    void Update()
    {
        UpdatePreviewPosition();

        if (turretBeingMoved != null)
        {
            MoveSelectedTurretWithMouse();
        }

        Mouse mouse = Mouse.current;

        if (mouse == null)
            return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            if (ignoreNextClickAfterMoveStart)
            {
                ignoreNextClickAfterMoveStart = false;
                return;
            }

            if (turretBeingMoved != null)
            {
                TryDropMovedTurret();
            }
            else
            {
                TryPlaceTurret();
            }
        }
    }

    void CreatePreview()
    {
        if (previewPrefab == null)
        {
            Debug.LogError("Preview Prefab is not assigned.");
            return;
        }

        previewObject = Instantiate(previewPrefab);

        previewObject.layer =
            LayerMask.NameToLayer("Ignore Raycast");

        Collider previewCollider =
            previewObject.GetComponent<Collider>();

        if (previewCollider != null)
        {
            previewCollider.enabled = false;
        }

        previewRenderer =
            previewObject.GetComponent<Renderer>();

        previewObject.SetActive(false);
    }

    void UpdatePreviewPosition()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null || mainCamera == null)
            return;

        // If we are not moving a turret and nothing is selected,
        // hide the placement preview.
        if (turretBeingMoved == null)
        {
            if (BuildManager.instance == null ||
                !BuildManager.instance.CanBuild)
            {
                currentPositionIsValid = false;

                if (previewObject != null)
                {
                    previewObject.SetActive(false);
                }

                return;
            }
        }

        Vector2 mousePosition =
            mouse.position.ReadValue();

        Ray ray =
            mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            1000f,
            placementPlaneLayer.value))
        {
            currentPlacementPosition = hit.point;

            currentPlacementPosition.y +=
                turretHeightOffset;

            if (gridSize > 0f)
            {
                currentPlacementPosition.x =
                    Mathf.Round(
                        currentPlacementPosition.x /
                        gridSize
                    ) * gridSize;

                currentPlacementPosition.z =
                    Mathf.Round(
                        currentPlacementPosition.z /
                        gridSize
                    ) * gridSize;
            }

            currentPositionIsValid =
                IsValidPlacement(currentPlacementPosition);

            bool canAffordSelectedTurret = true;

            // Moving an existing turret costs nothing.
            if (turretBeingMoved == null)
            {
                canAffordSelectedTurret =
                    BuildManager.instance.HasMoney;
            }

            if (previewObject != null)
            {
                previewObject.SetActive(true);

                previewObject.transform.position =
                    currentPlacementPosition;
            }

            if (previewRenderer != null)
            {
                bool canPlace =
                    currentPositionIsValid &&
                    canAffordSelectedTurret;

                previewRenderer.material =
                    canPlace
                        ? validMaterial
                        : invalidMaterial;
            }
        }
        else
        {
            currentPositionIsValid = false;

            if (previewObject != null)
            {
                previewObject.SetActive(false);
            }
        }
    }

    void TryPlaceTurret()
    {
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (BuildManager.instance == null)
            return;

        TurretBlueprint blueprint =
            BuildManager.instance.GetTurretToBuild();

        if (blueprint == null)
            return;

        if (blueprint.prefab == null)
        {
            Debug.LogError(
                "Selected turret has no prefab assigned."
            );

            return;
        }

        if (!currentPositionIsValid)
            return;

        if (PlayerStats.Money < blueprint.cost)
        {
            Debug.Log(
                "Not enough money to build " +
                blueprint.prefab.name
            );

            return;
        }

        PlayerStats.Money -= blueprint.cost;

        GameObject newTurret = Instantiate(
            blueprint.prefab,
            currentPlacementPosition,
            Quaternion.identity
        );

        MoveTurret moveTurret =
            newTurret.GetComponent<MoveTurret>();

        if (moveTurret != null)
        {
            moveTurret.SetPlacementManager(this);
            moveTurret.SetTurretNumber(nextTurretNumber);
        }

        placedTurretPositions.Add(
            currentPlacementPosition
        );

        Debug.Log(
            "Turret " +
            nextTurretNumber +
            " placed for $" +
            blueprint.cost +
            ". Remaining money: $" +
            PlayerStats.Money
        );

        nextTurretNumber++;
    }

    bool IsValidPlacement(Vector3 position)
    {
        if (pathGenerator == null)
            return false;

        if (pathGenerator.IsNearPath(
            position,
            pathBlockedDistance))
        {
            return false;
        }

        foreach (Vector3 placedPosition
                 in placedTurretPositions)
        {
            Vector3 flatPosition =
                new Vector3(
                    position.x,
                    0f,
                    position.z
                );

            Vector3 flatPlacedPosition =
                new Vector3(
                    placedPosition.x,
                    0f,
                    placedPosition.z
                );

            float distance =
                Vector3.Distance(
                    flatPosition,
                    flatPlacedPosition
                );

            if (distance <
                minDistanceBetweenTurrets)
            {
                return false;
            }
        }

        return true;
    }

    public void StartMovingTurret(GameObject turret)
    {
        if (turretBeingMoved != null)
            return;

        turretBeingMoved = turret;

        originalTurretPosition =
            turret.transform.position;

        ignoreNextClickAfterMoveStart = true;

        RemovePlacedTurretPosition(
            originalTurretPosition
        );

        movedTurretCollider =
            turretBeingMoved.GetComponent<Collider>();

        if (movedTurretCollider != null)
        {
            movedTurretCollider.enabled = false;
        }

        Turret turretScript =
            turretBeingMoved.GetComponent<Turret>();

        if (turretScript != null)
        {
            turretScript.enabled = false;
        }

        MoveTurret moveTurret =
            turretBeingMoved.GetComponent<MoveTurret>();

        if (moveTurret != null)
        {
            Debug.Log(
                "Turret " +
                moveTurret.turretNumber +
                " selected for moving."
            );
        }
    }

    void MoveSelectedTurretWithMouse()
    {
        if (turretBeingMoved == null)
            return;

        turretBeingMoved.transform.position =
            currentPlacementPosition;
    }

    void TryDropMovedTurret()
    {
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        MoveTurret moveTurret =
            turretBeingMoved.GetComponent<MoveTurret>();

        int turretNumber =
            moveTurret != null
                ? moveTurret.turretNumber
                : 0;

        if (!currentPositionIsValid)
        {
            turretBeingMoved.transform.position =
                originalTurretPosition;

            placedTurretPositions.Add(
                originalTurretPosition
            );

            EnableMovedTurretAgain();

            turretBeingMoved = null;
            movedTurretCollider = null;

            if (turretNumber > 0)
            {
                Debug.Log(
                    "Turret " +
                    turretNumber +
                    " returned to original position."
                );
            }

            return;
        }

        turretBeingMoved.transform.position =
            currentPlacementPosition;

        placedTurretPositions.Add(
            currentPlacementPosition
        );

        EnableMovedTurretAgain();

        turretBeingMoved = null;
        movedTurretCollider = null;

        if (turretNumber > 0)
        {
            Debug.Log(
                "Turret " +
                turretNumber +
                " moved."
            );
        }
    }

    void EnableMovedTurretAgain()
    {
        if (movedTurretCollider != null)
        {
            movedTurretCollider.enabled = true;
        }

        if (turretBeingMoved != null)
        {
            Turret turretScript =
                turretBeingMoved.GetComponent<Turret>();

            if (turretScript != null)
            {
                turretScript.enabled = true;
            }
        }
    }

    void RemovePlacedTurretPosition(
        Vector3 position)
    {
        for (int i =
                 placedTurretPositions.Count - 1;
             i >= 0;
             i--)
        {
            Vector3 flatPosition =
                new Vector3(
                    position.x,
                    0f,
                    position.z
                );

            Vector3 flatPlacedPosition =
                new Vector3(
                    placedTurretPositions[i].x,
                    0f,
                    placedTurretPositions[i].z
                );

            float distance =
                Vector3.Distance(
                    flatPosition,
                    flatPlacedPosition
                );

            if (distance < 0.1f)
            {
                placedTurretPositions.RemoveAt(i);
                return;
            }
        }
    }
}