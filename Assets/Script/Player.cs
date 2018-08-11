using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : Photon.PunBehaviour {

    private BattleUI battleUI;

    public float initialSize = 1.0f;
    public float initialSpeed = 20.0f;
    public float initalHealth = 100.0f;
    public float health = 100.0f;


    private float playerEnergy;
    private Vector3 playerSize;

    private float sizeEffect;//用于本地视口玩家角色大小显示效果
    private float speedOffset;//用于暂时性的增加速度
    private Vector3 sizeOffset;//用于暂时性的增加体型

    private float speed;
    private string playerName;//玩家自定义的名字
    private int count;//碰撞后偏移的执行次数
    public int Lock=0;
    public int HitLock=0;

    #region MoboBehaviour CallBacks
    // Use this for initialization
    void Start () {
        playerEnergy = initialSize;
        playerSize = new Vector3(initialSize, initialSize, initialSize);
        transform.localScale = playerSize;
        speed = initialSpeed;
        sizeEffect = 1.0f;
        speedOffset = 0.0f;
        sizeOffset = Vector3.zero;
     
        StartCoroutine(Recover());
    
        if (!this.photonView.isMine)
        {
            foreach(Player player in networkManager.playerList)
            {
                if (player.photonView.isMine)
                {
                    player.photonView.RPC("SetPlayerName", PhotonTargets.All, player.GetPlayerName());//设置玩家名字
                }
            }           
        }
        else
        {
            this.photonView.RPC("SetPlayerName", PhotonTargets.All, playerName);//设置玩家名字
        }

    }
	
	// Update is called once per frame
	void FixedUpdate () {
       
        //Debug.Log("当前Lock值: "+Lock);
        if(this.tag == "playerCopy")
        {
            Debug.LogWarning("分身当前血量：" + health);
            Debug.LogWarning("分身当前能量：" + playerEnergy);
        }
    }

    void OnDestroy()
    {

        //玩家分身不维护在玩家列表
        if(this.tag == "playerCopy")
        {
            return;
        }

        //从玩家列表中移除玩家
        for (int i  = networkManager.playerList.Count - 1; i >= 0; i--)
        {
            Player curPlayer = networkManager.playerList[i].GetComponent<Player>();
            if (this.photonView.viewID == curPlayer.photonView.viewID)
            {
                networkManager.playerList.Remove(curPlayer);
            }
        }
        //呈现更新的玩家列表
        //showPlayerList();
        battleUI.RemovePlayer();

        //玩家死亡时生成食物
        if (this.photonView.isMine)
        {
            Debug.LogWarning("Player Die");
            FoodManager.localFoodManager.InitPlayerFood(this);
        }

        //主客户端更换时重新启动同步协程
        if (PhotonNetwork.isMasterClient && !FoodManager.localFoodManager.isMasterBefore)
        {
            Debug.LogWarning("Master change!");
            FoodManager.localFoodManager.InitFoodInMaster();
            FoodManager.localFoodManager.StartCoroutine(FoodManager.localFoodManager.SyncFoodAITranform());
            FoodManager.localFoodManager.isMasterBefore = true;

        }

        if(this.health <=0 && IsLocalPlayer()){
            Debug.LogWarning("菜单为失败状态!");
			GameObject.Find("HUDCanvas").transform.Find("Menu").Find("Status").gameObject.GetComponent<Image>().sprite = GameObject.Find("HUDCanvas").GetComponent<MenuUI>().lose;
            GameObject.Find("HUDCanvas").GetComponent<MenuUI>().freeze = true;
				
		}
		else if(networkManager.playerList.Count == 1 && networkManager.playerList[0].gameObject == networkManager.localPlayer){
            Debug.LogWarning("菜单为胜利状态!");
            GameObject.Find("HUDCanvas").transform.Find("Menu").Find("Status").gameObject.GetComponent<Image>().sprite = GameObject.Find("HUDCanvas").GetComponent<MenuUI>().win;
            GameObject.Find("HUDCanvas").GetComponent<MenuUI>().freeze = true;
		}
		
    }

    void Awake()
    {

        //玩家分身不维护在玩家列表
        if (this.tag != "playerCopy")
        {
            // 增加当前player到玩家列表
            networkManager.playerList.Add(this);      
        }
        DontDestroyOnLoad(this.gameObject);

        //初始化玩家名字
        if (IsLocalPlayer())
        {
            playerName = NetworkMatch.playerName;
        }
        if(IsPlayerAI() && this.photonView.isMine)
        {
            playerName = PlayerAIName.GetUniqueName();
        }
        

        //隐藏自身等待场景加载完成
        if (!PhotonNetwork.isMasterClient && SceneManager.GetActiveScene().name != NetworkMatch.sceneName)
        {
            this.gameObject.SetActive(false);
        }
              
    }
    void OnEnable()
    {
        //当前场景
        Debug.LogWarning("当前场景： " + SceneManager.GetActiveScene().name);

        // 获得battleUI
        GameObject rootCanvas = GameObject.Find("HUDCanvas");
        GameObject battleUINode = rootCanvas.transform.Find("BattleUI").gameObject;
        battleUI = battleUINode.GetComponent<BattleUI>();

        //玩家分身不维护在玩家列表
        if (this.tag != "playerCopy")
        {
            // battleUI排行榜增加一个用户
            battleUI.AddPlayer();
        }
    }

    void OnControllerColliderHit(ControllerColliderHit other)
    {

        if (HitLock != 0)
            return;
        HitLock = 1;
        StartCoroutine(ReleaseHitLock());
        if (other.gameObject != this.gameObject && (other.gameObject.tag == "player" || other.gameObject.tag == "playerCopy") && this.photonView.isMine)
        {

            Debug.Log("碰撞到了玩家");
            Debug.Log("对方Lock值：" + other.gameObject.GetComponent<Player>().Lock);


            //自己的分身不能攻击自己
            if (!isCopyRelation(other.gameObject))
            {
                //生命值大于0才能伤害敌人
                Player enemy = other.gameObject.GetComponent<Player>();

                float selfDamage = 0.0f, enemyDamage = 0.0f;
                CalculateDamage(transform.localScale.x, other.gameObject.transform.localScale.x, ref selfDamage, ref enemyDamage);

                if (enemy.health > 0)
                {
                    this.photonView.RPC("GetDamage", PhotonTargets.AllViaServer, selfDamage);
                }

                if (health > 0)
                {
                    enemy.photonView.RPC("GetDamage", PhotonTargets.AllViaServer, enemyDamage);
                }
            }


            //碰撞效果
            GameObject Audio = GameObject.Find("Audio");
            if (other.gameObject.GetComponent<Player>().Lock == 0 && this.gameObject.transform.localScale.x >= other.gameObject.transform.localScale.x)
            {
                //other.gameObject.GetComponent<Player>().StartCoroutine(other.gameObject.GetComponent<Player>().Bomb(-other.normal));
                other.gameObject.GetComponent<Player>().Lock = 1;
                other.gameObject.GetComponent<Player>().photonView.RPC("DoBomb", PhotonTargets.All, -other.normal);
                Debug.Log("对方弹开");

                //玩家播放音效，分身不播放
                if(IsLocalPlayer())
                {
                    Audio.GetComponent<AudioManager>().PlayTouchSmallEnemy();
                }
                
            }
            else if (this.gameObject.transform.localScale.x < other.gameObject.transform.localScale.x)
            {
                Debug.Log("自己弹开");
                this.StartCoroutine(Bomb(other.normal));

                //玩家播放音效，分身不播放
                if (IsLocalPlayer())
                {
                    Audio.GetComponent<AudioManager>().PlayTouchBigEnemy();
                }
               
            }


        }
        else if (this.photonView.isMine && other.gameObject.tag != "poison" && other.gameObject.tag != "foodAI")/*other.gameObject.tag == "Wall" */
        {
            if (IsLocalPlayer())
            {
                //播放音效
                GameObject Audio = GameObject.Find("Audio");
                Audio.GetComponent<AudioManager>().PlayTouchWall();
            }        
        }

    }

    #endregion

    #region Utility

    private void CalculateDamage(float selfSize,float enemySize,ref float out_selfDamage,ref float out_enemyDamage)
    {
        const float minSize = 1.0f, maxSize = 25.0f;
        const float minDamage = 10.0f, maxDamage = 80.0f;
        float average = (selfSize + enemySize) / 2.0f;
        float ratio = (average - minSize) / (maxSize - minSize);
        float sumDamage = minDamage + (maxDamage - minDamage) * ratio;
        out_selfDamage = sumDamage * enemySize / (selfSize + enemySize);
        out_enemyDamage = sumDamage * selfSize / (selfSize + enemySize);
        const float expAmend = 0.013f;
        float selfExp = 1 + (enemySize - selfSize) * expAmend;
        float enemyExp = 1 + (selfSize - enemySize) * expAmend;
        out_selfDamage = Mathf.Pow(out_selfDamage, selfExp);
        out_enemyDamage = Mathf.Pow(out_enemyDamage, enemyExp);
    }

    //判断两个对象是否是分身关系
    public bool isCopyRelation(GameObject other)
    {
        if(this.gameObject.tag == "playerCopy" && other.tag == "player")
        {
            PlayerCopyController copyController = other.GetComponent<PlayerCopyController>();

            if(copyController == null)
            {
                return false;
            }
            if(copyController.getPlayerCopy() == this.gameObject)
            {
                return true;
            }
        }
        else if(this.gameObject.tag == "player" && other.tag == "playerCopy")
        {
            PlayerCopyController copyController = this.gameObject.GetComponent<PlayerCopyController>();
            if (copyController == null)
            {
                return false;
            }
            if (copyController.getPlayerCopy() == other)
            {
                return true;
            }
        }

        return false;
    }

    public int CalculateStarNum()
    {
        return 2 * (int)GetPlayerSize();
    }

    public float CalculateStarRange()
    {
        return GetPlayerSize();
    }

    //是否是玩家AI（非分身）
    public bool IsPlayerAI()
    {
        if(this.gameObject == networkManager.localPlayer || this.gameObject.tag == "playerCopy" )
        {
            return false;
        }
        else
        {
            return true;
        }

    }

    //是否是本地玩家（非AI）
    public bool IsLocalPlayer()
    {
        return this.gameObject == networkManager.localPlayer;
    }

    #endregion

    #region Change Attribute

    public void CopyPlayer(Player player)
    {
        
        this.health = player.health;
        this.playerEnergy = player.playerEnergy;
        this.playerSize = player.playerSize;
        this.speed = player.speed;
        this.playerName = player.playerName;
        this.count = player.count;
        this.Lock = player.Lock;
        this.transform.localScale = player.transform.localScale;
        this.initialSize = player.transform.localScale.x;
        this.initialSpeed = player.speed;

        if (this.GetComponent<PlayerAI>())
        {
            this.GetComponent<PlayerAI>().speed = player.speed;
        }

    }

    //更改大小
    void SetLocalScale(Vector3 playerSize, Vector3 sizeOffset)
    {

        Vector3 scale = playerSize + sizeOffset;
        if (scale.x <= 1)
        {
            playerEnergy = 1;
            this.playerSize = Vector3.one;
            transform.localScale = this.playerSize;
        }
        else
        {
            transform.localScale = scale;    
        }
        //battleUI.updateSeveralFrame();
    }

    void AddPlayerEnergy(float energyAdd)
    {
        

        playerEnergy += energyAdd;

        //限制最大能量
        playerEnergy = playerEnergy > 25 ? 25 : playerEnergy; 

        float sq = Mathf.Sqrt(playerEnergy);
        speed = 10 / sq + 8;
        playerSize = new Vector3(playerEnergy, playerEnergy, playerEnergy);
        SetLocalScale(playerSize, sizeOffset);
    }

    public void AddSpeedOffset(float speedOffset)
    {
        this.speedOffset += speedOffset;
        Debug.Log("速度改变: " + speedOffset);
    }

    public void AddSizeEffect(float sizeEffect)
    {
        this.sizeEffect *= sizeEffect;
    }

    public void SetSizeEffect(float effect)
    {
        this.sizeEffect = effect;
    }


    public void AddSizeOffset(Vector3 sizeOffset)
    {
        this.sizeOffset += sizeOffset;
        SetLocalScale(playerSize, this.sizeOffset);

    }

    #endregion

    #region Get Method

    public float GetSpeed()
    {
        return speed + speedOffset;
    }

    public float GetPlayerSize()
    {
        return transform.localScale.x;
    }

    public Vector3 GetRenderPlayerSize()
    {
        if (sizeEffect != 0)
        {
            return (transform.localScale) / sizeEffect;
        }
        else
        {
            return Vector3.positiveInfinity;
        }

    }

    public string GetPlayerName()
    {
        return this.playerName;
    }

    #endregion

    #region IEnumerator

    IEnumerator Bomb(Vector3 direction)
    {
        while (count < 5)
        {
            yield return null;
            this.gameObject.GetComponent<CharacterController>().Move(direction * Time.deltaTime * 40);
            Debug.Log("弹开方向： " + direction);
            Debug.Log("弹开执行次数： " + count);
            count++;
        }
        count = 0;
        this.photonView.RPC("ReleaseLock", PhotonTargets.All);
        Debug.Log("Lock= " + Lock);
    }

    IEnumerator Recover()
    {
        while (true)
        {
            if (this.health <= 99)
                this.health += 1;
            yield return new WaitForSeconds(1);
        }
    }
    IEnumerator ReleaseHitLock()
    {
        yield return new WaitForSeconds(1);
        this.HitLock = 0;
    }

    #endregion

    #region Photon Network
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(this.health);
        }
        else
        {
            this.health = (float)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void GetDamage(float damage)
    {
        health -= damage;
        if(health <= 0 && this.photonView.isMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
        		
    }

    [PunRPC]
    void DoBomb(Vector3 direction)
    {
        if (this.photonView.isMine)
        {
            this.StartCoroutine(Bomb(direction));
            Debug.Log("弹开RPC调用");
        }
    }

    [PunRPC]
    void ReleaseLock()
    {
        this.Lock = 0;
    }
  
    [PunRPC]
    public void SetPlayerName(string name)
    {
        Debug.LogWarning("调用SetPlayerName");
        this.playerName = name;

        this.GetComponent<PlayerHealthUI>().SetPlayerName(name);
    }

    [PunRPC]
    void EatFood()
    {
        //float baseScale = this.gameObject.transform.localScale.x;
        //if (gameObject.tag=="player" && GetComponent<PlayerSizeController>() != null)
        //    if (GetComponent<PlayerSizeController>().SkillInUse())
        //        baseScale -= GetComponent<PlayerSizeController>().addSize.x;

        AddPlayerEnergy(0.4f/Mathf.Sqrt(this.gameObject.transform.localScale.x));
        //AddPlayerEnergy(5.0f);

        //播放音效
        if (this.photonView.isMine && this.tag == "player" && !IsPlayerAI())
        {
            GameObject Audio = GameObject.Find("Audio");
            Audio.GetComponent<AudioManager>().PlayEatFood();
        }

    }

    [PunRPC]
    void EatPoison()
    {
        Debug.Log("玩家：碰到了毒物");
        AddPlayerEnergy(-0.5f);

        //播放音效
        if (this.tag == "player" && this.GetComponent<Player>().photonView.isMine && !IsPlayerAI())
        {
            GameObject Audio = GameObject.Find("Audio");
            Audio.GetComponent<AudioManager>().PlayEatPoison();
        }
    }

    [PunRPC]
    public void SetActive(bool isActive)
    {
        this.gameObject.SetActive(isActive);
    }


    #endregion

    #region Debug
    void showPlayerList()
    {
        Debug.LogWarning("玩家数：" + networkManager.playerList.Count);
        for (int i = 0; i < networkManager.playerList.Count; i++)
        {
            Player curPlayer = networkManager.playerList[i];
            Debug.LogWarning("玩家" + i + curPlayer.GetPlayerName());
        }
    }
    #endregion

}
