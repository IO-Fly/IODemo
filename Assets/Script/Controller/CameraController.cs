using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;



public class CameraController : MonoBehaviour {


    private GameObject player;
    public PostProcessingProfile normal, fx;
    private PostProcessingBehaviour camImageFx;

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
        public bool keepFlag;
        public Barrier(GameObject _obj, Shader _shader)
        {
            this.barrierObject = _obj;
            this.shader = _shader;
            keepFlag = true;
        }
        public void SetKeepFlag(bool flag)
        {
            keepFlag = flag;
        }
    }
    private LinkedList<Barrier> barriers = new LinkedList<Barrier>();
    private Shader transparentShader;

    // Use this for initialization
    void Start () {
        distanceToPlayer = distanceToPlayerInit;
        offsetAngleHorizontal = 0.0f;
        offsetAngleVertical = 0.0f;
        camImageFx = FindObjectOfType<PostProcessingBehaviour>();
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

        Vector3 playerTowards = player.gameObject.GetComponent<ObjectBehaviour>().GetForwardDirection();
        right = new Vector3(playerTowards.z, 0.0f, -playerTowards.x).normalized;
        up = Vector3.Cross(playerTowards, right).normalized;
        direction = Vector3.SlerpUnclamped(playerTowards, right, offsetAngleHorizontal / 90.0f);
        direction = Vector3.SlerpUnclamped(direction, up, 1.8f + offsetAngleVertical/90.0f).normalized;


        float playerSize = player.GetComponent<Player>().GetRenderPlayerSize().x;
        distanceToPlayer = playerSize * distanceToPlayerInit;


        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distanceToPlayerInit -= scroll * scrollSensitivity * playerSize;//滚轮往“上”滚时，摄像机距离拉近
        distanceToPlayerInit = Mathf.Clamp(distanceToPlayerInit, minDistanceInit, maxDistanceInit);
    }

    void LateUpdate () {

        //缺少指定的玩家
        if (player == null)
        {
            return;
        }

        transform.position = player.gameObject.transform.position + direction * distanceToPlayer;
        transform.LookAt(player.transform);
        transform.Translate(new Vector3(0.0f, 4.5f, -9.0f));
        HandleBarrier();
        if(this.transform.position.y>0){
                this.gameObject.GetComponent<PostProcessingBehaviour>().profile = normal;
            }
        if(this.transform.position.y<0){
           this.gameObject.GetComponent<PostProcessingBehaviour>() .profile = fx;
        }
        
    }
    
    private void HandleBarrier()
    {
        Vector3 pointBegin = transform.position;
        Vector3 pointEnd = player.transform.position;
        Vector3 direction = (pointEnd - pointBegin).normalized;
        Ray ray = new Ray(pointBegin, direction);

        
        RaycastHit[] hits = Physics.RaycastAll(ray, (pointEnd - pointBegin).magnitude);
        foreach(RaycastHit hit in hits)
        {
            GameObject thisBarrier = hit.collider.gameObject;
            if (thisBarrier == player) continue;
            MeshRenderer renderer = thisBarrier.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                bool findFlag = false;
                foreach(Barrier barrier in barriers)
                {
                    if (barrier.barrierObject == thisBarrier)
                    {
                        barrier.SetKeepFlag(true);
                        findFlag = true;
                        break;
                    }
                }
                if (!findFlag)
                {
                    barriers.AddFirst(new Barrier(thisBarrier, renderer.material.shader));
                    renderer.material.shader = transparentShader;
                    ///renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 0.3f);
                }
            } 
        }

        LinkedListNode<Barrier> link = barriers.First;
        while (link!=barriers.Last)
        {
            Barrier barrier = link.Value;

            if (!barrier.keepFlag)
            {
                Material thisMaterial = barrier.barrierObject.GetComponent<MeshRenderer>().material;
                thisMaterial.shader = barrier.shader;
                ///thisMaterial.color = new Color(thisMaterial.color.r, thisMaterial.color.g, thisMaterial.color.b, 1.0f);
                barriers.Remove(link);
            }
            else
            {
                barrier.SetKeepFlag(false);
            }

            link = link.Next;
        }
        
        
    }


    public Vector3 GetDirectionNormal()
    {
        return direction;
    }

   
    public void setPlayer(GameObject player)
    {
        this.player = player;
    }

}
