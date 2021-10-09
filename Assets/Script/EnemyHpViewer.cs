using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpViewer : MonoBehaviour
{
    private EnemyHp enemyHp;
    private Slider hpSlider;

    public void SetUp(EnemyHp enemyHp)
    {
        this.enemyHp = enemyHp;
        hpSlider = GetComponent<Slider>();
    }

    private void Update()
    {
        hpSlider.value = enemyHp.CurrentHp / enemyHp.MaxHp;
    }
}
