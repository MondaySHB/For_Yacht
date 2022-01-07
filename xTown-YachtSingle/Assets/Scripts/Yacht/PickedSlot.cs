using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XReal.XTown.Yacht
{
    public class PickedSlot : MonoBehaviour
    {
        public int slotIndex = 0;
        public bool occupied = false;
        public DiceScript dice = null;

        public void PutDiceInSlot(int diceIndex)
        {
            occupied = true;
            dice = DiceManager.dices[diceIndex];
            dice.OnPicked(transform);
        }

        public void TakeOutDice()
        {
            occupied = false;
            dice.OnTakeOut();
            dice = null;
        }
    }
}

