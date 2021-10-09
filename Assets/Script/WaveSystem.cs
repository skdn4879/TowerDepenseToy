using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [SerializeField]
    private Wave[] waves;                   //���� ���������� ��� ���̺� ����

    [SerializeField]
    private EnemySpwaner enemySpwaner;
    private int currentWaveIndex = -1;      //���� ���̺� �ε���

    //���̺� ���� ����� ���� get ������Ƽ (���� ���̺�, �� ���̺�)
    public int CurrentWave => currentWaveIndex + 1;     //������ 0�̱� ������ + 1
    public int MaxWave => waves.Length;

    public void StartWave()
    {
        //���� �ʿ� ���� ����, ���̺갡 ����������
        if(enemySpwaner.EnemyList.Count == 0 && currentWaveIndex < waves.Length - 1)
        {
            //�ε����� ������ -1�̹Ƿ� �ε����� ������ ���� ��
            currentWaveIndex++;
            //EnemySpawner�� StartWave �Լ� ȣ��, ���� ���̺� ���� ����
            enemySpwaner.StartWave(waves[currentWaveIndex]);
        }
    }
}

[System.Serializable]
public struct Wave
{
    public float spawnTime;             //���� ���̺� �� ���� �ֱ�
    public int maxEnemyCount;            //���� ���̺� �� ���� ����
    public GameObject[] enemyPrefabs;   //���� ���̺� �� ���� ����
}
