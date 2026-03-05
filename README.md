# uapp-toolkit-internet-checker

A Unity package for checking internet connectivity and managing actions that depend on an active internet connection.

## Description

**InternetCheckerService** is a service that:
- Checks internet connection availability
- Maintains a queue of actions to be executed only when internet is available
- Displays a UI message when internet is unavailable
- Allows users to skip waiting via a Skip button

## Usage

### 1. Initialization

```csharp
InternetCheckerService checkerService = GetComponent<InternetCheckerService>();
checkerService.Init();
```

### 2. Execute Action with Internet Check

```csharp
// Execute action when internet becomes available
checkerService.ExecuteWithInternet(() => {
    // Your code here
    Debug.Log("Internet is available!");
});

// With skip option (button appears after 3 seconds of waiting)
checkerService.ExecuteWithInternet(() => {
    SomeImportantNetworkAction();
}, availableSkip: true);
```

### 3. Track Connection Status Changes

```csharp
checkerService.OnChangedState.AddListener(() => {
    Debug.Log("Internet status changed: " + (checkerService.Online ? "Online" : "Offline"));
});
```

## Requirements

- Unity (with UI support)
- UnityEngine.Networking

## Properties

- **Online** (read-only) — current internet connection status
- **NoInternetMessage** — GameObject displaying the no-internet message
- **SkipButton** — button to skip waiting
- **OnChangedState** — UnityEvent triggered when connection status changes
