using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "NewWeaponTalent", menuName = "TalentTrees/WeaponTalent")]
public class WeaponTalent : ScriptableObject
{
    [Header("Talent Tree Configuration")]
    [Tooltip("Maximum points allowed in the talent tree.")]
    public int maxPoints = 5;

    [Tooltip("List of talent tiers.")]
    public List<TalentTier> tiers;

    public int TotalPoints
    {
        get
        {
            return tiers.Sum(tier => tier.nodes.Sum(node => node.currentPoints));
        }
    }

    public bool CanAddPointToNode(TalentNode node)
    {
        if (TotalPoints >= maxPoints)
            return false;

        foreach (var tier in tiers)
        {
            if (tier.nodes.Contains(node) && node.currentPoints < tier.maxPointsPerNode)
                return true;
        }

        return false;
    }
}

[System.Serializable]
public class TalentTier
{
    [Tooltip("Talent nodes in this tier.")]
    public List<TalentNode> nodes;

    [Tooltip("Maximum points allowed per node in this tier.")]
    public int maxPointsPerNode;
}
