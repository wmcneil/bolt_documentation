# Bolt and PoolManagers

To use a Pool Manager (or intercept Bolt's `Instantiate` and `Destroy` calls for any reason), use `BoltNetwork.SetInstantiateDestroyCallbacks`.

```C#
BoltNetwork.SetInstantiateDestroyCallbacks((prefab, position, rotation) => {
    GameObject go = Instantiate(prefab, position, rotation);
    return go;
}, (gameObject) => {
    Destroy(gameObject);
});
```
