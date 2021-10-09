using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpwaner : MonoBehaviour
{
    /*[SerializeField]
    private GameObject enemyPrefab; //적 프리팹
    [SerializeField]
    private float spawnTime;        //적 생성주기*/

    [SerializeField]
    private Transform[] wayPoints;  //현재 스테이지의 이동 경로
    private List<Enemy> enemyList;  //현재 맵에 존재하는 모든 적의 정보

    [SerializeField]
    private PlayerHp playerHp;      //플레이어의 체력 컴포넌트

    [SerializeField]
    private GameObject enemyHpSliderPrefab; //적 체력을 나타내는 Slider UI 프리팹
    [SerializeField]
    private Transform canvasTransform;      //UI를 표현하는 Canvas 오브젝트의 Transform

    [SerializeField]
    private PlayerGold playerGold;  //플레이어의 골드 컴포넌트

    private Wave currentWave;   //현재 웨이브 정보
    private int currentEnemyCount;  //현재 웨이브에 남아있는 적 숫자(웨이브 시작 시 max 적 사망 시 -1)

    //적의 생성과 삭제는 EnemySpawner에서 하기 때문에 set은 필요없다.
    public List<Enemy> EnemyList => enemyList;

    public int CurrentEnemyCount => currentEnemyCount;  //현재 웨이브에 남아있는 적
    public int MaxEnemyCount => currentWave.maxEnemyCount;  //최대 적 숫자

    private void Awake()
    {
        // 적 리스트 메모리 할당
        enemyList = new List<Enemy>();
        // 적 생성 코루틴 함수 호출
        //StartCoroutine("SpawnEnemy");
    }

    public void StartWave(Wave wave)
    {
        //매개변수로 받아온 웨이브 정보 저장
        currentWave = wave;
        //현재 웨이브의 최대 적 숫자 저장
        currentEnemyCount = currentWave.maxEnemyCount;
        //현재 웨이브 시작
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy()
    {
        //현재 웨이브에서 생성한 적 숫자
        int spawnEnemyCount = 0;

        //현재 웨이브에서 생성되어야 하는 적의 숫자만큼 적을 생성하고 코루틴 종료
        while(spawnEnemyCount < currentWave.maxEnemyCount)
        {
            //웨이브에 등장하는 적의 종류가 여러 종류일 때 임의의 적이 등장하도록 설정하고, 적 오브젝트 생성
            int enemyIndex = Random.Range(0, currentWave.enemyPrefabs.Length);
            GameObject clone = Instantiate(currentWave.enemyPrefabs[enemyIndex]);
            Enemy enemy = clone.GetComponent<Enemy>();  //방금 생성된 적의 Enemy컴포넌트

            //this는 나 자신(자신의 EnemySpawner 컴포넌트 정보)
            enemy.SetUp(this, wayPoints);       //wayPoints 정보를 매개변수로 SetUp() 호출
            enemyList.Add(enemy);               // 리스트에 방금 생성된 적의 정보 저장

            SpawnEnemyHpSlider(clone);          //적 체력을 나타내는 Slider UI 생성 및 설정

            spawnEnemyCount++;                  //현재 웨이브에서 생성한 적 숫자 증가

            //각 웨이브마다 spawnTime이 다를 수 있기 때문에 현재 웨이브의 spawnTime 사용
            yield return new WaitForSeconds(currentWave.spawnTime);
        }

        /*while (true)
        {
            GameObject clone = Instantiate(enemyPrefab);    //적 오브젝트 생성
            Enemy enemy = clone.GetComponent<Enemy>();      //방금 생성된 적 오브젝트의 Enemy 컴포넌트

            enemy.SetUp(this, wayPoints);                         //wayPoints 정보를 매개변수로 SetUp() 호출
            enemyList.Add(enemy);                           // 리스트에 방금 생성된 적의 정보 저장

            SpawnEnemyHpSlider(clone);                      //적 체력을 나타내는 Slider UI 생성 및 설정

            yield return new WaitForSeconds(spawnTime);     //spawnTime 시간동안 대기
        }*/
    }

    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy, int gold)
    {
        //적이 목표 지점까지 도착했을 때
        if(type == EnemyDestroyType.Arrive)
        {
            //플레이어 체력 1 감소
            playerHp.TakeDamage(1);
        }
        //적이 플레이어의 발사체에 사망하면
        else if(type == EnemyDestroyType.Kill)
        {
            //골드 획득
            playerGold.CurrentGold += gold;
        }

        //적이 사망할 때마다 현재 웨이브의 생존 적 숫자 감소(UI 표시용)
        currentEnemyCount--;
        //리스트에 사망하는 적 정보 삭제
        enemyList.Remove(enemy);
        //적 오브젝트 삭제
        Destroy(enemy.gameObject);
    }

    private void SpawnEnemyHpSlider(GameObject enemy)
    {
        //적 체력을 나타내는 슬라이더 ui 생성
        GameObject sliderClone = Instantiate(enemyHpSliderPrefab);

        //Slider UI 오브젝트를 parent(Canvas 오브젝트)의 자식으로 설정
        //UI는 Canvas의 자식으로 설정되어 있어야 화면에 보인다.
        sliderClone.transform.SetParent(canvasTransform);

        //계층 설정으로 바뀐 크기를 (1, 1, 1)로 설정
        sliderClone.transform.localScale = Vector3.one;

        //Slider UI가 쫒아다닐 대상을 본인으로 설정
        sliderClone.GetComponent<SliderPositionAutoSetter>().SetUp(enemy.transform);

        //Slider UI에 자신의 체력 정보를 표시하도록 설정
        sliderClone.GetComponent<EnemyHpViewer>().SetUp(enemy.GetComponent<EnemyHp>());
    }
}
