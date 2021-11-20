using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBackground : MonoBehaviour
{
    [SerializeField] private Transform BackgroundImage;
    [SerializeField] private InventorySystem inventorySystem;
    private GridLayoutGroup gridLayout;

    private void Awake()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
    }

    private void Start()
    {
        CreateGridBackGround();
    }

    private void CreateGridBackGround()
    {
        for (int x = 0; x < inventorySystem.GetWidth(); x++)
        {
            for (int y = 0; y < inventorySystem.GetHeight(); y++)
            {
                Transform backgrnd = Instantiate(BackgroundImage, gridLayout.gameObject.transform);
                backgrnd.gameObject.SetActive(true);
            }
        }
    }
}
