using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHp : MonoBehaviour
{
    [SerializeField]
    private Image imageScreen;

    [SerializeField]
    private float maxHp = 20;               //최대체력
    private float currentHp;                //현재체력

    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;

    private void Awake()
    {
        currentHp = maxHp;                  //현재 체력을 최대 체력과 같게 설정
    }

    public void TakeDamage(float damage)
    {
        //현재 체력을 damage만큼 감소
        currentHp -= damage;

        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");

        //체력이 0이 되면 게임오버
        if(currentHp <= 0)
        {

        }
    }

    private IEnumerator HitAlphaAnimation()
    {
        //전체 화면에 배치된 imageScreen의 색상을 color 변수에 저장
        //imageScreen의 투명도를 40%로 설정
        Color color = imageScreen.color;
        color.a = 0.4f;
        imageScreen.color = color;

        //투명도가 0%가 될때까지 투명도 감소
        while(color.a >= 0.0f)
        {
            color.a -= Time.deltaTime;
            imageScreen.color = color;

            yield return null;
        }
    }
}
