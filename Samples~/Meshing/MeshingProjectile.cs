// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class MeshingProjectile : MonoBehaviour
{
    private float lifeTime;
    private ObjectPool<MeshingProjectile> activeObjectPool;
    public Rigidbody rb;
    private Coroutine lifeTimeCoroutine;
    public void Initialize(ObjectPool<MeshingProjectile> pool, float duration)
    {
        activeObjectPool = pool;
        lifeTime = duration;
        
        if (lifeTimeCoroutine != null)
        {
            StopCoroutine(lifeTimeCoroutine);
        }
        lifeTimeCoroutine = StartCoroutine(LifetimeCoroutine());
    }

    private IEnumerator LifetimeCoroutine()
    {
        yield return new WaitForSeconds(lifeTime);
        lifeTimeCoroutine = null;
        activeObjectPool.Release(this);
    }
}
