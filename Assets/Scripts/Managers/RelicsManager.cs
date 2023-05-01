using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RelicsManager : MonoBehaviour
{
    private List<RelicData> _relicDataList;

    private int _emplIdx = 0;
    
    // References ------------------------------------------------------------------------------------------------------
    [SerializeField] private RectTransform[] _relicEmplacements;

    [SerializeField] private RectTransform[] _rewardLocation;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------

    // Methods ---------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        _relicDataList = Resources.LoadAll<RelicData>("Relics").ToList();
    }

    public Relic GetARandomRelic(Relic withoutthisRelic)
    {
        return _relicDataList.Where(relic => relic.RelicPrefab != withoutthisRelic).
        OrderBy(x => Random.value).First().RelicPrefab;
    }

    public void AddRelicInInventory(Relic relic)
    {
        relic.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
        relic.EffectBox.SetActive(false);
        
        if (_emplIdx <= _relicEmplacements.Length)
        {
            relic.transform.position = _relicEmplacements[_emplIdx].position;
            relic.transform.parent = _relicEmplacements[_emplIdx].transform;

            _emplIdx++;
        }
    }

    public List<Relic> CreateRelicRewards()
    {
        List<Relic> relics = new List<Relic>();

        relics.Add(Instantiate(GetARandomRelic(null),
            _rewardLocation[0].position, Quaternion.identity, _rewardLocation[0].transform));
        relics.Add(Instantiate(GetARandomRelic(null),
            _rewardLocation[1].position, Quaternion.identity, _rewardLocation[1].transform));
        relics.Add(Instantiate(GetARandomRelic(null),
            _rewardLocation[2].position, Quaternion.identity, _rewardLocation[2].transform));

        foreach (var relic in relics)
        {
            relic.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            
            relic.OnCollected += AddRelicInInventory;
        }
        
        return relics;
    }
}