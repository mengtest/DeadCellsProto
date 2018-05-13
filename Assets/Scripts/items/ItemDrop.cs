﻿using UnityEngine;

[ExecuteInEditMode]
public class ItemDrop : MonoBehaviour {
    private static string itemIconsPath = "Assets/Sprites/GameElements/cardIcons.png";
    private SpriteRenderer renderR;

    public GameObject popUp;
    public GameObject itemSelect;
    public Vector3 popOffset = new Vector3(0, 0.2f, -4.5f);
    [ShowOnly] public GameObject popUpInst;
    [ShowOnly] public GameObject itemSelectInst;

    public Equipment item;

    [ShowOnly] public Collider2D inside = null;

    private void Start() {
        SpriteLoader.loadSpritesFrom(itemIconsPath);
        if (item != null) {
            name = "ItemDrop[" + item.name + "]";
            renderR = GetComponent<SpriteRenderer>();
            renderR.sprite = SpriteLoader.getSprite(item.spriteName);
        }
    }

    public void changeItem(Equipment equipment) {
        if (equipment != null) {
            item = equipment;
            name = "ItemDrop[" + item.name + "]";
            renderR = GetComponent<SpriteRenderer>();
            renderR.sprite = SpriteLoader.getSprite(item.spriteName);
        }
    }

    private void PickUp(Collider2D other) {
        if (!other.CompareTag("Player"))
            return;

        Transform playerTransform = Headless.instance.transform;
        Equipment equip = Instantiate(item, playerTransform.position, playerTransform.rotation);
        if (!Headless.instance.inventory.addItemToInventory(equip)) {
            itemSelectInst = Instantiate(itemSelect, CanvasManager.instance.gameObject.transform);
            ItemSwitcher switcher = itemSelectInst.GetComponent<ItemSwitcher>();
            switcher.setOnGround(equip, this);
        } else {
            Destroy(gameObject);
        }
    }

    public void Sell() {
    }

//    protected abstract void OnPickUp(Collider2D other);

    private void OnTriggerEnter2D(Collider2D other) {
        if(!other.CompareTag("Player") || Headless.instance.inventory.watching || CanvasManager.instance == null || popUpInst != null)
            return;
        inside = other;
        popUpInst = Instantiate(popUp, CanvasManager.instance.gameObject.transform);
        Headless.instance.inventory.watching = true;
        popUpInst.GetComponent<RectTransform>().position = transform.position + popOffset;
    }

    private void Update() {
        if (inside != null && Input.GetButtonDown(InputManager.INTERACT)) {
            PickUp(inside);
            Headless.instance.inventory.watching = false;
            Destroy(popUpInst);
            popUpInst = null;
            inside = null;
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(!other.CompareTag("Player") || CanvasManager.instance == null)
            return;
        if (popUpInst == null && !Headless.instance.inventory.watching) {
            inside = other;
            popUpInst = Instantiate(popUp, CanvasManager.instance.gameObject.transform);
            Headless.instance.inventory.watching = true;
        }
        if (popUpInst != null) {
            popUpInst.GetComponent<RectTransform>().position = transform.position + popOffset;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(!other.CompareTag("Player") || popUpInst == null)
            return;
        inside = null;
        Headless.instance.inventory.watching = false;
        Destroy(popUpInst);
        popUpInst = null;
    }
}