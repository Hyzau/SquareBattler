using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public Material lineMaterial; // Assign a material with a white color in the Inspector.
    public int gridSize = 15;     // Number of cells (grid lines will be one more).
    public float cellSize = 1.0f; // Size of each cell.
    public Color gridColor = Color.white;
    private Vector3 origin;

    public void Start()
    {
        Debug.Log("Start() called.");
        this.origin = transform.position - new Vector3(gridSize * cellSize / 2, gridSize * cellSize / 2, 0);
    }

    /*public void OnDrawGizmos()
    {
        Debug.Log("OnDrawGizmos() called.");
        Gizmos.color = Color.red;

        // Draw vertical lines (for X axis)
        for (int x = 0; x <= gridSize; x++)
        {
            float xPos = this.origin.x + x * cellSize;
            Gizmos.DrawLine(new Vector3(xPos, this.origin.y, 0), new Vector3(xPos, this.origin.y + gridSize * cellSize, 0));
        }

        // Draw horizontal lines (for Y axis)
        for (int y = 0; y <= gridSize; y++)
        {
            float yPos = this.origin.y + y * cellSize;
            Gizmos.DrawLine(new Vector3(this.origin.x, yPos, 0), new Vector3(this.origin.x + gridSize * cellSize, yPos, 0));
        }
    }*/

    public void OnPostRender()
    {
        if (!lineMaterial)
        {
            Debug.LogError("Please assign a material for the grid lines.");
            return;
        }

        // Activate the material
        lineMaterial.SetPass(0);

        GL.Begin(GL.LINES);
        GL.Color(this.gridColor);

        // Draw vertical lines (for X axis)
        for (int x = 0; x <= gridSize; x++)
        {
            float xPos = this.origin.x + x * cellSize;
            GL.Vertex3(xPos, this.origin.y, 0);
            GL.Vertex3(xPos, this.origin.y + gridSize * cellSize, 0);
        }

        // Draw horizontal lines (for Y axis)
        for (int y = 0; y <= gridSize; y++)
        {
            float yPos = this.origin.y + y * cellSize;
            GL.Vertex3(this.origin.x, yPos, 0);
            GL.Vertex3(this.origin.x + gridSize * cellSize, yPos, 0);
        }

        GL.End();
    }

}
