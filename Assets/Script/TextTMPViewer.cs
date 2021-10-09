using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextTMPViewer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textPlayerHP;   // Text - TextMeshPro UI [�÷��̾� ü��]

    [SerializeField]
    private TextMeshProUGUI textPlayerGold; // Text - TextMeshPro UI [�÷��̾� ���]

    [SerializeField]
    private TextMeshProUGUI textWave;       // Text - TextMeshPro UI [���� ���̺� / �� ���̺�]

    [SerializeField]
    private TextMeshProUGUI textEnemyCount; // Text - TextMeshPro UI [���� �� ���� / �ִ� �� ����]

    [SerializeField]
    private PlayerHp playerHP;              //�÷��̾� ü�� ����

    [SerializeField]
    private PlayerGold playerGold;          //�÷��̾� ��� ����

    [SerializeField]
    private WaveSystem waveSystem;          //���̺� ����

    [SerializeField]
    private EnemySpwaner enemySpwaner;      //�� ����

    private void Update()
    {
        textPlayerHP.text = playerHP.CurrentHp + "/" + playerHP.MaxHp;
        textPlayerGold.text = playerGold.CurrentGold.ToString();
        textWave.text = waveSystem.CurrentWave + "/" + waveSystem.MaxWave;
        textEnemyCount.text = enemySpwaner.CurrentEnemyCount + "/" + enemySpwaner.MaxEnemyCount;
    }
}
