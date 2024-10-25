using System.Collections.Generic;
using UnityEngine;

public class BreakCube : MonoBehaviour
{
    private float explosionForce = 250f;
    private float explosionRadius = 100f;
    private float explosionForceVariation = 1000f;
    private float explosionRadiusVariation = 300f;
    private int piecesPerAxisX;
    private int piecesPerAxisY;
    private int piecesPerAxisZ;
    private float pieceSizeMultiplier = 10f; // Adjust this value to change the piece size

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
        {
            BreakIntoPieces();
        }
    }

    void BreakIntoPieces()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) return;

        Vector3 originalSize = GetComponent<Renderer>().bounds.size;
        piecesPerAxisX = Mathf.RoundToInt(originalSize.x / pieceSizeMultiplier);
        piecesPerAxisY = Mathf.RoundToInt(originalSize.y / pieceSizeMultiplier);
        piecesPerAxisZ = Mathf.RoundToInt(originalSize.z / pieceSizeMultiplier);

        Vector3 pieceSize = new Vector3(
            originalSize.x / piecesPerAxisX,
            originalSize.y / piecesPerAxisY,
            originalSize.z / piecesPerAxisZ
        );

        for (int x = 0; x < piecesPerAxisX; x++)
        {
            for (int y = 0; y < piecesPerAxisY; y++)
            {
                for (int z = 0; z < piecesPerAxisZ; z++)
                {
                    CreatePiece(Vector3.Scale(new Vector3(x, y, z), pieceSize) - originalSize / 2 + pieceSize / 2, pieceSize);
                }
            }
        }

        Destroy(gameObject);
    }

    void CreatePiece(Vector3 position, Vector3 size)
    {
        GameObject pieceObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pieceObject.transform.position = transform.position + position;
        pieceObject.transform.rotation = transform.rotation;
        pieceObject.transform.localScale = size;

        MeshRenderer pieceMeshRenderer = pieceObject.GetComponent<MeshRenderer>();
        pieceMeshRenderer.material = GetComponent<MeshRenderer>().material;

        Rigidbody rb = pieceObject.AddComponent<Rigidbody>();
        MeshCollider meshCollider = pieceObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;

        float randomExplosionForce = explosionForce + Random.Range(500, explosionForceVariation);
        float randomExplosionRadius = explosionRadius + Random.Range(200, explosionRadiusVariation);

        rb.AddExplosionForce(randomExplosionForce, transform.position, randomExplosionRadius);

        Destroy(pieceObject, 30f); // Destroy the piece after 30 seconds
    }
}