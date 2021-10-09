using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyDestroyType { Kill = 0, Arrive}  //적의 사망 형태 구분

public class Enemy : MonoBehaviour
{
    private int wayPointCount;      //이동 경로 개수
    private Transform[] wayPoints;  //이동 경로 정보
    private int currentIndex = 0;   //현재 목표지점 인덱스
    private Movement2D movement2D;  //오브젝트 이동 제어
    private EnemySpwaner enemySpwaner;  //적의 삭제를 본인이 하지않고 EnemySpawner에 알려서 삭제

    [SerializeField]
    private int gold = 10;      //적 사망시 획득 가능 골드

    public void SetUp(EnemySpwaner enemySpwaner, Transform[] wayPoints)
    {
        movement2D = GetComponent<Movement2D>();

        this.enemySpwaner = enemySpwaner;

        // 적 이동경로 wayPoints 설정
        wayPointCount = wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount];
        this.wayPoints = wayPoints;

        // 적의 위치를 첫 번째 wayPoint 위치로 설정
        //처음으로 찍은 watPoint인 Start 타일의 위치에서 적이 생성되게 하기위해
        transform.position = wayPoints[currentIndex].position;

        // 적 이동/목표지점 설정 코루틴 함수 시작
        StartCoroutine("OnMove");
    }

    private IEnumerator OnMove()
    {
        //다음 이동 방향 설정
        NextMoveTo();

        while (true)
        {
            // 적 오브젝트 회전
            transform.Rotate(Vector3.forward * 10);

            //적의 현재위치와 목표위치의 거리가 0.02f * movement2D.MoveSpeed 보다 작을 때 if문 실행
            //movement2D.MoveSpeed를 곱해주는 이유는 속도가 빠르면 한 프레임에 0.02보다 크게 움직이기 때문에
            //if문에 걸리지않고 경로를 탈주하는 오브젝트가 있을 수 있다.
            if (Vector3.Distance(transform.position, wayPoints[currentIndex].position) < 0.02f * movement2D.MoveSpeed)
            {
                // 다음 이동 방향 설정
                NextMoveTo();
            }

            yield return null;
        }
    }

    private void NextMoveTo()
    {
        //아직 이동할 wayPoint가 남아있다면
        if(currentIndex < wayPointCount - 1)
        {
            // 적의 위치를 정확하게 목표위치로 설정
            transform.position = wayPoints[currentIndex].position;
            // 이동 방향 설정 => 다음 목표지점(wayPoints)
            currentIndex++;
            Vector3 direction = (wayPoints[currentIndex].position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        else //마지막 위치라면
        {
            //골 지점에 적이 도착하면 골드 획득 없음
            gold = 0;
            // 적 오브젝트 삭제
            //Destroy(gameObject);
            OnDie(EnemyDestroyType.Arrive);
        }
    }

    public void OnDie(EnemyDestroyType type)
    {
        //EnemySpawner에서 리스트로 적 정보를 관리하기 때문에 Destroy()를 직접
        //하지않고 EnemySpawner에게 본인이 삭제될 때 필요한 처리를 하도록 DestoryEnemy()함수 호출
        enemySpwaner.DestroyEnemy(type, this, gold);
    }
}
