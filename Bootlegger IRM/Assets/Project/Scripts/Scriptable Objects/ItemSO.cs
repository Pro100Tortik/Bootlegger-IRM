using UnityEngine;
using UnityEngine.Localization;

namespace Bootlegger
{
    [CreateAssetMenu(fileName = "ItemSO", menuName = "Inventory System/Create New Item")]
    public class ItemSO : ScriptableObject
    {
        [field: SerializeField] public LocalizedString LocalizedName { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }
    }
}
