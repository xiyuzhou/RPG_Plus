using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    [HideInInspector]
    public int id;
    [Header("Info")]
    public float moveSpeed;
    public int gold;
    public int curHp;
    public int maxHp;
    public bool dead;
    [Header("Attack")]
    public int damage;
    public float attackRange;
    public float attackRate;
    private float lastAttackTime;
    [Header("Components")]
    public Rigidbody2D rig;
    public Player photonPlayer;
    public SpriteRenderer sr;
    public Animator weaponAnim;
    // local player
    public static PlayerController me;
    public HeaderInfo headerInfo;
    private Vector2 DirectionCache;
    public float resistance = 10f;
    public SpriteRenderer[] playerImage;
    private int lastDir = 1;
    void Update()
    {   
        if (!photonView.IsMine)
            return;
        Move();
        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime > attackRate)
            Attack();
        float mouseX = (Screen.width / 2) - Input.mousePosition.x;
        int direct = (mouseX < 0) ? 1 : -1;
        if (lastDir != direct)
        {
            lastDir = direct;
            photonView.RPC("RotationHandler", RpcTarget.All, direct);
        } 
    }
    [PunRPC]
    public void RotationHandler(int direction)
    {
        weaponAnim.transform.parent.localScale = new Vector3(direction, 1, 1);
    }
    void Move()
    {
        // get the horizontal and vertical inputs
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        //Vector2 moveDirection = new Vector2(x, y);
        //DirectionCache.x = Mathf.MoveTowards(DirectionCache.x, moveDirection.x, resistance * Time.deltaTime);
        //DirectionCache.y = Mathf.MoveTowards(DirectionCache.y, moveDirection.y, resistance * Time.deltaTime);
        // apply that to our velocity
        //transform.position = transform.position + new Vector3(moveDirection.x, moveDirection.y, 0f) * moveSpeed * Time.deltaTime;
        rig.velocity = new Vector2(x, y) * moveSpeed;
    }
    void Attack()
    {
        //AnimatorStateInfo state = weaponAnim.GetCurrentAnimatorStateInfo(0);
        //Debug.Log(state.IsName("Idle"));
        //if (!state.IsName("Idle"))
            //return;
        //weaponAnim.SetBool("AttackBool", false);
        lastAttackTime = Time.time;
        // calculate the direction
        Vector3 dir = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;
        // shoot a raycast in the direction
        RaycastHit2D hit = Physics2D.Raycast(transform.position + dir, dir, attackRange);
        // did we hit an enemy?
        if (hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
        {
            // get the enemy and damage them
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            enemy.photonView.RPC("TakeDamage", RpcTarget.MasterClient, damage);
        }
        // play attack animation
        //weaponAnim.SetTrigger("Attack");
        weaponAnim.SetBool("AttackBool", true);
        Invoke("attackFinished", attackRate);
    }
    void attackFinished()
    {
        weaponAnim.SetBool("AttackBool", false);
    }
    [PunRPC]
    public void TakeDamage(int damage)
    {
        curHp -= damage;
        // update the health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);
        if (curHp <= 0)
            Die();
        else
        {
            StartCoroutine(DamageFlash());
            IEnumerator DamageFlash()
            {
                sr.color = Color.red;
                yield return new WaitForSeconds(0.05f);
                sr.color = Color.white;
            }
        }
    }
    void Die()
    {
        dead = true;
        rig.isKinematic = true;
        transform.position = new Vector3(0, 99, 0);
        Vector3 spawnPos = GameManager.instance.spawnPoints[Random.Range(0, GameManager.instance.spawnPoints.Length)].position;
        StartCoroutine(Spawn(spawnPos, GameManager.instance.respawnTime));
    }
    IEnumerator Spawn(Vector3 spawnPos, float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);
        dead = false;
        transform.position = spawnPos;
        curHp = maxHp;
        rig.isKinematic = false;
        // update the health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);
    }
    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        Debug.Log(id);
        photonPlayer = player;
        GameManager.instance.players[id - 1] = this;
        // initialize the health bar
        headerInfo.Initialize(player.NickName, maxHp);

        if (player.IsLocal)
            me = this;
        else
            rig.isKinematic = true;

        int[] playerIndex = (int[])PhotonNetwork.PlayerList[id-1].CustomProperties["plyerImageIndex"];
        for (int i = 0;i<6;i++)
        {
            playerImage[i].sprite = SpriteManager.instance.sprites[playerIndex[i]];
        }
    }
    [PunRPC]
    void Heal(int amountToHeal)
    {
        curHp = Mathf.Clamp(curHp + amountToHeal, 0, maxHp);
        // update the health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);
    }
    [PunRPC]
    void GiveGold(int goldToGive)
    {
        gold += goldToGive;
        // update the ui
        GameUI.instance.UpdateGoldText(gold);
    }

}
