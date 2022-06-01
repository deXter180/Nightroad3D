using UnityEngine;
using System.Collections;
using UnityEditor;

[ExecuteInEditMode]
public class Crosshair : Singleton<Crosshair>
{
    [System.Serializable]
    public class CrosshairProperties
    {
        [Tooltip("Height of Crosshair")] public float Height = 12f;
        [Tooltip("Width of Crosshair")] public float Width = 2f;
        [Tooltip("Default size of spread")] public float DefaultSpread = 25f;
        [Tooltip("Color of Crosshair")] public Color CrosshairColor = Color.green;
        [Tooltip("Is Crosshair dynamic")] public bool Resizeable = true;
        [Tooltip("Maximum size of spread")] public float ResizedSpread = 50f;
        [Tooltip("Spreading Speed")] public float SpreadSpeed = 3f;
        [Tooltip("Suppressing Speed")] public float ResizeSpeed = 3f;
    }  

    [SerializeField] private CrosshairProperties crosshairProperties;
    private CrosshairProperties defaultCrosshair;
    private float spread;
    private bool resizing = false;   
    private bool isReady = false;


    protected override void Awake()
    {
        base.Awake();
        defaultCrosshair = crosshairProperties;
    }

    private void Update()
    {
        if (crosshairProperties != null)
        {
            if (isReady)
            {
                if (crosshairProperties.Resizeable)
                {
                    if (resizing)
                    {
                        spread = Mathf.Lerp(spread, crosshairProperties.ResizedSpread, crosshairProperties.SpreadSpeed * Time.deltaTime);
                    }
                    else
                    {
                        spread = Mathf.Lerp(spread, crosshairProperties.DefaultSpread, crosshairProperties.ResizeSpeed * Time.deltaTime);
                    }
                    spread = Mathf.Clamp(spread, crosshairProperties.DefaultSpread, crosshairProperties.ResizedSpread);
                }
            }
            else
            {
                spread = Mathf.Clamp(spread, crosshairProperties.DefaultSpread, crosshairProperties.ResizedSpread);
            }
        }       
    }

    private void OnGUI()
    {
        if (crosshairProperties != null)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, crosshairProperties.CrosshairColor);
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.Apply();

            //up rect
            GUI.DrawTexture(new Rect(Screen.width / 2 - crosshairProperties.Width / 2, (Screen.height / 2 - crosshairProperties.Height / 2) + spread / 2, crosshairProperties.Width, crosshairProperties.Height), texture);

            //down rect
            GUI.DrawTexture(new Rect(Screen.width / 2 - crosshairProperties.Width / 2, (Screen.height / 2 - crosshairProperties.Height / 2) - spread / 2, crosshairProperties.Width, crosshairProperties.Height), texture);

            //left rect
            GUI.DrawTexture(new Rect((Screen.width / 2 - crosshairProperties.Height / 2) + spread / 2, Screen.height / 2 - crosshairProperties.Width / 2, crosshairProperties.Height, crosshairProperties.Width), texture);

            //right rect
            GUI.DrawTexture(new Rect((Screen.width / 2 - crosshairProperties.Height / 2) - spread / 2, Screen.height / 2 - crosshairProperties.Width / 2, crosshairProperties.Height, crosshairProperties.Width), texture);
        }       
    }

    public IEnumerator SetupCrosshair(CrosshairProperties properties)
    {
        yield return Helpers.GetWait(0.1f);
        isReady = true;
        crosshairProperties = properties;
        spread = crosshairProperties.DefaultSpread;
    }

    public void RemoveCrosshair()
    {
        isReady = false;
        crosshairProperties = defaultCrosshair;
    }

    public void SetRisizing(bool state)
    {        
        resizing = state;      
    }
}
