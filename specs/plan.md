# State Machine Refactoring Plan

## Goals

- Improve the state machine by providing more detailed information about event processing outcomes
- Allow distinguishing between "not processed", "processed but remained in same state", and "processed with state transition"

## Tasks

- [x] Create an enum to represent the event processing result
  - Suggestions: `ProcessingResult` with values like:
    - `NotProcessed` - Event couldn't be processed (lock couldn't be acquired)
    - `Remained` - Event was processed but state didn't change
    - `Transitioned` - Event was processed and state changed

- [x] Modify `ProcessEventUnchecked` to:
  - Rename to `TryProcessEventUnchecked` for consistency
  - Return a boolean indicating whether a state transition occurred

- [x] Add a new public `ProcessEvent()` method that:
  - Tries to acquire the lock like `TryProcessEvent()`
  - Returns the new enum to indicate the detailed result
  - Calls the renamed `TryProcessEventUnchecked` method
  
- [x] Keep the existing `TryProcessEvent()` method intact
  - Update its implementation to use `TryProcessEventUnchecked` if needed

- [ ] Update tests and documentation to reflect the new functionality

- [ ] Consider adding XML comments to clearly explain the return values 
