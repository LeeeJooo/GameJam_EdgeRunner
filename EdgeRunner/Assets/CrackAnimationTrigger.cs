using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackAnimationTrigger : MonoBehaviour
{
    private void DestroyTrigger()
    {
        Destroy(transform.parent.gameObject);
    }
}
