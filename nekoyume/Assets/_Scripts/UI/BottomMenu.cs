using System;
using System.Collections.Generic;
using System.Linq;
using Nekoyume.Game.Mail;
using Nekoyume.Model;
using UniRx;

namespace Nekoyume.UI.Module
{
    public class BottomMenu : Widget
    {
        public enum ToggleableType
        {
            Mail,
            Quest,
            Chat,
            IllustratedBook,
            Settings
        }

        public class Model
        {
            public readonly ReactiveProperty<UINavigator.NavigationType> NavigationType =
                new ReactiveProperty<UINavigator.NavigationType>(UINavigator.NavigationType.Back);

            public Action<BottomMenu> NavigationAction;

            public readonly ReactiveProperty<bool> HasNotificationInMail = new ReactiveProperty<bool>();
            public readonly ReactiveProperty<bool> HasNotificationInQuest = new ReactiveProperty<bool>();
            public readonly ReactiveProperty<bool> HasNotificationInChat = new ReactiveProperty<bool>();
            public readonly ReactiveProperty<bool> HasNotificationInIllustratedBook = new ReactiveProperty<bool>();
            public readonly ReactiveProperty<bool> HasNotificationInSettings = new ReactiveProperty<bool>();
        }

        // 네비게이션 버튼.
        public NormalButton backButton;
        public NormalButton mainButton;
        public NormalButton quitButton;

        // 토글 그룹과 버튼.
        private ToggleGroup _toggleGroup;
        public NotifiableButton mailButton;
        public NotifiableButton questButton;
        public NotifiableButton chatButton;
        public NotifiableButton illustratedBookButton;
        public NotifiableButton settingsButton;

        private readonly List<IDisposable> _disposablesAtOnEnable = new List<IDisposable>();

        public readonly Model SharedModel = new Model();

        #region Mono

        public override void Initialize()
        {
            base.Initialize();

            SharedModel.NavigationType.Subscribe(SubscribeNavigationType).AddTo(gameObject);
            SharedModel.HasNotificationInMail.SubscribeTo(mailButton.SharedModel.HasNotification).AddTo(gameObject);
            SharedModel.HasNotificationInQuest.SubscribeTo(questButton.SharedModel.HasNotification).AddTo(gameObject);
            SharedModel.HasNotificationInChat.SubscribeTo(chatButton.SharedModel.HasNotification).AddTo(gameObject);
            SharedModel.HasNotificationInIllustratedBook.SubscribeTo(illustratedBookButton.SharedModel.HasNotification)
                .AddTo(gameObject);
            SharedModel.HasNotificationInSettings.SubscribeTo(settingsButton.SharedModel.HasNotification)
                .AddTo(gameObject);

            backButton.button.OnClickAsObservable().Subscribe(SubscribeNavigationButtonClick).AddTo(gameObject);
            mainButton.button.OnClickAsObservable().Subscribe(SubscribeNavigationButtonClick).AddTo(gameObject);
            quitButton.button.OnClickAsObservable().Subscribe(SubscribeNavigationButtonClick).AddTo(gameObject);

            _toggleGroup = new ToggleGroup();
            _toggleGroup.OnToggledOn.Subscribe(SubscribeOnToggledOn).AddTo(gameObject);
            _toggleGroup.OnToggledOff.Subscribe(SubscribeOnToggledOff).AddTo(gameObject);
            _toggleGroup.RegisterToggleable(mailButton);
            _toggleGroup.RegisterToggleable(questButton);
            _toggleGroup.RegisterToggleable(chatButton);
            _toggleGroup.RegisterToggleable(illustratedBookButton);
            _toggleGroup.RegisterToggleable(settingsButton);
            mailButton.SetWidgetType<Mail>();
            questButton.SetWidgetType<Quest>();
            illustratedBookButton.button.OnClickAsObservable().Subscribe(SubscribeOnClick).AddTo(gameObject);
            chatButton.button.OnClickAsObservable().Subscribe(SubscribeOnClick).AddTo(gameObject);
            settingsButton.button.OnClickAsObservable().Subscribe(SubscribeOnClick).AddTo(gameObject);
        }

