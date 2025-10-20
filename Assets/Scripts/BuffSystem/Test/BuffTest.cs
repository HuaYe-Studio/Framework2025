using BuffSystem.BuffDesign;
using UnityEngine;

namespace BuffSystem.Test
{
    public sealed class BuffTest : MonoBehaviour
    {
        private BuffManager _buffManager;

        private void Awake()
        {
            _buffManager = GetComponent<BuffManager>();
        }

        private bool _inPoison;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                _inPoison = !_inPoison;
                if (_inPoison)
                {
                    _buffManager.AddBuff(BuffConstant.BuffConstant.BuffName.CAST_POISON, gameObject);
                    Debug.Log("施毒Buff已施加");
                }
                else
                {
                    _buffManager.RemoveBuff(BuffConstant.BuffConstant.BuffName.CAST_POISON);
                    Debug.Log("施毒Buff已移除");
                }
            }
        }
    }
}