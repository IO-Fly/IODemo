using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UGUIMiniMap;

public class bl_MiniMapItem : MonoBehaviour {

    [Separator("TARGET")]
    [Tooltip("UI Prefab")]
    public GameObject GraphicPrefab = null;
    [Tooltip("Transform to UI Icon will be follow")]
    public Transform Target = null;
    [Tooltip("Custom Position from target position")]
    public Vector3 OffSet = Vector3.zero;

    [Separator("ICON")]
    public Sprite Icon = null;
    public Sprite DeathIcon = null;
    public Color IconColor = new Color(1, 1, 1, 0.9f);
    [Range(1,100)]public float Size = 20;

    [Separator("CIRCLE AREA")]
    public bool ShowCircleArea = false;
    [Range(1,100)]public float CircleAreaRadius = 10;
    public Color CircleAreaColor = new Color(1, 1, 1, 0.9f);

    [Separator("ICON BUTTON")]
    [Tooltip("UI can interact when press it?")]
    [CustomToggle("is Interact-able")]
    public bool isInteractable = true;
    [TextArea(2,2)]public string InfoItem = "Info Icon here";

    [Separator("SETTINGS")]
    [Tooltip("Can Icon show when is off screen?")]
    [CustomToggle("Off Screen")]
    public bool OffScreen = true;
    [CustomToggle("Destroy Icon with object")] public bool DestroyWithObject = false;
    [Range(0,5)]public float BorderOffScreen = 0.01f;
    [Range(1,50)]public float OffScreenSize = 10;
    [CustomToggle("isHoofdPunt")]
    public bool isHoofdPunt = false;
    [Tooltip("Time before render/show item in minimap after instance")]
    [Range(0,3)]public float RenderDelay = 0.3f;
    public ItemEffect m_Effect = ItemEffect.None;
    //Privates
    private Image Graphic = null;
    private RectTransform RectRoot;
    private GameObject cacheItem = null;
    private RectTransform CircleAreaRect = null;
    private Vector3 position;
    private bl_MiniMap MiniMap;

    /// <summary>
    /// Get all required component in start
    /// </summary>
    void Start()
    {
        MiniMap = bl_MiniMapUtils.GetMiniMap();
        if (bl_MiniMap.MapUIRoot != null)
        {
            CreateIcon();
        }
        else { Debug.Log("You need a MiniMap in scene for use MiniMap Items."); }
    }

