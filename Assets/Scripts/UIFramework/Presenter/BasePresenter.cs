using System;
using UIFramework.View;
using UnityEngine;

namespace UIFramework.Presenter
{
    public class BasePresenter<TView> : MonoBehaviour , IPresenter where TView : IView
    {
        private TView _view;

        public TView View
        {
            get
            {
                if (_view != null)
                {
                    return _view;
                }

                if (TryGetComponent(out TView view))
                {
                    _view = view;
                    return _view;
                }

                throw new Exception($"{this.gameObject}对象上不存在View组件,获取View失败");
            }
        }
    }
}