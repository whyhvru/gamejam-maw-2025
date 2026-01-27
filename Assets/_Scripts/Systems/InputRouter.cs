using System;
using UnityEngine;

public class InputRouter : MonoBehaviour
{
    public Action OnHoldEars;               // Q
    public Action OnReleaseEars;

    public Action OnStartWalk;             // W
    public Action OnStopWalk;

    public Action OnWhistleStart;          // E
    public Action OnWhistleStop;

    public Action OnToggleBreath;          // R

    public Action OnAttack;                // A
    public Action OnRetreat;              // S
    public Action OnTakeSedative;         // D
    public Action OnUseFlint;     // F

    public Action OnHeartbeatVisual;      // V
    public Action OnQuit;

    private void Update()
    {
        // Q — закрытие ушей
        if (Input.GetKeyDown(KeyCode.Q)) OnHoldEars?.Invoke();
        if (Input.GetKeyUp(KeyCode.Q)) OnReleaseEars?.Invoke();

        // W — движение вперёд
        if (Input.GetKeyDown(KeyCode.W)) OnStartWalk?.Invoke();
        if (Input.GetKeyUp(KeyCode.W)) OnStopWalk?.Invoke();

        // E — свист (удержание)
        if (Input.GetKeyDown(KeyCode.E)) OnWhistleStart?.Invoke();
        if (Input.GetKeyUp(KeyCode.E)) OnWhistleStop?.Invoke();

        // R — переключение дыхания
        if (Input.GetKeyDown(KeyCode.R)) OnToggleBreath?.Invoke();

        // A — атака
        if (Input.GetKeyDown(KeyCode.A)) OnAttack?.Invoke();

        // S — резкий отступ
        if (Input.GetKeyDown(KeyCode.S)) OnRetreat?.Invoke();

        // D — успокоительное
        if (Input.GetKeyDown(KeyCode.D)) OnTakeSedative?.Invoke();

        // F — фонарик
        if (Input.GetKeyDown(KeyCode.F)) OnUseFlint?.Invoke();

        // TAB — сердцебиение
        if (Input.GetKeyDown(KeyCode.Tab)) OnHeartbeatVisual?.Invoke();

        if (Input.GetKeyDown(KeyCode.Return)) OnQuit?.Invoke();
    }
}
