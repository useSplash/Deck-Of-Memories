using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JumpingText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float jumpHeight;
    public float speed;

    void Update()
    {
        textMesh.ForceMeshUpdate();
        var textInfo = textMesh.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;

            // Calculate Y offset using sine wave for smooth jumping effect
            float offsetY = Mathf.Sin(Time.time * -speed + i) * jumpHeight;

            // Move each vertex of the character up and down
            vertices[vertexIndex + 0].y += offsetY;
            vertices[vertexIndex + 1].y += offsetY;
            vertices[vertexIndex + 2].y += offsetY;
            vertices[vertexIndex + 3].y += offsetY;
        }

        // Update mesh
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMesh.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}