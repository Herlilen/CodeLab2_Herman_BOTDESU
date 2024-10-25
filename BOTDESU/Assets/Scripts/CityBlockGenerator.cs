using System.Collections.Generic;
using UnityEngine;

public class CityBlockGenerator : MonoBehaviour
{
    public GameObject buildingPrefab;
    public int rows = 5;
    public int columns = 5;
    public float spacing = 10f;
    public Vector2 buildingWidthRange = new Vector2(5f, 10f);
    public Vector2 buildingHeightRange = new Vector2(10f, 20f);
    public int broadWayWidth = 2; // Width of the broad way in terms of number of rows/columns

    void Start()
    {
        GenerateCityBlock();
    }

    void GenerateCityBlock()
    {
        int middleRowStart = (rows - broadWayWidth) / 2;
        int middleRowEnd = middleRowStart + broadWayWidth;
        int middleColumnStart = (columns - broadWayWidth) / 2;
        int middleColumnEnd = middleColumnStart + broadWayWidth;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Skip creating buildings in the broad way area
                if (i >= middleRowStart && i < middleRowEnd)
                {
                    continue;
                }

                Vector3 position = new Vector3(i * spacing, 0, j * spacing);
                CreateBuilding(position);
            }
        }
    }

    void CreateBuilding(Vector3 position)
    {
        GameObject building = Instantiate(buildingPrefab, position, Quaternion.identity);
        float width = Random.Range(buildingWidthRange.x, buildingWidthRange.y);
        float height = Random.Range(buildingHeightRange.x, buildingHeightRange.y);
        building.transform.localScale = new Vector3(width, height, width);

        // Adjust the position to make the building stand on its bottom
        building.transform.position += new Vector3(0, height / 2, 0);

        if (building.GetComponent<BreakCube>() == null)
        {
            building.AddComponent<BreakCube>();
        }
    }
}