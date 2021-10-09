using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyDestroyType { Kill = 0, Arrive}  //���� ��� ���� ����

public class Enemy : MonoBehaviour
{
    private int wayPointCount;      //�̵� ��� ����
    private Transform[] wayPoints;  //�̵� ��� ����
    private int currentIndex = 0;   //���� ��ǥ���� �ε���
    private Movement2D movement2D;  //������Ʈ �̵� ����
    private EnemySpwaner enemySpwaner;  //���� ������ ������ �����ʰ� EnemySpawner�� �˷��� ����

    [SerializeField]
    private int gold = 10;      //�� ����� ȹ�� ���� ���

    public void SetUp(EnemySpwaner enemySpwaner, Transform[] wayPoints)
    {
        movement2D = GetComponent<Movement2D>();

        this.enemySpwaner = enemySpwaner;

        // �� �̵���� wayPoints ����
        wayPointCount = wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount];
        this.wayPoints = wayPoints;

        // ���� ��ġ�� ù ��° wayPoint ��ġ�� ����
        //ó������ ���� watPoint�� Start Ÿ���� ��ġ���� ���� �����ǰ� �ϱ�����
        transform.position = wayPoints[currentIndex].position;

        // �� �̵�/��ǥ���� ���� �ڷ�ƾ �Լ� ����
        StartCoroutine("OnMove");
    }

    private IEnumerator OnMove()
    {
        //���� �̵� ���� ����
        NextMoveTo();

        while (true)
        {
            // �� ������Ʈ ȸ��
            transform.Rotate(Vector3.forward * 10);

            //���� ������ġ�� ��ǥ��ġ�� �Ÿ��� 0.02f * movement2D.MoveSpeed ���� ���� �� if�� ����
            //movement2D.MoveSpeed�� �����ִ� ������ �ӵ��� ������ �� �����ӿ� 0.02���� ũ�� �����̱� ������
            //if���� �ɸ����ʰ� ��θ� Ż���ϴ� ������Ʈ�� ���� �� �ִ�.
            if (Vector3.Distance(transform.position, wayPoints[currentIndex].position) < 0.02f * movement2D.MoveSpeed)
            {
                // ���� �̵� ���� ����
                NextMoveTo();
            }

            yield return null;
        }
    }

    private void NextMoveTo()
    {
        //���� �̵��� wayPoint�� �����ִٸ�
        if(currentIndex < wayPointCount - 1)
        {
            // ���� ��ġ�� ��Ȯ�ϰ� ��ǥ��ġ�� ����
            transform.position = wayPoints[currentIndex].position;
            // �̵� ���� ���� => ���� ��ǥ����(wayPoints)
            currentIndex++;
            Vector3 direction = (wayPoints[currentIndex].position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        else //������ ��ġ���
        {
            //�� ������ ���� �����ϸ� ��� ȹ�� ����
            gold = 0;
            // �� ������Ʈ ����
            //Destroy(gameObject);
            OnDie(EnemyDestroyType.Arrive);
        }
    }

    public void OnDie(EnemyDestroyType type)
    {
        //EnemySpawner���� ����Ʈ�� �� ������ �����ϱ� ������ Destroy()�� ����
        //�����ʰ� EnemySpawner���� ������ ������ �� �ʿ��� ó���� �ϵ��� DestoryEnemy()�Լ� ȣ��
        enemySpwaner.DestroyEnemy(type, this, gold);
    }
}
