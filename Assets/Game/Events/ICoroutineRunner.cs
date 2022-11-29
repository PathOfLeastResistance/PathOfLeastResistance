using System.Collections;
using UnityEngine;

public interface ICoroutineRunner
{
    public Coroutine StartCoroutine(IEnumerator routine);
    public void StopCoroutine(IEnumerator routine);
}
