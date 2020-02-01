using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[SelectionBase]
//[RequireComponent(typeof(Waypoint))]
public class CubeEditor : MonoBehaviour
{
    enum Type
    {
        GROUND,
        PILLAR,
        WALL,
        TURRET,
        NPC,
        PLAYER,
        CORE
    }

    [SerializeField]
    Type             TYPE;

    TextMesh        _textMesh;

    private void Awake()
    {
        _textMesh = GetComponentInChildren<TextMesh>();
    }

    void Update()
    {
        float xGrid, yGrid;
        SnapToGrid(out xGrid, out yGrid);
        UpdateLabel(xGrid, yGrid);
    }

    private void SnapToGrid(out float xGrid, out float yGrid)
    {
        Vector3 position = transform.position;

        xGrid = Mathf.RoundToInt(position.x / ConstantHolder.GRID_SIZE);
        float xSnapPosition = xGrid * ConstantHolder.GRID_SIZE;

        yGrid = Mathf.RoundToInt(position.z / ConstantHolder.GRID_SIZE);
        float zSnapPosition = yGrid * ConstantHolder.GRID_SIZE;

        transform.position = new Vector3(xSnapPosition, 0, zSnapPosition);
    }

    private void UpdateLabel(float xGrid, float yGrid)
    {
        if (TYPE == Type.GROUND)
        {
            string labelText = xGrid + "," + yGrid;
            _textMesh.text = labelText;
            gameObject.name = labelText;
        }
    }
}