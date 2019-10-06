using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBase : MonoBehaviour
{
    protected bool isInit;
    public Vector2Int mapCoords;
    SpriteRenderer shadow;
    public bool showShadow = true;
    public float shadowSize = 1;
    public Vector2 pivotOffset;
    public float yPositionOffsetForComparePosition = .5f;

    protected List<Renderer> renderers = new List<Renderer>();
    List<int> originalSortingOrders = new List<int>();

    public Vector3 worldPosition; //z для прыжка

    public void Init()
    {
        UpdatePivotOffsetFromCollider();
    }

    protected virtual void Start()
    {
        Init();
        InitRenederers();
    }

    protected void InitRenederers()
    {
        Renderer[] renderersComps = GetComponentsInChildren<Renderer>(true);

        for (int i = 0; i < renderersComps.Length; i++)
        {
            AddRenderer(renderersComps[i]);
        }
    }

    void UpdatePivotOffsetFromCollider()
    {
        Collider2D collider = GetComponentInChildren<Collider2D>();

        if (collider != null)
        {
            pivotOffset.y = collider.offset.y;

            CircleCollider2D circle = collider as CircleCollider2D;

            if (circle != null)
            {
                pivotOffset.y -= circle.radius;
            }
            else
            {
                BoxCollider2D box = collider as BoxCollider2D;

                if (box != null)
                {
                    pivotOffset.y -= box.size.y / 2f;
                }
                else
                {
                    CapsuleCollider2D capsule = collider as CapsuleCollider2D;
                    if (capsule != null)
                    {
                        pivotOffset.y -= capsule.size.y / 2f;
                    }
                }
            }
        }
    }

    public void AddRenderer(Renderer renderer)
    {
        if (renderers.Contains(renderer))
            return;
        renderers.Add(renderer);
        originalSortingOrders.Add(renderer.sortingOrder);
        int index = renderers.IndexOf(renderer);
        UpdateSortingOrder(index);
    }

    public void DeleteRederer(Renderer renderer)
    {
        int index = renderers.IndexOf(renderer);
        renderers.RemoveAt(index);
        originalSortingOrders.RemoveAt(index);
    }

    public void ChangeSortingOrder(Renderer renderer, int sortingOrder)
    {
        int index = renderers.IndexOf(renderer);

        if (index != -1)
        {
            originalSortingOrders[index] = sortingOrder;
            UpdateSortingOrders();
            // UpdateSortingOrder(index);
        }
    }

    public Vector2 GetPivotPosition()
    {
        return new Vector2(worldPosition.x, worldPosition.y);
    }

    public void MoveTo(Vector2 position, float zJumpPosition = 0, bool needClampToWorldEndPoints = true)
    {
        if (Mathf.Round(worldPosition.y * 10) != Mathf.Round(position.y * 10))
        {
            worldPosition = new Vector3(position.x, position.y, zJumpPosition);
            UpdateSortingOrders();
        }
        else
        {
            worldPosition = new Vector3(position.x, position.y, zJumpPosition);
        }

        if (needClampToWorldEndPoints)
        {
            worldPosition.x = Mathf.Clamp(worldPosition.x, MapsController.Ins.GetCurrentWorldEndPoints().x, MapsController.Ins.GetCurrentWorldEndPoints().y);
            worldPosition.y = Mathf.Clamp(worldPosition.y, MapsController.Ins.GetCurrentWorldUpDownEndPoints().x, MapsController.Ins.GetCurrentWorldUpDownEndPoints().y);
        }

        transform.position = new Vector3(worldPosition.x + pivotOffset.x, worldPosition.y - pivotOffset.y + worldPosition.z);

        UpdateShadow(showShadow, position);
    }

    protected void UpdateShadow(bool show, Vector2 position = default)
    {
        if (show)
        {
            if (shadow == null)
            {
                // pivotOffset = transform.Find("Pivot").transform.localPosition;
                shadow = Instantiate(GameAssets.Get.pfEntityShadow.gameObject).GetComponent<SpriteRenderer>();
                shadow.transform.SetParent(transform, false);
                shadow.transform.localScale = shadow.transform.localScale * shadowSize;
            }

            shadow.gameObject.SetActive(true);
            shadow.transform.position = position;
        }
        else
        {
            if (shadow == null)
                return;

            shadow.gameObject.SetActive(false);
        }
    }

    void UpdateSortingOrders()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            UpdateSortingOrder(i);
        }
    }

    void UpdateSortingOrder(int index)
    {
        if (renderers[index] != null)
            renderers[index].sortingOrder = GetCurrentSortingOrder() + originalSortingOrders[index];
    }

    public void UpdateSortingOrder(int index, float yPosition)
    {
        if (renderers[index] != null)
            renderers[index].sortingOrder = GetSortingOrder(yPosition) + originalSortingOrders[index];
    }

    public int GetSortingOrder(float yPosition)
    {
        return Mathf.Abs((int)(-yPosition * 10 + MapsController.Ins.GetCurrentWorldUpDownEndPoints().y) * 10);
    }

    public int GetCurrentSortingOrder()
    {
        return Mathf.Abs((int)(-worldPosition.y * 10 + MapsController.Ins.GetCurrentWorldUpDownEndPoints().y) * 10);
    }

    public bool CompareEntitiesPositions(EntityBase eb)
    {
        return CompareEntitiesPositions(eb.worldPosition, eb.yPositionOffsetForComparePosition);
    }

    public bool CompareEntitiesPositions(Vector3 worldPosition, float yPositionOffset = .5f)
    {
        if (Mathf.Abs(this.worldPosition.y - worldPosition.y) < yPositionOffset)
            return true;

        return false;
    }
}