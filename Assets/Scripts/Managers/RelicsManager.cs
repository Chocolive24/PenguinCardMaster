using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class RelicsManager : MonoBehaviour
{
    private List<RelicData> _relicDataList;

    private int _emplIdx = 0;
    
    // References ------------------------------------------------------------------------------------------------------
    [SerializeField] private RelicInventoryData _relicInventoryData;
    
    [SerializeField] private RectTransform[] _relicEmplacements;

    [SerializeField] private RectTransform[] _rewardLocation;

    [SerializeField] private GameObject _relicInfoGameObject;
    [SerializeField] private TextMeshProUGUI _relicInfoTxt;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------
    public bool IsInventoryFull => _relicInventoryData.RelicData.Count >= _relicEmplacements.Length;
    public GameObject RelicInfoPos => _relicInfoGameObject;

    public TextMeshProUGUI RelicInfoTxt
    {
        get => _relicInfoTxt;
        set => _relicInfoTxt = value;
    }


    // Methods ---------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        _relicDataList = Resources.LoadAll<RelicData>("Relics").ToList();

        Collectible.OnCollected += AddRelicWithData;
    }

    private void OnDestroy()
    {
        Collectible.OnCollected -= AddRelicWithData;
    }

    private void Start()
    {
        InstantiateRelics();
    }

    private void InstantiateRelics()
    {
        foreach (var relicData in _relicInventoryData.RelicData)
        {
            var relic = Instantiate(relicData.RelicPrefab);
            
            AddRelicWithoutData(relic);
        }
    }

    public Relic GetARandomRelic(List<RelicData> relicDataList)
    {
        return relicDataList.OrderBy(x => Random.value).First().RelicPrefab;
    }

    public Relic GetARelicByType(RelicData.RelicType type)
    {
        return _relicDataList.Where(relic => relic.Type == type).
            OrderBy(x => Random.value).First().RelicPrefab;
    }

    public Relic GetARelicByComponent()
    {
        foreach (var relic in _relicDataList)
        {
            if (relic.RelicPrefab.TryGetComponent(out Boots boots))
            {
                return boots;
            }
        }

        return null;
    }

    public void AddRelicWithData(Collectible collectible)
    {
        Relic relic = collectible.GetComponent<Relic>();

        if (relic)
        {
            relic.EffectBox.SetActive(false);

            if (_emplIdx <= _relicEmplacements.Length)
            {
                relic.transform.position = _relicEmplacements[_emplIdx].position;
                relic.transform.parent = _relicEmplacements[_emplIdx].transform;

                relic.GetComponent<RectTransform>().sizeDelta = relic.BaseSizeDelta;
                
                relic.IsCollected = true;
            
                _relicInventoryData.AddRelicData(relic.RelicDataRef);
            
                _emplIdx++;
            }
        }
    }
    
    public void AddRelicWithoutData(Relic relic)
    {
        relic.EffectBox.SetActive(false);
        
        if (_emplIdx <= _relicEmplacements.Length)
        {
            relic.transform.position = _relicEmplacements[_emplIdx].position;
            relic.transform.parent = _relicEmplacements[_emplIdx].transform;
            
            relic.GetComponent<RectTransform>().sizeDelta = relic.BaseSizeDelta;
            
            relic.IsCollected = true;
            
            _emplIdx++;
        }
    }
    
    public List<Relic> CreateRelicRewards()
    {
        List<Relic> relics = new List<Relic>();
        
        List<RelicData> alreadySpawnedRelics = Resources.LoadAll<RelicData>("Relics").ToList();

        var relic1 = Instantiate(GetARandomRelic(alreadySpawnedRelics),
            _rewardLocation[0].position, Quaternion.identity, _rewardLocation[0].transform);

        relics.Add(relic1);
        alreadySpawnedRelics.Remove(relic1.RelicDataRef);

        var relic2 = Instantiate(GetARandomRelic(alreadySpawnedRelics),
            _rewardLocation[1].position, Quaternion.identity, _rewardLocation[1].transform);
        
        relics.Add(relic2);
        alreadySpawnedRelics.Remove(relic2.RelicDataRef);

        var relic3 = Instantiate(GetARandomRelic(alreadySpawnedRelics),
            _rewardLocation[2].position, Quaternion.identity, _rewardLocation[2].transform);
        
        relics.Add(relic3);
        alreadySpawnedRelics.Remove(relic3.RelicDataRef);
        
        foreach (var relic in relics)
        {
            relic.GetComponent<RectTransform>().sizeDelta = 2 * relic.BaseSizeDelta;
            
            //relic.OnCollected += AddRelicWithData;
        }
        
        return relics;
    }
}
