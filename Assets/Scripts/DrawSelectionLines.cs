using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSelectionLines : MonoBehaviour
{
    public List<Vector2> points = new List<Vector2>();

    public Material lineMaterial;

    [HideInInspector]
    public bool completeSelection = false;

    public ImageOperations activeImageOperations;
    private Vector2 mousePoint;

    void Update()
    {
        mousePoint = Input.mousePosition;
    }

    void DrawSelection()
    {
        if (points.Count > 1)
        {
            for(int i = 0; i<points.Count-1; i++)
            {
                Vector3 point1 = new Vector3(points[i].x,points[i].y,-10f);
                Vector3 point2 = new Vector3(points[i + 1].x, points[i + 1].y, -10f); ;

                GL.Begin(GL.LINES);
                GL.Color(new Color(lineMaterial.color.r, lineMaterial.color.g, lineMaterial.color.b, lineMaterial.color.a));
                GL.Vertex3(point1.x, point1.y, point1.z);
                GL.Vertex3(point2.x, point2.y, point2.z);
                GL.End();
            }

            Vector3 selectionEnd = new Vector3(points[points.Count-1].x, points[points.Count-1].y, -10f);
            Vector3 movingPoint = new Vector3(mousePoint.x, mousePoint.y, -10f); ;

            GL.Begin(GL.LINES);
            GL.Color(new Color(lineMaterial.color.r, lineMaterial.color.g, lineMaterial.color.b, lineMaterial.color.a));
            GL.Vertex3(selectionEnd.x, selectionEnd.y, selectionEnd.z);
            GL.Vertex3(movingPoint.x, movingPoint.y, movingPoint.z);
            GL.End();

            if (completeSelection)
            {
                GL.Begin(GL.LINES);
                GL.Color(new Color(lineMaterial.color.r, lineMaterial.color.g, lineMaterial.color.b, lineMaterial.color.a));
                GL.Vertex3(points[points.Count-1].x, points[points.Count - 1].y, -10f);
                GL.Vertex3(points[0].x, points[0].y, -10f);
                GL.End();
            }
        }
    }

    void OnPostRender()
    {
        DrawSelection();
    }

    void OnDrawGizmos()
    {
        DrawSelection();
    }

}
