using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState
{
    FreeRoam,
    Dialog,
    Shop
}

public class GameController : MonoBehaviour
{
    GameState state;
    GameObject player;

    [SerializeField]
    PlayerControl playerControl;

    [SerializeField]
    MerchantControl merchantControl;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };
        DialogManager.Instance.OnHideDialog += () =>
        {
            state = GameState.FreeRoam;
        };
        ShopManager.Instance.OnOpenShop += () =>
        {
            state = GameState.Shop;
        };
        ShopManager.Instance.OnCloseShop += () =>
        {
            state = GameState.FreeRoam;
        };

        AudioManager.PlayOnRepeat(AudioFileName.BGM);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == GameState.FreeRoam)
        {
            if (player != null)
            {
                player.SetActive(true);
            }
            playerControl.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            player.SetActive(false);
            DialogManager.Instance.HandleUpdate();
        }
        else if (state == GameState.Shop)
        {
            player.SetActive(false);
            ShopManager.Instance.HandleUpdate();
        }
    }
}
