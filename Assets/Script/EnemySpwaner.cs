using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpwaner : MonoBehaviour
{
    /*[SerializeField]
    private GameObject enemyPrefab; //�� ������
    [SerializeField]
    private float spawnTime;        //�� �����ֱ�*/

    [SerializeField]
    private Transform[] wayPoints;  //���� ���������� �̵� ���
    private List<Enemy> enemyList;  //���� �ʿ� �����ϴ� ��� ���� ����

    [SerializeField]
    private PlayerHp playerHp;      //�÷��̾��� ü�� ������Ʈ

    [SerializeField]
    private GameObject enemyHpSliderPrefab; //�� ü���� ��Ÿ���� Slider UI ������
    [SerializeField]
    private Transform canvasTransform;      //UI�� ǥ���ϴ� Canvas ������Ʈ�� Transform

    [SerializeField]
    private PlayerGold playerGold;  //�÷��̾��� ��� ������Ʈ

    private Wave currentWave;   //���� ���̺� ����
    private int currentEnemyCount;  //���� ���̺꿡 �����ִ� �� ����(���̺� ���� �� max �� ��� �� -1)

    //���� ������ ������ EnemySpawner���� �ϱ� ������ set�� �ʿ����.
    public List<Enemy> EnemyList => enemyList;

    public int CurrentEnemyCount => currentEnemyCount;  //���� ���̺꿡 �����ִ� ��
    public int MaxEnemyCount => currentWave.maxEnemyCount;  //�ִ� �� ����

    private void Awake()
    {
        // �� ����Ʈ �޸� �Ҵ�
        enemyList = new List<Enemy>();
        // �� ���� �ڷ�ƾ �Լ� ȣ��
        //StartCoroutine("SpawnEnemy");
    }

    public void StartWave(Wave wave)
    {
        //�Ű������� �޾ƿ� ���̺� ���� ����
        currentWave = wave;
        //���� ���̺��� �ִ� �� ���� ����
        currentEnemyCount = currentWave.maxEnemyCount;
        //���� ���̺� ����
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy()
    {
        //���� ���̺꿡�� ������ �� ����
        int spawnEnemyCount = 0;

        //���� ���̺꿡�� �����Ǿ�� �ϴ� ���� ���ڸ�ŭ ���� �����ϰ� �ڷ�ƾ ����
        while(spawnEnemyCount < currentWave.maxEnemyCount)
        {
            //���̺꿡 �����ϴ� ���� ������ ���� ������ �� ������ ���� �����ϵ��� �����ϰ�, �� ������Ʈ ����
            int enemyIndex = Random.Range(0, currentWave.enemyPrefabs.Length);
            GameObject clone = Instantiate(currentWave.enemyPrefabs[enemyIndex]);
            Enemy enemy = clone.GetComponent<Enemy>();  //��� ������ ���� Enemy������Ʈ

            //this�� �� �ڽ�(�ڽ��� EnemySpawner ������Ʈ ����)
            enemy.SetUp(this, wayPoints);       //wayPoints ������ �Ű������� SetUp() ȣ��
            enemyList.Add(enemy);               // ����Ʈ�� ��� ������ ���� ���� ����

            SpawnEnemyHpSlider(clone);          //�� ü���� ��Ÿ���� Slider UI ���� �� ����

            spawnEnemyCount++;                  //���� ���̺꿡�� ������ �� ���� ����

            //�� ���̺긶�� spawnTime�� �ٸ� �� �ֱ� ������ ���� ���̺��� spawnTime ���
            yield return new WaitForSeconds(currentWave.spawnTime);
        }

        /*while (true)
        {
            GameObject clone = Instantiate(enemyPrefab);    //�� ������Ʈ ����
            Enemy enemy = clone.GetComponent<Enemy>();      //��� ������ �� ������Ʈ�� Enemy ������Ʈ

            enemy.SetUp(this, wayPoints);                         //wayPoints ������ �Ű������� SetUp() ȣ��
            enemyList.Add(enemy);                           // ����Ʈ�� ��� ������ ���� ���� ����

            SpawnEnemyHpSlider(clone);                      //�� ü���� ��Ÿ���� Slider UI ���� �� ����

            yield return new WaitForSeconds(spawnTime);     //spawnTime �ð����� ���
        }*/
    }

    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy, int gold)
    {
        //���� ��ǥ �������� �������� ��
        if(type == EnemyDestroyType.Arrive)
        {
            //�÷��̾� ü�� 1 ����
            playerHp.TakeDamage(1);
        }
        //���� �÷��̾��� �߻�ü�� ����ϸ�
        else if(type == EnemyDestroyType.Kill)
        {
            //��� ȹ��
            playerGold.CurrentGold += gold;
        }

        //���� ����� ������ ���� ���̺��� ���� �� ���� ����(UI ǥ�ÿ�)
        currentEnemyCount--;
        //����Ʈ�� ����ϴ� �� ���� ����
        enemyList.Remove(enemy);
        //�� ������Ʈ ����
        Destroy(enemy.gameObject);
    }

    private void SpawnEnemyHpSlider(GameObject enemy)
    {
        //�� ü���� ��Ÿ���� �����̴� ui ����
        GameObject sliderClone = Instantiate(enemyHpSliderPrefab);

        //Slider UI ������Ʈ�� parent(Canvas ������Ʈ)�� �ڽ����� ����
        //UI�� Canvas�� �ڽ����� �����Ǿ� �־�� ȭ�鿡 ���δ�.
        sliderClone.transform.SetParent(canvasTransform);

        //���� �������� �ٲ� ũ�⸦ (1, 1, 1)�� ����
        sliderClone.transform.localScale = Vector3.one;

        //Slider UI�� �i�ƴٴ� ����� �������� ����
        sliderClone.GetComponent<SliderPositionAutoSetter>().SetUp(enemy.transform);

        //Slider UI�� �ڽ��� ü�� ������ ǥ���ϵ��� ����
        sliderClone.GetComponent<EnemyHpViewer>().SetUp(enemy.GetComponent<EnemyHp>());
    }
}
