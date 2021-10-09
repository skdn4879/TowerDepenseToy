using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerDataViewer : MonoBehaviour
{
    [SerializeField]
    private Image imageTower;
    [SerializeField]
    private TextMeshProUGUI textDamage;
    [SerializeField]
    private TextMeshProUGUI textRate;
    [SerializeField]
    private TextMeshProUGUI textRange;
    [SerializeField]
    private TextMeshProUGUI textLevel;

    [SerializeField]
    private TowerAttackRange towerAttackRange;

    [SerializeField]
    private Button buttonUpgrade;

    [SerializeField]
    private SystemTextViewer systemTextViewer;

    [SerializeField]
    private TextMeshProUGUI textUpgradeCost;
    [SerializeField]
    private TextMeshProUGUI textSellCost;

    private TowerWeapon currentTower;

    private void Awake()
    {
        OffPanel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OffPanel();
        }
    }

    public void OnPanel(Transform towerWeapon)
    {
        currentTower = towerWeapon.GetComponent<TowerWeapon>();
        //타워 정보 패널 On
        gameObject.SetActive(true);
        UpdateTowerData();
        //타워 오브젝트 주변에 표시되는 타워 공격 범위 Sprite On
        towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.Range);
    }

    public void OffPanel()
    {
        //타워 정보 패널 Off
        gameObject.SetActive(false);
        //타워 공격 범위 Sprite Off
        towerAttackRange.OffAttackRange();
    }

    private void UpdateTowerData()
    {
        if(currentTower.WeaponType == WeaponType.Cannon || currentTower.WeaponType == WeaponType.Laser)
        {
            imageTower.rectTransform.sizeDelta = new Vector2(100, 60);
            textDamage.text = "Damage : " + currentTower.Damage
                + " + " + "<color=red>" + currentTower.AddedDamage.ToString("F1") + "</color>";
        }
        else
        {
            imageTower.rectTransform.sizeDelta = new Vector2(60, 60);

            if(currentTower.WeaponType == WeaponType.Slow)
            {
                textDamage.text = "Slow : " + currentTower.Slow * 100 + "%";
            }
            else if(currentTower.WeaponType == WeaponType.Buff)
            {
                textDamage.text = "Buff : " + currentTower.Buff * 100 + "%";
            }
        }

        imageTower.sprite = currentTower.TowerSprite;
        //textDamage.text = "Damage : " + currentTower.Damage;
        textRate.text = "Rate : " + currentTower.Rate;
        textRange.text = "Range : " + currentTower.Range;
        textLevel.text = "Level : " + currentTower.Level;
        textUpgradeCost.text = currentTower.UpgradeCost.ToString();
        textSellCost.text = currentTower.SellCost.ToString();

        //타워의 레벨이 최대 레벨이면 버튼 비활성화
        buttonUpgrade.interactable = currentTower.Level < currentTower.MaxLevel ? true : false;
    }

    public void OnClickEventTowerUpgrade()
    {
        //타워 업그레이드 시도(성공: true, 실패: false)
        bool isSuccess = currentTower.Upgrade();

        if(isSuccess == true)
        {
            //타워가 업그레이드 되었기 때문에 타워 정보 갱신
            UpdateTowerData();
            //타워 주변에 보이는 공격 범위도 갱신
            towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.Range);
        }
        else
        {
            //타워 업그레이드에 필요한 골드가 부족하다 출력
            systemTextViewer.PrintText(SystemType.Money);
        }
    }

    public void OnClickEventTowerSell()
    {
        //타워 판매
        currentTower.Sell();
        //선택한 타워가 사라져서 Panel, 타워 범위 Off
        OffPanel();
    }
}
