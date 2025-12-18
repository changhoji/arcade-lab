using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

public class SceneLoader
{
    readonly LifetimeScope parent;

    public SceneLoader(LifetimeScope lifetimeScope)
    {
        parent = lifetimeScope;
    }

    public IEnumerator LoadSceneAsync(string sceneName)
    {
        using (LifetimeScope.EnqueueParent(parent))
        {
            var loading = SceneManager.LoadSceneAsync(sceneName);
            while (!loading.isDone)
            {
                yield return null;
            }
        }
    }
}
