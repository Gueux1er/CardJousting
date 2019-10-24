using UnityEngine;
using DG.Tweening;

public class vfxAttack : MonoBehaviour
{
    private void Start()
    {
        float randomSide = Random.Range(0, 1f);
        bool left = randomSide < 0.5f ? true : false;
        transform.DOMove(transform.position + new Vector3(left == true ? -1f : 1f, 0, 2), 0.2f).OnComplete(() => Destroy(gameObject));
    }
}
