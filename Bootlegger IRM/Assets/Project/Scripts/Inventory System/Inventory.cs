using System.Collections.Generic;
using UnityEngine;

namespace Bootlegger
{
    public class Inventory : SaveableBehaviour
    {
        public ReactiveProperty<int> SelectedSlot { get; private set; } = new();
    }
}
