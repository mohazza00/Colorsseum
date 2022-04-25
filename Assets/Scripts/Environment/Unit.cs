using UnityEngine;

public enum UnitColor
{
    NONE,
    GREEN,
    YELLOW,
    RED,
    BLUE,
    ENEMY,
}

public class Unit : MonoBehaviour
{
    public UnitColor unitColor;
    public EnemyType enemyType;
    public SpriteRenderer sprite;
    public SpriteRenderer minimapIcon;

    public UnitColor originalUnitColor;
    public Color originalColor;

    private bool colorChanged;

    private void Start()
    {
        if(unitColor != UnitColor.NONE && enemyType != EnemyType.TUTORIAL)
            ChangeColor(unitColor);
        originalUnitColor = unitColor;
        originalColor = sprite.color;
    }

    private void Update()
    {
        if (unitColor == UnitColor.NONE)
            return;

        if(!colorChanged)
        {
            if (GameManager.Instance.changeColor)
            {
                ChangeColor(GameManager.Instance.randomColor);
                colorChanged = true;
            }
        }
        if(colorChanged)
        {
            if (!GameManager.Instance.changeColor)
            {
                GoBackToNormalColors();
                colorChanged = false;
            }
        }
        
    }

    public void ChangeColor(UnitColor color)
    {
        switch (color)
        {
            case UnitColor.NONE:
                sprite.color = Color.white;
                minimapIcon.color = Color.white;
                break;

            case UnitColor.BLUE:
                unitColor = color;
                sprite.color = GameManager.Instance.blue;
                minimapIcon.color = sprite.color;
                break;

            case UnitColor.RED:
                unitColor = color;
                sprite.color = GameManager.Instance.red;
                minimapIcon.color = sprite.color;
                break;

            case UnitColor.GREEN:
                unitColor = color;
                sprite.color = GameManager.Instance.green;
                minimapIcon.color = sprite.color;
                break;

            case UnitColor.YELLOW:
                unitColor = color;
                sprite.color = GameManager.Instance.yellow;
                minimapIcon.color = sprite.color;
                break;
        }
    }

    public void GoBackToNormalColors()
    {
        sprite.color = originalColor;
        unitColor = originalUnitColor;
        minimapIcon.color = originalColor;
    }

    public static Quaternion SetArrowRotation(Vector2 dir)
    {
       // Debug.Log(dir);
        if (dir.x == 0)
        {
            if (dir.y == 1)
                return Quaternion.Euler(0f, 0f, 0f);
            if (dir.y == -1)
                return Quaternion.Euler(0f, 0f, 180f);
        }

        if (dir.y == 0)
        {
            if (dir.x == 1)
                return Quaternion.Euler(0f, 0f, -90f);
            if (dir.x == -1)
                return Quaternion.Euler(0f, 0f, 90f);
        }

        if (dir.x >= 0.2f && dir.y >= 0.2f)
        {
            return Quaternion.Euler(0f, 0f, -45f);
        }

        if (dir.x <= -0.2f && dir.y <= -0.2f)
        {
            return Quaternion.Euler(0f, 0f, 135f);
        }

        if (dir.x <= -0.2f && dir.y >= 0.2f)
        {
            return Quaternion.Euler(0f, 0f, 45f);
        }

        if (dir.x >= 0.2f && dir.y <= -0.2f)
        {
            return Quaternion.Euler(0f, 0f, -135f);
        }

        return Quaternion.identity;
    }
}
