using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class LOSSensor : MonoBehaviour
{
    public float Distance = 10f;
    public float Angle = 30;
    public float Height = 1.0f;
    public Color MeshColor = Color.cyan;
    public LayerMask InclusionLayers;
    public LayerMask ExclusionLayers;
    private Mesh mesh;
    private PlayerController player;

    private void Start()
    {
        player = PlayerController.Instance;
    }

    private void OnValidate()
    {
        if (Application.isEditor)
        {
            mesh = CreateWedgeMesh();
        }
        
    }

    private void OnDrawGizmos()
    {
        if (Application.isEditor)
        {
            if (mesh)
            {
                Gizmos.color = MeshColor;
                Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
            }
        }       
    }

    private void LateUpdate()
    {
        transform.localRotation = Quaternion.identity;
    }

    public bool IsPlayerInSight()
    {
        Vector3 origin = transform.position;
        Vector3 dest = player.transform.position;
        Vector3 dir = dest - origin;
        if (dir.y < 0 || dir.y > Height)
        {
            return false;
        }
        dir.y = 0;
        float deltaAngle = Vector3.Angle(dir, transform.forward);
        if (deltaAngle > Angle)
        {
            return false;
        }
        origin.y += Height / 2;
        dest.y = origin.y;
        if (Physics.Linecast(origin, dest, ExclusionLayers))
        {
            return false;
        }
        return true;
    }

    private Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();
        int segments = 10;
        int numTriangle = (segments * 4) + 2 + 2;
        int numVertices = numTriangle * 3;
        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -Angle, 0) * Vector3.forward * Distance;
        Vector3 bottomRight = Quaternion.Euler(0, Angle, 0) * Vector3.forward * Distance;

        Vector3 topCenter = bottomCenter + Vector3.up * Height;
        Vector3 topLeft = bottomLeft + Vector3.up * Height;
        Vector3 topRight = bottomRight + Vector3.up * Height;

        int vert = 0;

        //left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        //right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -Angle;
        float deltaAngle = (Angle * 2) / segments;
        for (int i = 0; i < segments; ++i)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * Distance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * Distance;

            topLeft = bottomLeft + Vector3.up * Height;
            topRight = bottomRight + Vector3.up * Height;

            //far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            //top side
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            //bottom side
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }

        

        for (int i = 0; i < numVertices; ++i)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
}
