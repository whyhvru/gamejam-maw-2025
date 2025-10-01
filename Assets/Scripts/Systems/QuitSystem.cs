using UnityEngine;

public class QuitSystem : MonoBehaviour
{
    [SerializeField] private InputRouter _inputRouter;

    private void OnEnable()
    {
        _inputRouter.OnQuit += Quit;
    }

    private void OnDisable()
    {
        _inputRouter.OnQuit -= Quit;
    }

    private void Quit()
    {
        Application.Quit();
    }
}