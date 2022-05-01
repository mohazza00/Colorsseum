using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Skills : MonoBehaviour
{
    public Player player;

    [Header("Skill 1")]
    public Item skill_1;
    public float skill_1_coolDownTimer;
    public float skill_1_activeTimer;
    private bool skill_1_active;
    private bool skill_1_coolingDown;
    public TextMeshProUGUI skill_1_timer;
    public Image skill_1_image;
    public SpriteRenderer skill_1_activated;

    [Header("Skill 2")]
    public Item skill_2;
    public float skill_2_coolDownTimer;
    public float skill_2_activeTimer;
    private bool skill_2_active;
    private bool skill_2_coolingDown;
    public TextMeshProUGUI skill_2_timer;
    public Image skill_2_image;
    public SpriteRenderer skill_2_activated;

    [Header("Skill 3")]
    public Item skill_3;
    public float skill_3_coolDownTimer;
    public float skill_3_activeTimer;
    private bool skill_3_active;
    private bool skill_3_coolingDown;
    public TextMeshProUGUI skill_3_timer;
    public Image skill_3_image;
    public SpriteRenderer skill_3_activated;

    [Header("Skill 4")]
    public Item skill_4;
    public float skill_4_coolDownTimer;
    public float skill_4_activeTimer;
    private bool skill_4_active;
    private bool skill_4_coolingDown;
    public TextMeshProUGUI skill_4_timer;
    public Image skill_4_image;
    public SpriteRenderer skill_4_activated;

    private bool blinking;
    private float blinkingTimer;
    private bool blinkOn;

    private void Awake()
    {
        player = GetComponent<Player>();
    }
    private void Update()
    {
        if (GameManager.Instance.gameState == GameState.SPANWING_WAVE || GameManager.Instance.gameState == GameState.SPAWNING_MINIONS_WAVE)
            return;

        CheckInput();
        Blinking();
        SkillsCoolDown();
    }

    private void CheckInput()
    {
        if (skill_1 != null)
        {
            if(!skill_1_coolingDown && !skill_1_active)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    UseItem(skill_1, 0);
                }
            }      
        }

        if (skill_2 != null)
        {
            if (!skill_2_coolingDown && !skill_2_active)
            {
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    UseItem(skill_2, 1);
                }
            }
        }

        if (skill_3 != null)
        {
            if (!skill_3_coolingDown && !skill_3_active)
            {
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    UseItem(skill_3, 2);
                }
            }
        }

        if (skill_4 != null)
        {
            if (!skill_4_coolingDown && !skill_4_active)
            {
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    UseItem(skill_4, 3);
                }
            }
        }
    }

    public void UseItem(Item skill, int slot)
    {
        AudioManager.Instance.PlaySound(SoundName.USE_ITEM);

        switch (skill.type)
        {
            case ItemType.POTION:
                UsePotion(skill, slot);
                break;

            case ItemType.FREEZE_WATCH:
                StartCoroutine(_ActivateFreeze(skill, slot));
                break;

            case ItemType.COLOR_BOMB:
                StartCoroutine(_ActivateBomb(skill, slot));
                break;

            case ItemType.SHIELD:
                StartCoroutine(_ActivateShield(skill, slot));
                break;

            case ItemType.ARROWS_MAGNET:
                StartCoroutine(_ActivateMagnet(skill, slot));
                break;

            case ItemType.DOUBLE_DASH:
                StartCoroutine(_ActivateDoubleDash(skill, slot));
                break;

            case ItemType.INFINITE_ARROWS:
                StartCoroutine(_ActivateInfiniteArrows(skill, slot));
                break;

            case ItemType.FORCE_FIELD:
                StartCoroutine(_ActivateForceField(skill, slot));
                break;
        }
    }

    private void Blinking()
    {
        if(blinking)
        {
            if(blinkingTimer <= 0f)
            {
                blinkingTimer = 0.2f;
                blinkOn = !blinkOn;
            }
            else
            {
                blinkingTimer -= Time.deltaTime;
            }

            if(blinkOn)
            {
                player.sprite.color = new Color(1, 1, 1, 0);
            }
            else
            {
                player.sprite.color = new Color(1, 1, 1, 1);
            }
        }
    }

    private void SkillsCoolDown()
    {
        if (skill_1_coolingDown)
        {
            if (skill_1_coolDownTimer <= 0)
            {
                skill_1_coolingDown = false;
                skill_1_timer.gameObject.SetActive(false);
                skill_1_image.color = new Color(1, 1, 1, 1f);
            }
            else
            {
                skill_1_coolDownTimer -= Time.deltaTime;
                skill_1_timer.text = skill_1_coolDownTimer.ToString("0.0");
            }
        }

        if (skill_2_coolingDown)
        {
            if (skill_2_coolDownTimer <= 0)
            {
                skill_2_coolingDown = false;
                skill_2_timer.gameObject.SetActive(false);
                skill_2_image.color = new Color(1, 1, 1, 1f);
            }
            else
            {
                skill_2_coolDownTimer -= Time.deltaTime;
                skill_2_timer.text = skill_2_coolDownTimer.ToString("0.0");
            }
        }

        if (skill_3_coolingDown)
        {
            if (skill_3_coolDownTimer <= 0)
            {
                skill_3_coolingDown = false;
                skill_3_timer.gameObject.SetActive(false);
                skill_3_image.color = new Color(1, 1, 1, 1f);
            }
            else
            {
                skill_3_coolDownTimer -= Time.deltaTime;
                skill_3_timer.text = skill_3_coolDownTimer.ToString("0.0");
            }
        }

        if (skill_4_coolingDown)
        {
            if (skill_4_coolDownTimer <= 0)
            {
                skill_4_coolingDown = false;
                skill_4_timer.gameObject.SetActive(false);
                skill_4_image.color = new Color(1, 1, 1, 1f);
            }
            else
            {
                skill_4_coolDownTimer -= Time.deltaTime;
                skill_4_timer.text = skill_4_coolDownTimer.ToString("0.0");
            }
        }
    }

    private void UsePotion(Item item, int slot)
    {
        player.damageDetector.RegainHealth(5);
        player.inventory.itemsSlots[slot].slotUI.sprite = null;
        player.inventory.itemsSlots[slot].slotUI.color = new Color(1, 1, 1, 0);
        player.inventory.itemsSlots[slot].isOccupied = false;
        player.inventory.purchasedItems.Remove(item);

        if (slot == 0) skill_1 = null;
        if (slot == 1) skill_2 = null;
        if (slot == 2) skill_3 = null;
        if (slot == 3) skill_4 = null;

    }

    IEnumerator _ActivateShield(Item skill, int slot)
    {
        player.damageDetector.invincible = true;
        blinking = true;
        blinkOn = true;

        ActivateSkill(skill, slot);

        yield return new WaitForSeconds(skill.duration);
        player.damageDetector.invincible = false;
        blinking = false;
        player.sprite.color = new Color(1, 1, 1, 1);

        DeactivateSkill(skill, slot);
    }

    IEnumerator _ActivateDoubleDash(Item skill, int slot)
    {
        player.canDoubleDash = true;

        ActivateSkill(skill, slot);

        yield return new WaitForSeconds(skill.duration);
        player.canDoubleDash = false;

        DeactivateSkill(skill, slot);

    }

    IEnumerator _ActivateMagnet(Item skill, int slot)
    {
        GameManager.Instance.pullArrows = true;

        ActivateSkill(skill, slot);

        yield return new WaitForSeconds(skill.duration);

        GameManager.Instance.pullArrows = false;

        DeactivateSkill(skill, slot);
    }

    IEnumerator _ActivateFreeze(Item skill, int slot)
    {
        GameManager.Instance.freezeEnemies = true;

        ActivateSkill(skill, slot);

        yield return new WaitForSeconds(skill.duration);
        GameManager.Instance.freezeEnemies = false;

        DeactivateSkill(skill, slot);

    }

    IEnumerator _ActivateBomb(Item skill, int slot)
    {
        GameManager.Instance.changeColor = true;
        GameManager.Instance.randomColor = GetRandomColor();

        ActivateSkill(skill, slot);

        yield return new WaitForSeconds(skill.duration);
        GameManager.Instance.changeColor = false;
        GameManager.Instance.randomColor = UnitColor.NONE;

        DeactivateSkill(skill, slot);

    }

    IEnumerator _ActivateInfiniteArrows(Item skill, int slot)
    {
        GameManager.Instance.infiniteArrows = true;
        GameManager.Instance.infiniteArrowIndex = player.shooting.currentArrow;
        player.inventory.FillArrowsFully(GameManager.Instance.infiniteArrowIndex);

        ActivateSkill(skill, slot);

        yield return new WaitForSeconds(skill.duration);
        GameManager.Instance.infiniteArrows = false;
        player.inventory.EmptyArrows(GameManager.Instance.infiniteArrowIndex);


        DeactivateSkill(skill, slot);

    }

    IEnumerator _ActivateForceField(Item skill, int slot)
    {
        PoolingManager.Instance.SpawnObj(PoolObjectType.FORCE, transform.position, null);
        GameManager.Instance.forceField.SetActive(true);

        ActivateSkill(skill, slot);

        yield return new WaitForSeconds(skill.duration);
        GameManager.Instance.forceField.SetActive(false);


        DeactivateSkill(skill, slot);

    }
    private UnitColor GetRandomColor()
    {
        int rand = Random.Range(0, 4);

        if (rand == 0) return UnitColor.BLUE;
        if (rand == 1) return UnitColor.RED;
        if (rand == 2) return UnitColor.GREEN;
        if (rand == 3) return UnitColor.YELLOW;

        return UnitColor.NONE;
    }

    private void ActivateSkill(Item skill, int slot)
    {
        if (slot == 0)
        {
            skill_1_active = true;
            skill_1_activated.gameObject.SetActive(true);
            skill_1_activated.sprite = skill_1.sprite;
        }
        if (slot == 1)
        {
            skill_2_active = true;
            skill_2_activated.gameObject.SetActive(true);
            skill_2_activated.sprite = skill_2.sprite;
        }
        if (slot == 2)
        {
            skill_3_active = true;
            skill_3_activated.gameObject.SetActive(true);
            skill_3_activated.sprite = skill_3.sprite;
        }
        if (slot == 3)
        {
            skill_4_active = true;
            skill_4_activated.gameObject.SetActive(true);
            skill_4_activated.sprite = skill_4.sprite;
        }
    }

    private void DeactivateSkill(Item skill, int slot)
    {
        if (slot == 0)
        {
            skill_1_active = false;
            skill_1_coolDownTimer = skill.coolDown;
            skill_1_coolingDown = true;
            skill_1_timer.gameObject.SetActive(true);
            skill_1_image.color = new Color(1, 1, 1, 0.6f);
            skill_1_activated.gameObject.SetActive(false);
            skill_1_activated.sprite = null;
        }

        if (slot == 1)
        {
            skill_2_active = false;
            skill_2_coolDownTimer = skill.coolDown;
            skill_2_coolingDown = true;
            skill_2_timer.gameObject.SetActive(true);
            skill_2_image.color = new Color(1, 1, 1, 0.6f);
            skill_2_activated.gameObject.SetActive(false);
            skill_2_activated.sprite = null;
        }

        if (slot == 2)
        {
            skill_3_active = false;
            skill_3_coolDownTimer = skill.coolDown;
            skill_3_coolingDown = true;
            skill_3_timer.gameObject.SetActive(true);
            skill_3_image.color = new Color(1, 1, 1, 0.6f);
            skill_3_activated.gameObject.SetActive(false);
            skill_3_activated.sprite = null;
        }

        if (slot == 3)
        {
            skill_4_active = false;
            skill_4_coolDownTimer = skill.coolDown;
            skill_4_coolingDown = true;
            skill_4_timer.gameObject.SetActive(true);
            skill_4_image.color = new Color(1, 1, 1, 0.6f);
            skill_4_activated.gameObject.SetActive(false);
            skill_4_activated.sprite = null;
        }
    }

}
