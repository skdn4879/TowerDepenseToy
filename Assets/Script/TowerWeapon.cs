using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { Cannon = 0, Laser, Slow, Buff}

//public enum WeaponState { SearchTarget = 0, AttackToTarget} //SearchTarget ���� ��� Ž��, AttackToTarget ��� ����
public enum WeaponState { SearchTarget = 0, TryAttackCannon, TryAttackLaser}

public class TowerWeapon : MonoBehaviour
{
    [Header("Commons")]
    [SerializeField]
    private TowerTemplate towerTemplate;                            //Ÿ�� ����(���ݷ�, ���ݼӵ� ��)
    [SerializeField]
    private WeaponType weaponType;                                  //���� �Ӽ� ����
    [SerializeField]
    private Transform spawnPoint;                                   //�߻�ü ���� ��ġ

    [Header("Cannon")]
    [SerializeField]
    private GameObject projectilePrefab;                            //�߻�ü ������

    [Header("Laser")]
    [SerializeField]
    private LineRenderer lineRenderer;                              //�������� ���Ǵ� ��
    [SerializeField]
    private Transform hitEffect;                                    //Ÿ�� ȿ��
    [SerializeField]
    private LayerMask targetLayer;                                  //������ �ε����� ���̾� ����

    private int level = 0;                                          //Ÿ�� ����

    private WeaponState weaponState = WeaponState.SearchTarget;     //Ÿ�� ���� ����
    private Transform attackTarget = null;                          //���� ���
    private EnemySpwaner enemySpwaner;                              //���ӿ� �����ϴ� �� ���� ȹ���

    private SpriteRenderer spriteRenderer;                          //Ÿ�� ������Ʈ �̹��� �����
    private PlayerGold playerGold;                                  //�÷��̾��� ��� ���� ȹ�� �� ����

    private Tile ownerTile;       //���� Ÿ���� ��ġ�� Ÿ��

    private TowerSpawner towerSpawner;
    private float addedDamage;          //������ ���� �߰��� ������
    private int buffLevel;              //���� �޴� �� ���� ���� (0 : �ȹ���, 1~3 : ��������)

    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    //public float Damage => attackDamage;
    public float Damage => towerTemplate.weapon[level].damage;
    //public float Rate => attackRate;
    public float Rate => towerTemplate.weapon[level].rate;
    //public float Range => attackRange;
    public float Range => towerTemplate.weapon[level].range;
    public float Level => level + 1;
    public int MaxLevel => towerTemplate.weapon.Length;
    public float Slow => towerTemplate.weapon[level].slow;
    public WeaponType WeaponType => weaponType;

    public float AddedDamage
    {
        set => addedDamage = Mathf.Max(0, value);
        get => addedDamage;
    }
    public int BuffLevel
    {
        set => buffLevel = Mathf.Max(0, value);
        get => buffLevel;
    }

    public float Buff => towerTemplate.weapon[level].buff;

    public int UpgradeCost => Level < MaxLevel ? towerTemplate.weapon[level + 1].cost : 0;
    public int SellCost => towerTemplate.weapon[level].sell;

    public void SetUp(TowerSpawner towerSpawner, EnemySpwaner enemySpwaner, PlayerGold playerGold, Tile ownerTile)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.towerSpawner = towerSpawner;
        this.playerGold = playerGold;
        this.enemySpwaner = enemySpwaner;
        this.ownerTile = ownerTile;

