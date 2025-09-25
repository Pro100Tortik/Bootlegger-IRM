using UnityEngine;

namespace Bootlegger
{
    [CreateAssetMenu(fileName = "GameResourcesSO", menuName = "Scriptable Objects/Create Game Resources")]
    public class GameResourcesSO : ScriptableObject
    {
        [field: SerializeField] public LayerMask InteractionMask { get; private set; }
    }
}