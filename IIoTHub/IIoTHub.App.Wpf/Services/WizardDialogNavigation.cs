using IIoTHub.App.Wpf.ViewModels;

namespace IIoTHub.App.Wpf.Services
{
    public class WizardDialogNavigation
    {
        private int _currentPageIndex;
        private readonly List<Func<ViewModelBase>> _pageViewModelFactories;

        public WizardDialogNavigation(List<Func<ViewModelBase>> pageViewModelFactories)
        {
            _pageViewModelFactories = pageViewModelFactories;
        }

        /// <summary>
        /// 目前的ViewModel
        /// </summary>
        public object CurrentPageViewModel => _pageViewModelFactories[_currentPageIndex]();

        /// <summary>
        /// 是否可以往下一頁
        /// </summary>
        public bool CanMoveNext => _currentPageIndex < _pageViewModelFactories.Count - 1;

        /// <summary>
        /// 是否可以往上一頁
        /// </summary>
        public bool CanMovePrevious => _currentPageIndex > 0;

        /// <summary>
        /// 移動到下一頁
        /// </summary>
        public void MoveNext()
        {
            if (CanMoveNext)
            {
                _currentPageIndex++;
            }
        }

        /// <summary>
        /// 移動到上一頁
        /// </summary>
        public void MovePrevious()
        {
            if (CanMovePrevious)
            {
                _currentPageIndex--;
            }
        }
    }
}
