namespace Machinery
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public sealed class StateMachineTests
    {
        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateStateMachine()
        {
            // Arrange
            TestContext context = new();
            TestState initialState = new("Initial");

            // Act
            StateMachine<TestContext, string, TestState> stateMachine = new(context, initialState);

            // Assert
            Assert.NotNull(stateMachine);
            Assert.Same(context, stateMachine.Context);
            Assert.Same(initialState, stateMachine.CurrentState);
        }

        [Fact]
        public void Constructor_WithNullInitialState_ShouldThrowArgumentNullException()
        {
            // Arrange
            TestContext context = new();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new StateMachine<TestContext, string, TestState>(context, null!));
        }

        [Fact]
        public void StaticCreate_WithValidParameters_ShouldCreateStateMachine()
        {
            // Arrange
            TestContext context = new();
            TestState initialState = new("Initial");

            // Act
            var stateMachine = StateMachine<string>.Create(context, initialState);

            // Assert
            Assert.NotNull(stateMachine);
            Assert.Same(context, stateMachine.Context);
            Assert.Same(initialState, stateMachine.CurrentState);
        }

        [Fact]
        public void TryProcessEvent_WhenCalled_ShouldReturnTrue()
        {
            // Arrange
            TestContext context = new();
            TestState initialState = new("Initial");
            var stateMachine = StateMachine<string>.Create(context, initialState);

            // Act
            bool result = stateMachine.TryProcessEvent("Test");

            // Assert
            Assert.True(result);
            Assert.Equal(1, initialState.OnRemainCallCount); // Since our test state doesn't transition
        }

        [Fact]
        public void TryProcessEvent_WhenProcessingEventAlready_ShouldReturnFalse()
        {
            // Arrange
            TestContext context = new();
            ReentrantTestState initialState = new("Initial");
            var stateMachine = StateMachine<string>.Create(context, initialState);

            // Set the state machine reference in the state
            initialState.StateMachine = stateMachine;

            // Act - This will attempt to call TryProcessEvent recursively
            bool result = stateMachine.TryProcessEvent("Reentrant");

            // Assert
            Assert.True(result); // First call succeeds
            Assert.False(initialState.ReentrantCallResult); // Second call fails (reentrant)
        }

        [Fact]
        public void ProcessEvent_WhenCalled_ShouldReturnCorrectResult()
        {
            // Arrange
            TestContext context = new();
            TestState initialState = new("Initial") { NextState = new("Next") };
            var stateMachine = StateMachine<string>.Create(context, initialState);

            // Act - Remained
            var resultRemained = stateMachine.ProcessEvent("Stay");

            // Assert
            Assert.Equal(ProcessingResult.Remained, resultRemained);

            // Act - Transitioned
            var resultTransitioned = stateMachine.ProcessEvent("Change");

            // Assert
            Assert.Equal(ProcessingResult.Transitioned, resultTransitioned);
        }

        [Fact]
        public void ProcessEvent_WhenLocked_ShouldReturnNotProcessed()
        {
            // Arrange
            TestContext context = new();
            ReentrantTestState initialState = new("Initial");
            var stateMachine = StateMachine<string>.Create(context, initialState);

            // Set the state machine reference in the state
            initialState.StateMachine = stateMachine;

            // Act - This will attempt to call ProcessEvent recursively
            var result = stateMachine.ProcessEvent("ReentrantProcess");

            // Assert
            Assert.Equal(ProcessingResult.Transitioned, result); // First call succeeds
            Assert.Equal(
                ProcessingResult.NotProcessed, initialState.ReentrantProcessResult); // Second call fails (reentrant)
        }

        [Fact]
        public void StateMachine_ShouldTransitionCorrectly()
        {
            // Arrange
            TestContext context = new();
            TestState nextState = new("Next");
            TestState initialState = new("Initial") { NextState = nextState };

            var stateMachine = StateMachine<string>.Create(context, initialState);

            // Act
            stateMachine.ProcessEvent("Change");

            // Assert
            Assert.Same(nextState, stateMachine.CurrentState);
            Assert.Equal(1, initialState.OnExitingCallCount);
            Assert.Equal(1, initialState.OnExitedCallCount);
            Assert.Equal(1, nextState.OnEnteringCallCount);
            Assert.Equal(1, nextState.OnEnteredCallCount);
        }

        [Fact]
        public void StateMachine_WithValueTypes_ShouldWorkCorrectly()
        {
            // Arrange - Create a state machine with value types
            var stateMachine = StateMachine<char>.Create(42, new ValueState('A'));

            // Act - Process an event
            var result = stateMachine.ProcessEvent('B');

            // Assert
            Assert.Equal(ProcessingResult.Transitioned, result);
            Assert.Equal('B', stateMachine.CurrentState.Kind);
            Assert.Equal(42, stateMachine.Context);
        }

        private sealed class TestContext
        {
            // ReSharper disable once CollectionNeverQueried.Local
            public List<string> Log { get; } = [];
        }

        private sealed class TestState : IState<TestContext, string, TestState>
        {
            public TestState(string name) => Name = name;

            private string Name { get; }
            public TestState? NextState { get; init; }

            public int OnRemainCallCount { get; private set; }
            public int OnExitingCallCount { get; private set; }
            public int OnExitedCallCount { get; private set; }
            public int OnEnteringCallCount { get; private set; }
            public int OnEnteredCallCount { get; private set; }

            public bool TryCreateNewState(TestContext context, string ev, [MaybeNullWhen(false)] out TestState newState)
            {
                if (NextState != null && ev is "Change")
                {
                    newState = NextState;
                    return true;
                }

                newState = null;
                return false;
            }

            public void OnExiting(TestContext context, string ev, TestState newState)
            {
                OnExitingCallCount++;
                context.Log.Add($"{Name}.OnExiting(ev: {ev}, newState: {newState.Name})");
            }

            public void OnExited(TestContext context, string ev, TestState newState)
            {
                OnExitedCallCount++;
                context.Log.Add($"{Name}.OnExited(ev: {ev}, newState: {newState.Name})");
            }

            public void OnRemain(TestContext context, string ev)
            {
                OnRemainCallCount++;
                context.Log.Add($"{Name}.OnRemain(ev: {ev})");
            }

            public void OnEntering(TestContext context, string ev, TestState oldState)
            {
                OnEnteringCallCount++;
                context.Log.Add($"{Name}.OnEntering(ev: {ev}, oldState: {oldState.Name})");
            }

            public void OnEntered(TestContext context, string ev, TestState oldState)
            {
                OnEnteredCallCount++;
                context.Log.Add($"{Name}.OnEntered(ev: {ev}, oldState: {oldState.Name})");
            }
        }

        private sealed class ReentrantTestState : IState<TestContext, string, ReentrantTestState>
        {
            public ReentrantTestState(string name) => Name = name;

            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            internal string Name { get; }
            public StateMachine<TestContext, string, ReentrantTestState>? StateMachine { get; set; }
            public bool ReentrantCallResult { get; private set; }
            public ProcessingResult ReentrantProcessResult { get; private set; }

            public bool TryCreateNewState(
                TestContext context, string ev, [MaybeNullWhen(false)] out ReentrantTestState newState)
            {
                if (ev is "Reentrant")
                {
                    // Try reentrant call
                    ReentrantCallResult = StateMachine?.TryProcessEvent("AnotherEvent") ?? false;
                    newState = null;
                    return false;
                }

                if (ev is "ReentrantProcess")
                {
                    // Create a new state for the transition
                    ReentrantTestState nextState = new("Next") { StateMachine = StateMachine };
                    newState = nextState;
                    return true;
                }

                newState = null;
                return false;
            }

            public void OnExiting(TestContext context, string ev, ReentrantTestState newState) { }
            public void OnExited(TestContext context, string ev, ReentrantTestState newState) { }
            public void OnRemain(TestContext context, string ev) { }
            public void OnEntering(TestContext context, string ev, ReentrantTestState oldState) { }

            public void OnEntered(TestContext context, string ev, ReentrantTestState oldState)
            {
                if (ev is "ReentrantProcess")
                {
                    // Try reentrant call
                    ReentrantProcessResult = StateMachine?.ProcessEvent("FinalEvent") ?? ProcessingResult.NotProcessed;
                }
            }
        }

        private readonly struct ValueState : IState<int, char, ValueState>
        {
            public ValueState(char kind)
            {
                Kind = kind;
                TransitionCount = 0;
                RemainCount = 0;
            }

            private ValueState(char kind, int transitionCount, int remainCount)
            {
                Kind = kind;
                TransitionCount = transitionCount;
                RemainCount = remainCount;
            }

            public char Kind { get; }
            private int TransitionCount { get; }
            private int RemainCount { get; }

            public bool TryCreateNewState(int context, char ev, out ValueState newState)
            {
                if (ev != Kind) // Transition to a state matching the event
                {
                    newState = new(ev, TransitionCount + 1, RemainCount);
                    return true;
                }

                newState = default;
                return false;
            }

            public void OnExiting(int context, char ev, ValueState newState) { }

            public void OnExited(int context, char ev, ValueState newState) { }

            public void OnRemain(int context, char ev) { }

            public void OnEntering(int context, char ev, ValueState oldState) { }

            public void OnEntered(int context, char ev, ValueState oldState) { }
        }
    }
}
