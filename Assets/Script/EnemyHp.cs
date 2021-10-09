using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHp : MonoBehaviour
{
    [SerializeField]
    private float maxHp;            //�ִ� ü��
    private float currentHp;        //���� ü��
    private bool isDie = false;     //���� ������¸� isDie�� true�� �ٲ�
    private Enemy enemy;
    private SpriteRenderer spriteRenderer;

    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;

    private void Awake()
    {
        currentHp = maxHp;          //���� ü���� �ִ� ü������ �ٲ�
        enemy = GetComponent<Enemy>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float damage)
    {
        if (isDie == true) return;

        currentHp -= damage;

        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");

        if (currentHp <= 0)
        {
            isDie = true;
            enemy.OnDie(EnemyDestroyType.Kill);
        }
    }

    private IEnumerator HitAlphaAnimation()
    {
        // ���� ���� ������ color�� ����
        Color color = spriteRenderer.color;

        //���� ������ 40%�� ����
        color.a = 0.4f;
        spriteRenderer.color = color;

        //0.05�ʰ� ���
        yield return new WaitForSeconds(0.05f);

        //���� ������ 100%�� ����
        color.a = 1.0f;
        spriteRenderer.color = color;
    }
}