        //���� �Ӽ��� ĳ���̳� �������� ����
        if(weaponType == WeaponType.Cannon || weaponType == WeaponType.Laser)
        {
            //���� ���¸� WeaponState.SearchTarget���� ����
            ChangeState(WeaponState.SearchTarget);
        }
        
    }

    public void ChangeState(WeaponState newState)
    {
        StopCoroutine(weaponState.ToString());

        weaponState = newState;

        StartCoroutine(weaponState.ToString());
    }

    private void Update()
    {
        if(attackTarget != null)
        {
            RotateToTarget();
        }
    }

    private void RotateToTarget()
    {
        //�������κ����� �Ÿ��� ���������κ����� ������ �̿��� ��ġ�� ���ϴ� �� ��ǥ�� �̿�
        //���� = arctan(y/x)
        //x, y ������ ���ϱ�
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;
        //x, y ���� ���� �������� ���� ���ϱ�
        //������ radian �����̱� ������ Mathf.Rad2Deg�� ���� �� ������ ����
        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    private IEnumerator SearchTarget()
    {
        while (true)
        {
            /*//���� ������ �ִ� ���� ã�� ���� ���� �Ÿ��� �ִ��� ũ�� ����
            float closestDisSqr = Mathf.Infinity;

            //EnemySpawner�� EnemyList�� �ִ� ���� �ʿ� �����ϴ� ��� �� �˻�
            for(int i = 0; i < enemySpwaner.EnemyList.Count; i++)
            {
                float distance = Vector3.Distance(enemySpwaner.EnemyList[i].transform.position, transform.position);
                //���� �˻����� ������ �Ÿ��� ���ݹ��� ���� �ְ�, ������� �˻��� ������ �Ÿ��� ������
                //if(distance <= attackRange && distance <= closestDisSqr)
                if(distance <= towerTemplate.weapon[level].range && distance <= closestDisSqr)
                {
                    closestDisSqr = distance;
                    attackTarget = enemySpwaner.EnemyList[i].transform;
                }
            }

            if(attackTarget != null)
            {
                ChangeState(WeaponState.AttackToTarget);
            }

            yield return null;*/

            //���� Ÿ���� ���� ������ �ִ� ���� ��� Ž��
            attackTarget = FindClosestAttackTarget();

            if(attackTarget != null)
            {
                if(weaponType == WeaponType.Cannon)
                {
                    ChangeState(WeaponState.TryAttackCannon);
                }
                else if(weaponType == WeaponType.Laser)
                {
                    ChangeState(WeaponState.TryAttackLaser);
                }
            }

            yield return null;
        }
    }

    private Transform FindClosestAttackTarget()
    {
        //���� ������ �ִ� ���� ã�� ���� ���� �Ÿ��� �ִ��� ũ�� ����
        float closestDisSqr = Mathf.Infinity;

        //EnemySpawner�� EnemyList�� �ִ� ���� �ʿ� �����ϴ� ��� �� �˻�
        for (int i = 0; i < enemySpwaner.EnemyList.Count; i++)
        {
            float distance = Vector3.Distance(enemySpwaner.EnemyList[i].transform.position, transform.position);
            //���� �˻����� ������ �Ÿ��� ���ݹ��� ���� �ְ�, ������� �˻��� ������ �Ÿ��� ������
            //if(distance <= attackRange && distance <= closestDisSqr)
            if (distance <= towerTemplate.weapon[level].range && distance <= closestDisSqr)
            {
                closestDisSqr = distance;
                attackTarget = enemySpwaner.EnemyList[i].transform;
            }
        }

        return attackTarget;
    }

    private bool IsPossibleToAttackTarget()
    {
        //target�� �ִ��� �˻�(�ٸ� �߻�ü�� ���� �ı� or Goal�������� �̵��� ���� ��)
        if(attackTarget == null)
        {
            return false;
        }

        //target�� ���� ���� �ȿ� �ִ��� �˻�(���� ������ ����� ���ο� �� Ž��)
        float distance = Vector3.Distance(attackTarget.position, transform.position);
        if(distance > towerTemplate.weapon[level].range)
        {
            attackTarget = null;
            return false;
        }

        return true;
    }

    /*private IEnumerator AttackToTarget()
    {
        while (true)
        {
            //1.target�� �ִ��� �˻�(�ٸ� �߻�ü�� ���� ����, goal ���� �������� ���� ���� ��)
            if(attackTarget == null)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            //2.target�� ���� ���� �ȿ� �ִ��� �˻�(���� ������ ����� ���ο� �� Ž��)
            float distance = Vector3.Distance(attackTarget.position, transform.position);
            //if(distance > attackRange)
            if(distance > towerTemplate.weapon[level].range)
            {
                attackTarget = null;
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            //3.attackRate �ð���ŭ ���
            //yield return new WaitForSeconds(attackRate);
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            //4.����(�߻�ü ����)
            SpawnProjectile();
        }
    }*/

    private IEnumerator TryAttackCannon()
    {
        while (true)
        {
            if(IsPossibleToAttackTarget() == false)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            //3.attackRate �ð���ŭ ���
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            //4.ĳ�� ����(�߻�ü ����)
            SpawnProjectile();
        }
    }

    private IEnumerator TryAttackLaser()
    {
        //������, ������ Ÿ�� ȿ�� Ȱ��ȭ
        EnableLaser();

        while (true)
        {
            //target�� �����ϴ°� �������� �˻�
            if(IsPossibleToAttackTarget() == false)
            {
                //������, ������ Ÿ�� ȿ�� ��Ȱ��ȭ
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            //������ ����
            SpawnLaser();

            yield return null;
        }
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        //���ݷ� = Ÿ�� �⺻ ���ݷ� + ������ ���� �߰��� ���ݷ�
        float damage = towerTemplate.weapon[level].damage + AddedDamage;

        //������ �߻�ü���� ���� ���(attackTarget) ���� ����
        //clone.GetComponent<Projectile>().SetUp(attackTarget, attackDamage);
        clone.GetComponent<Projectile>().SetUp(attackTarget, damage);
    }

    private void EnableLaser()
    {
        lineRenderer.gameObject.SetActive(true);
        hitEffect.gameObject.SetActive(true);
    }

    private void DisableLaser()
    {
        lineRenderer.gameObject.SetActive(false);
        hitEffect.gameObject.SetActive(false);
    }

    private void SpawnLaser()
    {
        Vector3 direction = attackTarget.position - spawnPoint.position;
        RaycastHit2D[] hit = Physics2D.RaycastAll(spawnPoint.position, direction, towerTemplate.weapon[level].range, targetLayer);

        //���� �������� ���� ���� ������ ���� �� �� ���� attackTarget�� ������ ������Ʈ�� ����
        for(int i = 0; i < hit.Length; i++)
        {
            if(hit[i].transform == attackTarget)
            {
                //���� ��������
                lineRenderer.SetPosition(0, spawnPoint.position);
                //���� ��ǥ����
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                //Ÿ��ȿ�� ��ġ ����
                hitEffect.position = hit[i].point;

                //���ݷ� = Ÿ�� �⺻ ���ݷ� + ������ ���� �߰��� ���ݷ�
                float damage = towerTemplate.weapon[level].damage + AddedDamage;

                //�� ü�� ����(1�ʿ� damage��ŭ)
                attackTarget.GetComponent<EnemyHp>().TakeDamage(damage * Time.deltaTime);
            }
        }
    }

    public bool Upgrade()
    {
        //Ÿ�� ���׷��̵忡 �ʿ��� ��尡 ������� �˻�
        if(playerGold.CurrentGold < towerTemplate.weapon[level + 1].cost)
        {
            return false;
        }

        //Ÿ�� ���� ����
        level++;
        //Ÿ�� ���� ����(Sprite)
        spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
        //��� ����
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;

        //���� �Ӽ��� �������̸�
        if(weaponType == WeaponType.Laser)
        {
            //������ ���� ������ ���� ����
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth = 0.05f;
        }

        //Ÿ���� ���׷��̵� �� �� ��� ���� Ÿ���� ���� ȿ�� ����
        //���� Ÿ���� ����Ÿ���� ���, ���� Ÿ���� ����Ÿ���� ���
        towerSpawner.OnBuffAllBuffTowers();

        return true;
    }

    public void Sell()
    {
        //��� ����
        playerGold.CurrentGold += towerTemplate.weapon[level].sell;
        //���� Ÿ�Ͽ� �ٽ� Ÿ�� �Ǽ��� �����ϵ��� ����
        ownerTile.IsBuildTower = false;
        //Ÿ�� �ı�
        Destroy(gameObject);
    }

    public void OnBuffAroundTower()
    {
        //���� �ʿ� ��ġ�� ��� Tower �±׸� ���� ������Ʈ�� Ž��
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for(int i = 0; i < towers.Length; i++)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            //�̹� ������ �ް� �ְ�, ���� ����Ÿ���� �������� ���� �����̸� �н�
            if(weapon.BuffLevel > Level)
            {
                continue;
            }
            
            //���� ����Ÿ���� �ٸ� Ÿ���� �Ÿ��� �˻��ؼ� ���� �ȿ� Ÿ���� ������
            if(Vector3.Distance(weapon.transform.position, transform.position) <= towerTemplate.weapon[level].range)
            {
                //������ ������ ĳ���̳� ������ Ÿ���̸�
                if(weapon.WeaponType == WeaponType.Cannon || weapon.WeaponType == WeaponType.Laser)
                {
                    //������ ���� ���ݷ� ����
                    weapon.AddedDamage = weapon.Damage * (towerTemplate.weapon[level].buff);
                    //Ÿ���� �ް��ִ� �������� ����
                    weapon.BuffLevel = (int)Level;
                }
            }
        }
    }
}