        private void SubscribeOnClick(Unit unit)
        {   
            Find<Alert>().Show("UI_ALERT_NOT_IMPLEMENTED_TITLE", "UI_ALERT_NOT_IMPLEMENTED_CONTENT");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _disposablesAtOnEnable.DisposeAllAndClear();
            ReactiveCurrentAvatarState.MailBox?.Subscribe(SubscribeAvatarMailBox).AddTo(_disposablesAtOnEnable);
        }

        protected override void OnDisable()
        {
            _toggleGroup?.SetToggledOffAll();
            _disposablesAtOnEnable.DisposeAllAndClear();
            base.OnDisable();
        }

        #endregion

        public void Show(UINavigator.NavigationType navigationType, Action<BottomMenu> navigationAction,
            bool useShowButtons = false, params ToggleableType[] showButtons)
        {
            base.Show();
            SharedModel.NavigationType.Value = navigationType;
            SharedModel.NavigationAction = navigationAction;

            if (!useShowButtons)
            {
                mailButton.Show();
                questButton.Show();
                chatButton.Show();
                illustratedBookButton.Show();
                settingsButton.Show();
                
                return;
            }
            
            mailButton.Hide();
            questButton.Hide();
            chatButton.Hide();
            illustratedBookButton.Hide();
            settingsButton.Hide();
            
            foreach (var toggleableType in showButtons)
            {
                switch (toggleableType)
                {
                    case ToggleableType.Mail:
                        mailButton.Show();
                        break;
                    case ToggleableType.Quest:
                        questButton.Show();
                        break;
                    case ToggleableType.Chat:
                        chatButton.Show();
                        break;
                    case ToggleableType.IllustratedBook:
                        illustratedBookButton.Show();
                        break;
                    case ToggleableType.Settings:
                        settingsButton.Show();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        // 이 위젯은 애니메이션 없이 바로 닫히는 것을 기본으로 함.
        public override void Close(bool ignoreCloseAnimation = false)
        {
            foreach (var toggleable in _toggleGroup.Toggleables)
            {
                if (!(toggleable is IWidgetControllable widgetControllable))
                    continue;
                
                widgetControllable.HideWidget();
            }
            
            base.Close(true);
        }

        #region Subscribe

        private void SubscribeNavigationType(UINavigator.NavigationType navigationType)
        {
            switch (navigationType)
            {
                case UINavigator.NavigationType.None:
                    backButton.Hide();
                    mainButton.Hide();
                    quitButton.Hide();
                    break;
                case UINavigator.NavigationType.Back:
                    backButton.Show();
                    mainButton.Hide();
                    quitButton.Hide();
                    break;
                case UINavigator.NavigationType.Main:
                    backButton.Hide();
                    mainButton.Show();
                    quitButton.Hide();
                    break;
                case UINavigator.NavigationType.Quit:
                    backButton.Hide();
                    mainButton.Hide();
                    quitButton.Show();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(navigationType), navigationType, null);
            }
        }

        private void SubscribeNavigationButtonClick(Unit unit)
        {
            SharedModel.NavigationAction?.Invoke(this);
        }

        private static void SubscribeOnToggledOn(IToggleable toggleable)
        {
            if (!(toggleable is IWidgetControllable widgetControllable))
                return;

            widgetControllable.ShowWidget();
        }

        private static void SubscribeOnToggledOff(IToggleable toggleable)
        {
            if (!(toggleable is IWidgetControllable widgetControllable))
                return;

            widgetControllable.HideWidget();
        }

        private void SubscribeAvatarMailBox(MailBox mailBox)
        {
            if (mailBox is null)
                return;

            mailButton.SharedModel.HasNotification.Value = mailBox.Any(i => i.New);
            Find<Mail>().UpdateList();
        }

        #endregion
    }
}
