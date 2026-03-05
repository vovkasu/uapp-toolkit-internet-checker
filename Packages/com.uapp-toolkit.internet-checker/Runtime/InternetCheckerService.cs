using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UAppToolKit.InternetChecker
{
    public class InternetCheckerService : MonoBehaviour
    {
        public bool Online
        {
            get => _online;
            private set
            {
                if (_online != value)
                {
                    _online = value;
                    OnChangedState?.Invoke();
                    UpdateNoInternetMessageView();
                }
            }
        }

        public UnityEvent OnChangedState = new();
        public GameObject NoInternetMessage;
        public Button SkipButton;

        private bool _online;
        private List<Action> _actions = new();
        private Coroutine _waitInternetCoroutine;
        private Coroutine _delayShowSkipButtonCoroutine;

        public void Init()
        {
            _online = true;
            UpdateNoInternetMessageView();
            SkipButton.onClick.AddListener(SkipProcess);
        }

        public void ExecuteWithInternet(Action action, bool availableSkip = false)
        {
            _actions.Add(action);
            if (_waitInternetCoroutine == null)
            {
                _waitInternetCoroutine = StartCoroutine(WaitInternetAndExecute());
                SkipButton.gameObject.SetActive(false);
                if (availableSkip)
                {
                    _delayShowSkipButtonCoroutine = StartCoroutine(DelayShowSkipButton());
                }
            }
        }

        private IEnumerator DelayShowSkipButton()
        {
            yield return new WaitForSeconds(3f);
            SkipButton.gameObject.SetActive(true);
        }

        private IEnumerator WaitInternetAndExecute()
        {
            while (UnityEngine.Application.internetReachability == NetworkReachability.NotReachable)
            {
                Online = false;
                yield return new WaitForSeconds(0.25f);
            }

            do
            {
                using (var request = UnityWebRequest.Get("https://google.com/generate_204"))
                {
                    request.timeout = 2;
                    yield return request.SendWebRequest();

                    Online = request.result == UnityWebRequest.Result.Success;
                    if (!Online)
                    {
                        yield return new WaitForSeconds(0.25f);
                    }
                }
            } 
            while (!Online);

            StopCoroutines();
            Execute();
        }

        private void StopCoroutines()
        {
            StopCoroutine(_waitInternetCoroutine);
            _waitInternetCoroutine = null;
            if (_delayShowSkipButtonCoroutine != null)
            {
                StopCoroutine(_delayShowSkipButtonCoroutine);
                _delayShowSkipButtonCoroutine = null;
            }
        }

        private void Execute()
        {
            var actions = new List<Action>(_actions);
            _actions.Clear();
            foreach (var action in actions)
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Debug.LogException(e, this);
                }
            }
        }

        private void UpdateNoInternetMessageView()
        {
            NoInternetMessage.SetActive(!Online);
        }

        private void SkipProcess()
        {
            StopCoroutines();
            NoInternetMessage.SetActive(false);
        }
    }
}
