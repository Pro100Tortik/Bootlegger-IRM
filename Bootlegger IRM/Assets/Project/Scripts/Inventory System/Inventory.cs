using System.Collections.Generic;
using UnityEngine;

namespace Bootlegger
{
    public class Inventory : SaveableBehaviour
    {
        public ReactiveProperty<int> SelectedSlot { get; private set; } = new();

        public IReadOnlyCollection<InventorySlot> InventorySlots => slots;
        [SerializeField] private InventorySlot[] slots = new InventorySlot[9];
        private bool[] _slotInputs = new bool[9];

        private void Awake()
        {
            foreach (var slot in slots)
            {
                slot.MaxAmount = 10;
            }
        }

        public void UpdateInput(InventoryInput input)
        {
            _slotInputs[0] = input.Slot1;
            _slotInputs[1] = input.Slot2;
            _slotInputs[2] = input.Slot3;
            _slotInputs[3] = input.Slot4;
            _slotInputs[4] = input.Slot5;
            _slotInputs[5] = input.Slot6;
            _slotInputs[6] = input.Slot7;
            _slotInputs[7] = input.Slot8;
            _slotInputs[8] = input.Slot9;
        }

        protected override void OnFixedUpdate()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (_slotInputs[i])
                {
                    SelectedSlot.Value = i;
                    break;
                }
            }


        }
    }
}
