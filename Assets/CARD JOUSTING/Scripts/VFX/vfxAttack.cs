using UnityEngine;
using DG.Tweening;

public class vfxAttack : MonoBehaviour
{
    public void MoveTo(Transform attackerTransform)
    {
        transform.position = attackerTransform.position;
        float randomSide = Random.Range(0, 1f);
        bool left = randomSide < 0.5f ? true : false;
        transform.DOMove(transform.position + new Vector3(left == true ? -1f : 1f, 0, 2), 0.2f).OnComplete(() => Destroy(gameObject));
    }
}
