using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Collectible : MonoBehaviour
{
    public enum CollectibleType
    {
        REWARD,
        SHOP_OBJECT,
    }

    protected CollectibleType _collectibleType;
    
    protected int _objectCost;

    protected bool _isCollected;
    
    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<Collectible> OnBuy;
    public static event Action<Collectible> OnCollected;

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public int ObjectCost
    {
        get => _objectCost;
        set => _objectCost = value;
    }
    
    public bool IsCollected
    {
        get => _isCollected;
        set => _isCollected = value;
    }

    // Methods ---------------------------------------------------------------------------------------------------------
    public void Awake()
    {
        ShopManager.OnObjectBuy += SetToCollected;
    }

    public void OnDestroy()
    {
        ShopManager.OnObjectBuy -= SetToCollected;
    }

    public void OnCollectibleClick()
    {
        if (!_isCollected)
        {
            if (_collectibleType == CollectibleType.SHOP_OBJECT)
            {
                OnBuy?.Invoke(this);
            }
            
            else if (_collectibleType == CollectibleType.REWARD)
            {
                OnCollected?.Invoke(this);
            }
        }
    }

    public void Init(CollectibleType collectibleType, int cost)
    {
        _collectibleType = collectibleType;
        _objectCost = cost;
    }
    
    private void SetToCollected(ShopManager shopManager, Collectible obj)
    {
        if (obj == this)
        {
            OnCollected?.Invoke(this);
        }
    }
}
