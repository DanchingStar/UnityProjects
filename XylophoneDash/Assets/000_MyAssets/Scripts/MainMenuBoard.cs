using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBoard : MonoBehaviour
{
    [SerializeField] private Material[] imageMaterials;

    private MeshRenderer meshRenderer;

    private const int MAX_LENGTH = 5;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        if (imageMaterials.Length != MAX_LENGTH + 1)
        {
            Debug.LogError($"MainMenuBoard.Awake : Error imageMaterials.Length = {imageMaterials.Length}");
        }
    }

    /// <summary>
    /// w’è‚µ‚½”Ô†‚Ì‰æ‘œ‚É•Ï‚¦‚é
    /// </summary>
    /// <param name="number"></param>
    public void ChangeImage(int number)
    {
        if (1 <= number && number <= MAX_LENGTH)
        {
            meshRenderer.material = imageMaterials[number];
        }
        else
        {
            meshRenderer.material = imageMaterials[0];
        }
    }
}
