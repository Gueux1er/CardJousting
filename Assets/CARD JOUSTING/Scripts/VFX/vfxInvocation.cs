using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class vfxInvocation : MonoBehaviour
{
    [SerializeField] GameObject m_bubble;
    Sequence m_sequence;
    void Start()
    {
        InstantiateBubbles();
    }

    private void InstantiateBubbles()
    {
        float rf00 = Random.Range(0f, 0.4f);
        m_sequence.Append(GetComponent<MeshRenderer>().material.DOFloat(0, "_explose", 0.4f));

        int r = Random.RandomRange(4, 8);
        for(int i = 0; i < r; i ++)
        {
            Vector2 offset = Random.insideUnitCircle;
            GameObject go = Instantiate(m_bubble, transform.position + new Vector3(offset.x,0,offset.y) , Quaternion.identity);
            float rScale = Random.RandomRange(0.15f, 0.5f);
            go.transform.parent = transform;
            go.transform.localScale = new Vector3(rScale, rScale, rScale);
            m_sequence.Append(go.GetComponent<MeshRenderer>().material.DOFloat(0, "_explose", 0.4f).SetDelay(i+1 * 0.15f).OnComplete(() => Destroy(go)));
        }
        m_sequence.OnComplete(() => Destroy(gameObject));
    }
}
