using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace XReal.XTown.Yacht
{
    public class DiceManager : MonoBehaviour
    {
        public static DiceScript[] dices;
        public UnityEvent onRollingFinish;

        /*
        [SerializeField]
        private DiceScript _dicePrefab;
        */

        // Start is called before the first frame update
        void Awake()
        {
            dices = transform.GetComponentsInChildren<DiceScript>();
            int diceIndex = 0;
            foreach (var dice in dices)
            {
                dice.diceIndex = diceIndex;
                diceIndex += 1;
            }
        }



        public void OnInitialze()
        {
            /* Multiplay */
            if (GameManager.multiplayMode && (!GameManager.IsMyTurn || GameManager.currentTurn <= 0)) return;
            /*
            for(int i = 0; i < 5; ++i)
            {
                DiceScript dice = Instantiate(_dicePrefab).GetComponent<DiceSctipt>();
                dice.diceIndex = i;
            }
            */


            List<DiceInfo> diceInfoList = DiceScript.diceInfoList;

            foreach (DiceInfo diceInfo in diceInfoList)
            {
                if (GameManager.currentGameState == GameState.ready)
                {
                    diceInfo.diceNumber = 0;
                    diceInfo.rolling = false;
                    diceInfo.keeping = false;
                    diceInfo.sortedIndex = 0;
                }
                else if (GameManager.currentGameState == GameState.shaking)
                {
                    if (diceInfo.keeping == false)
                    {
                        diceInfo.diceNumber = 0;
                    }
                }
            }

            Debug.Log("INIT DiceManager");
        }

        public void OnReadyStart()
        {
            foreach (var dice in dices)
            {
                if (dice.diceInfo.keeping == false)
                {
                    dice.Ready();
                }
            }
        }


        public void OnRollingStart()
        {
            foreach (var dice in dices)
            {
                if (dice.diceInfo.keeping == false)
                {
                    // Roll dices!!
                    dice.Roll();
                }
            }
        }


        public void OnRollingFinish()
        {
            var sortedList = DiceScript.diceInfoList.OrderBy(x => x.diceNumber).ToList();

            int i = 0;
            foreach (DiceInfo sortedDiceInfo in sortedList)
            {
                DiceInfo diceInfo = DiceScript.diceInfoList.Where(x => x.diceIndex == sortedDiceInfo.diceIndex).First();
                diceInfo.sortedIndex = i;
                i += 1;
            }
            var sortedUnkeptList = sortedList.Where(x => x.keeping == false).ToList();
            StartCoroutine(DiceRollFinish(sortedUnkeptList));
        }

        public void OnFinish()
        {
            var sortedList = DiceScript.diceInfoList.OrderBy(x => x.diceNumber).ToList();

            int i = 0;
            foreach (DiceInfo sortedDiceInfo in sortedList)
            {
                DiceInfo diceInfo = DiceScript.diceInfoList.Where(x => x.diceIndex == sortedDiceInfo.diceIndex).First();
                diceInfo.sortedIndex = i;
                i += 1;
            }

            var sortedUnkeptList = sortedList.Where(x => x.keeping == false).ToList();
            StartCoroutine(TurnFinish(sortedUnkeptList));
        }

        IEnumerator DiceRollFinish(List<DiceInfo> sortedUnkeptList)
        {
            foreach (DiceInfo diceInfo in sortedUnkeptList)
            {
                int i = diceInfo.diceIndex;
                dices[i].OnRollingFinish();
                yield return new WaitForSecondsRealtime(0.05f);
            }
        }

        IEnumerator TurnFinish(List<DiceInfo> sortedUnkeptList)
        {
            foreach (DiceInfo diceInfo in sortedUnkeptList)
            {
                PickedSlotController.instance.PutIntoEmptySlot(diceInfo.diceIndex);
                yield return new WaitForSecondsRealtime(0.05f);
            }
            

        }
    }
}
