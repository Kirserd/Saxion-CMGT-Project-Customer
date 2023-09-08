﻿using UnityEngine;
public abstract class Task
{
    public delegate void OnCompletedHandler(bool result);
    public OnCompletedHandler OnCompleted;

    public delegate void OnStartedHandler();
    public OnStartedHandler OnStarted;

    protected TaskData _data;
    private Dad _dad;
    protected Task()
    {
        OnStarted += HandleOnStarted;
        OnCompleted += ctx => HandleOnCompleted(ctx);
        _dad = GameObject.FindGameObjectWithTag("Player").GetComponent<Dad>();
    }
    private void HandleOnStarted() => _dad.PlayerStateMachine.UpdateState(_dad.TaskState);
    private void HandleOnCompleted(bool result)
    {
        if (result)
        {
            _dad = GameObject.FindGameObjectWithTag("Player").GetComponent<Dad>();
            _dad.AddPoints(_data.IsGame, _data.Points);
        }
    }

    public virtual void Start(TaskStarter caller) => OnStarted?.Invoke();
    public virtual void Stop(TaskStarter caller, bool result) => OnCompleted?.Invoke(result);
    public abstract void ForcefullyStop();
}