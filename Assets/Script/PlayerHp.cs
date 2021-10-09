using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHp : MonoBehaviour
{
    [SerializeField]
    private Image imageScreen;

    [SerializeField]
    private float maxHp = 20;               //�ִ�ü��
    private float currentHp;                //����ü��

    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;

    private void Awake()
    {
        currentHp = maxHp;                  //���� ü���� �ִ� ü�°� ���� ����
    }

    public void TakeDamage(float damage)
    {
        //���� ü���� damage��ŭ ����
        currentHp -= damage;

        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");

        //ü���� 0�� �Ǹ� ���ӿ���
        if(currentHp <= 0)
        {

        }
    }

    private IEnumerator HitAlphaAnimation()
    {
        //��ü ȭ�鿡 ��ġ�� imageScreen�� ������ color ������ ����
        //imageScreen�� ������ 40%�� ����
        Color color = imageScreen.color;
        color.a = 0.4f;
        imageScreen.color = color;

        //������ 0%�� �ɶ����� ���� ����
        while(color.a >= 0.0f)
        {
            color.a -= Time.deltaTime;
            imageScreen.color = color;

            yield return null;
        }
    }
}
