using TMPro;
using UnityEngine;

namespace Sherko.Utils
{
    [ExecuteAlways]
    [RequireComponent(typeof(TMP_Text))]
    public class VersionUI : MonoBehaviour
    {
        private TMP_Text _tmp;
    
        private void Awake()
        {
            _tmp = GetComponent<TMP_Text>();
            _tmp.text = "v" + Application.version;
        }
    }
}

