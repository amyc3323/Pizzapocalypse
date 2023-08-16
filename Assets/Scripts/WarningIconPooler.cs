using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningIconPooler : MonoBehaviour
{
    public static WarningIconPooler instance;
    public Queue<WarningIcon> pool;
    public WarningIcon warningIconPrefab;
    [SerializeField] private int poolStartSize;
    [SerializeField] private bool expands=false;
    private void Awake()
    {
        instance = this;
        for(int i = 0; i < poolStartSize; i++)
        {
            WarningIcon newW = Instantiate(warningIconPrefab, transform);
            newW.gameObject.SetActive(false);
            pool.Enqueue(newW);
        }
    }
    public WarningIcon GetWarningIcon()
    {
        if (pool.Count <= 0)
        {
            if(!expands) return null;
            else
            {
                WarningIcon newW = Instantiate(warningIconPrefab, transform);
                return newW;
            }
        }
        else {
            WarningIcon w=pool.Dequeue();
            w.gameObject.SetActive(true);
            return w;
        }
    }
    public void ReturnWarningIcon(WarningIcon warningIcon)
    {
        warningIcon.gameObject.SetActive(false);
        pool.Enqueue(warningIcon);
    }
}
