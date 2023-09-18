﻿using UnityEngine;

public abstract class Task
{
    public static Task Instance;

    public delegate void OnCompletedHandler(TaskStarter.Availability state);
    public OnCompletedHandler OnCompleted;
    public OnCompletedHandler OnForcefullyStopped;

    public delegate void OnStartedHandler();
    public OnStartedHandler OnStarted;

    protected TaskStarter _caller;
    protected GameObject _prefabInstance;

    public static Dad Dad;
    protected static Transform _root;
    public static Transform TaskGUI;

    protected Task()
    {
        InitializeSubscriptions();
        InitializeReferences();
    }
    private void InitializeSubscriptions()
    {
        OnStarted += HandleOnStarted;
        OnCompleted += HandleOnCompleted;
    }
    private static void InitializeReferences()
    {
        if (Dad is null)
            Dad = GameObject.FindGameObjectWithTag("Player").GetComponent<Dad>();

        if (_root is null)
            _root = GameObject.FindGameObjectWithTag("SceneObjects").transform;

        if (TaskGUI is null)
            TaskGUI = GameObject.FindGameObjectWithTag("TaskGUI").transform;
    }
    private void HandleOnStarted()
    {
        Instance = this;
        Dad.PlayerStateMachine.UpdateState(new TaskState(Dad, _caller.Data));
        Setup();
    }
    private void HandleOnCompleted(TaskStarter.Availability state)
    {
        if (_caller is null)
            return;

        if (_caller.Interval == DayCycle.TimeInterval.All)
        {
            Finalize();
            return;
        }
        _caller.SetAvailabilityState(state);
        Finalize();

        void Finalize()
        {
            Dad.PlayerStateMachine.UpdateState(Dad.MovingState);

            Clear();
            Reset();
        }
    }
    protected void HandleOnStateChanged(TaskStarter.Availability state)
    {
        if (_caller is null)
            return;

        _caller.OnStateChanged -= HandleOnStateChanged;
        if (state != TaskStarter.Availability.Late)
            return;

        ForcefullyStop(state);
    }
    public virtual void Start(TaskStarter caller)
    {
        if (_caller == caller)
            return;

        _caller = caller;
        _caller.TryHideHint();

        if (_caller.Interval != DayCycle.TimeInterval.All)
            _caller.OnStateChanged += HandleOnStateChanged;

        OnStarted?.Invoke();
    }
    protected virtual void Setup() => _prefabInstance = _caller.InstantiatePrefab(_root);
    protected virtual void Clear()
    {
        Object.Destroy(_prefabInstance);
        for (int i = 0; i < TaskGUI.childCount; i++)
            Object.Destroy(TaskGUI.GetChild(i).gameObject);
    }
    public virtual void Stop(TaskStarter caller, TaskStarter.Availability state) => OnCompleted?.Invoke(state);
    public virtual void ForcefullyStop(TaskStarter.Availability state)
    {
        OnForcefullyStopped?.Invoke(state);
        Stop(_caller, state);
    }
    public virtual void ForcefullyStop(TaskStarter.Availability state, ButtonState buttonState)
    {
        if (buttonState == ButtonState.Hold)
            return;

        ForcefullyStop(state);
    }
    public virtual void Reset()
    {
        OnStarted = null;
        OnCompleted = null;
        OnForcefullyStopped = null;
        _caller = null;

        InitializeSubscriptions();
    }
}