    /// <summary>
    /// 
    /// </summary>
    void CreateIcon()
    {
        //Instantiate UI in canvas
        cacheItem = Instantiate(GraphicPrefab) as GameObject;
        RectRoot = bl_MiniMapUtils.GetMiniMap().MMUIRoot;
        //SetUp Icon UI
        Graphic = cacheItem.GetComponent<Image>();
        if (Icon != null) { Graphic.sprite = Icon; Graphic.color = IconColor; }
        cacheItem.transform.SetParent(RectRoot.transform, false);
        Graphic.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        cacheItem.GetComponent<CanvasGroup>().interactable = isInteractable;
        if (Target == null) { Target = this.GetComponent<Transform>(); }
        StartEffect();
        bl_IconItem ii = cacheItem.GetComponent<bl_IconItem>();
        ii.DelayStart(RenderDelay);
        ii.GetInfoItem(InfoItem);
        if (ShowCircleArea)
        {
           CircleAreaRect = ii.SetCircleArea(CircleAreaRadius, CircleAreaColor);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        //If a component missing, return for avoid bugs.
        if (Target == null)
            return;
        if (Graphic == null)
            return;

        if (isHoofdPunt)
        {
            if (MiniMap.Target != null)
            {
                transform.position = MiniMap.Target.TransformPoint((MiniMap.Target.forward) * 100);
            }
        }
        //Get the Rect of Target UI
        RectTransform rt = Graphic.GetComponent<RectTransform>();
        //Setting the modify position
        Vector3 CorrectPosition = TargetPosition + OffSet;
        //Convert the position of target in ViewPortPoint
        Vector2 vp2 = bl_MiniMap.MiniMapCamera.WorldToViewportPoint(CorrectPosition);
        //Calculate the position of target and convert into position of screen
        position = new Vector2((vp2.x * RectRoot.sizeDelta.x) - (RectRoot.sizeDelta.x * 0.5f),
            (vp2.y * RectRoot.sizeDelta.y) - (RectRoot.sizeDelta.y * 0.5f));

        Vector2 UnClampPosition = position;
        //if show off screen
        if (OffScreen)
        {
            //Calculate the max and min distance to move the UI
            //this clamp in the RectRoot sizeDela for border
            position.x = Mathf.Clamp(position.x, -((RectRoot.sizeDelta.x * 0.5f) - BorderOffScreen), ((RectRoot.sizeDelta.x * 0.5f) - BorderOffScreen));
            position.y = Mathf.Clamp(position.y, -((RectRoot.sizeDelta.y * 0.5f) - BorderOffScreen), ((RectRoot.sizeDelta.y * 0.5f) - BorderOffScreen));
        }
        
        //calculate the position of UI again, determine if off screen
        //if off screen reduce the size
        float size = Size;
        //Use this (useCompassRotation when have a circle miniMap)
        if (m_miniMap.useCompassRotation)
        {
            //Compass Rotation
            Vector3 screenPos = Vector3.zero;
            //Calculate difference
            Vector3 forward = Target.position - m_miniMap.TargetPosition;
            //Position of target from camera
            Vector3 cameraRelativeDir = bl_MiniMap.MiniMapCamera.transform.InverseTransformDirection(forward);
            //normalize values for screen fix
            cameraRelativeDir.z = 0;
            cameraRelativeDir = cameraRelativeDir.normalized / 2;
           //Convert values to positive for calculate area OnScreen and OffScreen.
            float posPositiveX = Mathf.Abs(position.x);
            float relativePositiveX = Mathf.Abs((0.5f + (cameraRelativeDir.x * m_miniMap.CompassSize)));          
            //when target if offScreen clamp position in circle area.
            if (posPositiveX >= relativePositiveX)
            {
                screenPos.x = 0.5f + (cameraRelativeDir.x * m_miniMap.CompassSize)/*/ Camera.main.aspect*/;
                screenPos.y = 0.5f + (cameraRelativeDir.y * m_miniMap.CompassSize);
                position = screenPos;
                size = OffScreenSize;
            }
            else
            {
                size = Size;
            }
        }
        else
        {
            if (position.x == (RectRoot.sizeDelta.x * 0.5f) - BorderOffScreen || position.y == (RectRoot.sizeDelta.y * 0.5f) - BorderOffScreen ||
                position.x == -(RectRoot.sizeDelta.x * 0.5f) - BorderOffScreen || -position.y == (RectRoot.sizeDelta.y * 0.5f) - BorderOffScreen)
            {
                size = OffScreenSize;
            }
            else
            {
                size = Size;
            }
        }
        //Apply position to the UI (for follow)
        rt.anchoredPosition = position;
        if(CircleAreaRect != null) { CircleAreaRect.anchoredPosition = UnClampPosition; }
        //Change size with smooth transition
        float CorrectSize = size * MiniMap.IconMultiplier;
        rt.sizeDelta = Vector2.Lerp(rt.sizeDelta, new Vector2(CorrectSize, CorrectSize), Time.deltaTime * 8);

        if (MiniMap.RotationAlwaysFront)
        {
            //with this the icon rotation always will be the same (for front)
            Quaternion r = Quaternion.identity;
            r.x = Target.rotation.x;

            rt.localRotation = r;
        }
        else
        {
            //with this the rotation icon will depend of target
            Vector3 vre = MiniMap.transform.eulerAngles;
            Vector3 re = Vector3.zero;
            //Fix player rotation for apply to el icon.
            re.z = ((-this.Target.rotation.eulerAngles.y) + vre.y);

            Quaternion q = Quaternion.Euler(re);
            rt.rotation = q;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void StartEffect()
    {
        Animator a = Graphic.GetComponent<Animator>();
        if (m_Effect == ItemEffect.Pulsing)
        {
            a.SetInteger("Type", 2);
        }
        else if (m_Effect == ItemEffect.Fade)
        {
            a.SetInteger("Type", 1);
        }
    }

    /// <summary>
    /// When player or the target die,desactive,remove,etc..
    /// call this for remove the item UI from Map
    /// for change to other icon and desactive in certain time
    /// or destroy immediate
    /// </summary>
    /// <param name="inmediate"></param>
    public void DestroyItem(bool inmediate)
    {
        if (Graphic == null)
        {
            Debug.Log("Graphic Item of " + this.name + " not exist in scene");
            return;
        }

        if (DeathIcon == null || inmediate)
        {
            Graphic.GetComponent<bl_IconItem>().DestroyIcon(inmediate);
        }
        else
        {
            Graphic.GetComponent<bl_IconItem>().DestroyIcon(inmediate,DeathIcon);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ico"></param>
    public void SetIcon(Sprite ico)
    {
        if(cacheItem == null)
        {
            Debug.LogWarning("You can't set a icon before create the item.");
            return;
        }

        cacheItem.GetComponent<bl_IconItem>().SetIcon(ico);
    }

    /// <summary>
    /// Show a visible circle area in the minimap with this
    /// item as center
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="AreaColor"></param>
    public void SetCircleArea(float radius, Color AreaColor)
    {
        CircleAreaRect = cacheItem.GetComponent<bl_IconItem>().SetCircleArea(radius,AreaColor);
    }

    /// <summary>
    /// 
    /// </summary>
    public void HideCircleArea()
    {
        cacheItem.GetComponent<bl_IconItem>().HideCircleArea();
        CircleAreaRect = null;
    }

    /// <summary>
    /// Call this for hide item in miniMap
    /// For show again just call "ShowItem()"
    /// NOTE: For destroy item call "DestroyItem(bool immediate)" instant this.
    /// </summary>
    public void HideItem()
    {
        if (cacheItem != null)
        {
            cacheItem.SetActive(false);
        }
        else
        {
            Debug.Log("There is no item to disable.");
        }
    }

    /// <summary>
    /// Call this for show again the item in miniMap when is hide
    /// </summary>
    public void ShowItem()
    {
        if (cacheItem != null)
        {
            cacheItem.SetActive(true);
            cacheItem.GetComponent<bl_IconItem>().SetVisibleAlpha();
        }
        else
        {
            Debug.Log("There is no item to active.");
        }
    }

    /// <summary>
    /// If you need destroy icon when this gameObject is destroy.
    /// </summary>
    void OnDestroy()
    {
        if (DestroyWithObject)
        {
            DestroyItem(true);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public Vector3 TargetPosition
    {
        get
        {
            if (Target == null)
            {
                return Vector3.zero;
            }

            return new Vector3(Target.position.x, 0, Target.position.z);
        }
    }

    private bl_MiniMap _minimap = null;
    private bl_MiniMap m_miniMap
    {
        get
        {
            if (_minimap == null)
            {
                _minimap = this.cacheItem.transform.root.GetComponentInChildren<bl_MiniMap>();
            }
            return _minimap;
        }
    }
}