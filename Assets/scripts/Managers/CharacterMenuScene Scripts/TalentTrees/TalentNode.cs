using UnityEngine;

[CreateAssetMenu(fileName = "NewTalentNode", menuName = "TalentTrees/TalentNode")]
public class TalentNode : ScriptableObject
{
    [Header("Talent Node Configuration")]
    [Tooltip("The name of the talent node.")]
    public string nodeName;

    [Tooltip("Description of the talent node.")]
    [TextArea]
    public string description;

    [Tooltip("Current points allocated to this node.")]
    public int currentPoints;

    [Tooltip("Maximum points allowed for this node.")]
    public int maxPoints;
}
