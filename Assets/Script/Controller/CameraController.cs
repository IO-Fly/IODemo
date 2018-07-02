using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;



public class CameraController : MonoBehaviour {


    private GameObject player;
    //public PostProcessingProfile normal, fx;
    //private PostProcessingBehaviour camImageFx;

    public float mouseSensitivity = 3.0f;
    public float scrollSensitivity = 3.0f;

    private float distanceToPlayer;
    public float distanceToPlayerInit = 7.5f;
    public float maxDistanceInit = 10.0f;
    public float minDistanceInit = 5.0f;

    private Vector3 direction;//方向向量，玩家角色指向摄像机
    private Vector3 up;
    private Vector3 right;

    private float offsetAngleHorizontal;
    private float offsetAngleVertical;

    struct Barrier
    {
        public GameObject barrierObject;
        public Shader shader;
        public Barrier(GameObject _obj = null, Shader _shader = null)
        {
            this.barrierObject = _obj;
            this.shader = _shader;
        }
    }
    private List<Barrier> currentBarrier = new List<Barrier>();
    private Shader transparentShader;

    // Use this for initialization
    void Start () {
        distanceToPlayer = distanceToPlayerInit;
        offsetAngleHorizontal = 0.0f;
        offsetAngleVertical = 0.0f;
        //camImageFx = FindObjectOfType<PostProcessingBehaviour>();
        transparentShader = Shader.Find("Transparent/Diffuse");
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //缺少指定的玩家
        if(player == null){
            return;
        }
        HandleMouseButtonDown();
        HandleMouseScroll();
    }

    void LateUpdate () {
        //缺少指定的玩家
        if (player == null)
        {
            return;
        }
        FollowPlayer();
        HandleBarrier();
        //HandlePostProcessing(); 
    }

    private void HandleMouseButtonDown()
    {
        if (Input.GetMouseButton(1))//鼠标右键被按下，调整视角
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            offsetAngleHorizontal += mouseX;
            offsetAngleVertical += mouseY;
        }
        else//视角回归原位，对视角进行平滑过渡
        {
            if (Mathf.Abs(offsetAngleHorizontal) < 1e-6)
                offsetAngleHorizontal = 0.0f;
            else
                offsetAngleHorizontal *= 0.6f;
            if (Mathf.Abs(offsetAngleVertical) < 1e-6)
                offsetAngleVertical = 0.0f;
            else
                offsetAngleVertical *= 0.6f;
        }
    }

    private void HandleMouseScroll()
    {
        float playerSize = player.GetComponent<Player>().GetRenderPlayerSize().x;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distanceToPlayerInit -= scroll * scrollSensitivity * playerSize;//滚轮往“上”滚时，摄像机距离拉近
        distanceToPlayerInit = Mathf.Clamp(distanceToPlayerInit, minDistanceInit, maxDistanceInit);
    }


    private void FollowPlayer()
    {
        Vector3 playerTowards = player.gameObject.GetComponent<ObjectBehaviour>().GetForwardDirection();
        right = new Vector3(playerTowards.z, 0.0f, -playerTowards.x).normalized;
        up = Vector3.Cross(playerTowards, right).normalized;
        direction = Vector3.SlerpUnclamped(playerTowards, right, offsetAngleHorizontal / 90.0f);
        direction = Vector3.SlerpUnclamped(direction, up, 1.8f + offsetAngleVertical / 90.0f).normalized;

        float playerSize = player.GetComponent<Player>().GetRenderPlayerSize().x;
        distanceToPlayer = playerSize * distanceToPlayerInit;

        transform.position = player.gameObject.transform.position + direction * distanceToPlayer;
        transform.LookAt(player.transform);
        transform.Translate(new Vector3(0.0f, 4.5f, -9.0f));
    }

    private void HandleBarrier()
    {
        Vector3 pointBegin = transform.position;
        Vector3 pointEnd = player.transform.position;
        Vector3 direction = (pointEnd - pointBegin).normalized;
        Ray ray = new Ray(pointBegin, direction);


        bool hasHitBarrier = false;
        RaycastHit[] hits = Physics.RaycastAll(ray, (pointEnd - pointBegin).magnitude);
        foreach(RaycastHit hit in hits)
        {
            GameObject thisBarrierObject = hit.collider.gameObject;
            if (thisBarrierObject == player) continue;

            hasHitBarrier = true;

            bool isHasObject = false;
            for (int i = 0; i < currentBarrier.Count; i++)
            {
                
                if (currentBarrier[i].barrierObject == thisBarrierObject)
                {
                    isHasObject = true;
                }
            }

            if (isHasObject)
            {
                continue;
            }

            MeshRenderer renderer = thisBarrierObject.GetComponent<MeshRenderer>();
            if (renderer != null && renderer.enabled)
            {
                Material m = renderer.material;
                Barrier barrier = new Barrier(thisBarrierObject, m.shader);
                currentBarrier.Add(barrier);

                m.shader = transparentShader;
                m.color = new Color(m.color.r, m.color.g, m.color.b, 0.3f);
            }          
        }


        if (!hasHitBarrier && currentBarrier.Count > 0)
        {
            for(int i = 0; i < currentBarrier.Count; i++)
            {         
                Material m = currentBarrier[i].barrierObject.GetComponent<MeshRenderer>().material;
                m.shader = currentBarrier[i].shader;
            }             
            currentBarrier.Clear();
        }

    }

    /*private void HandlePostProcessing()
    {
        if (player.transform.position.y > 0.0f)
        {
            this.gameObject.GetComponent<PostProcessingBehaviour>().profile = normal;
        }
        if (player.transform.position.y < 0.0f)
        {
            this.gameObject.GetComponent<PostProcessingBehaviour>().profile = fx;
        }
        //if(this.transform.position.y>0){
        //        this.gameObject.GetComponent<PostProcessingBehaviour>().profile = normal;
        //    }
        //if (this.transform.position.y<0){
        //   this.gameObject.GetComponent<PostProcessingBehaviour>() .profile = fx;
        //}
    }*/



    public Vector3 GetDirectionNormal()
    {
        return direction;
    }

   
    public void setPlayer(GameObject player)
    {
        this.player = player;
    }

}
