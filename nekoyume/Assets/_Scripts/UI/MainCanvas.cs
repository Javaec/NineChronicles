using System;
using System.Collections;
using System.Collections.Generic;
using Nekoyume.EnumType;
using Nekoyume.Pattern;
using Nekoyume.UI.Module;
using UnityEngine;

namespace Nekoyume.UI
{
    [RequireComponent(typeof(Canvas))]
    public class MainCanvas : MonoSingleton<MainCanvas>
    {
        public GameObject hud;
        public GameObject popup;
        public GameObject screen;
        public GameObject tooltip;
        public GameObject widget;
        public GameObject systemInfo;
        public GameObject development;

        private List<Widget> _firstWidgets;
        private List<Widget> _secondWidgets;

        public Canvas Canvas { get; private set; }
        public RectTransform RectTransform { get; private set; }

        public Transform GetTransform(WidgetType widgetType)
        {
            switch (widgetType)
            {
                case WidgetType.Hud:
                    return hud.transform;
                case WidgetType.Popup:
                    return popup.transform;
                case WidgetType.Screen:
                    return screen.transform;
                case WidgetType.Tooltip:
                    return tooltip.transform;
                case WidgetType.Widget:
                    return widget.transform;
                case WidgetType.SystemInfo:
                    return systemInfo.transform;
                case WidgetType.Development:
                    return development.transform;
                default:
                    throw new ArgumentOutOfRangeException(nameof(widgetType), widgetType, null);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            Canvas = GetComponent<Canvas>();
            RectTransform = GetComponent<RectTransform>();
        }

        public void InitializeFirst()
        {
            _firstWidgets = new List<Widget>
            {
                // 스크린 영역. 로딩창류.
                Widget.Create<GrayLoadingScreen>(),
                Widget.Create<StageLoadingScreen>(),
                Widget.Create<LoadingScreen>(),
                Widget.Create<PreloadingScreen>(true),
                Widget.Create<Title>(true),
                Widget.Create<ModuleBlur>(),

                // 알림 영역.
                Widget.Create<UpdatePopup>(),
                Widget.Create<BlockFailPopup>(),
                Widget.Create<ActionFailPopup>(),
                Widget.Create<LoginPopup>(),
                Widget.Create<SystemPopup>(),
                
                // 시스템 정보 영역.
                Widget.Create<BlockChainMessageBoard>(true),
                Widget.Create<Notification>(true),

                //개발용 최상단 영역.
#if DEBUG
                Widget.Create<Cheat>(true),
#endif
            };

            foreach (var value in _firstWidgets)
            {
                value.Initialize();
            }

            Notification.RegisterWidgetTypeForUX<Mail>();
        }

        public IEnumerator InitializeSecond()
        {
            _secondWidgets = new List<Widget>();
            
            // 툴팁류.
            _secondWidgets.Add(Widget.Create<ItemInformationTooltip>());
            yield return null;
            
            // 일반.
            _secondWidgets.Add(Widget.Create<Synopsis>());
            yield return null;
            _secondWidgets.Add(Widget.Create<Login>());
            yield return null;
            _secondWidgets.Add(Widget.Create<LoginDetail>());
            yield return null;
            _secondWidgets.Add(Widget.Create<Menu>());
            yield return null;
            _secondWidgets.Add(Widget.Create<Status>());
            yield return null;
            _secondWidgets.Add(Widget.Create<Blind>());
            yield return null;
            _secondWidgets.Add(Widget.Create<Shop>());
            yield return null;
            _secondWidgets.Add(Widget.Create<QuestPreparation>());
            yield return null;
            _secondWidgets.Add(Widget.Create<WorldMap>());
            yield return null;
            _secondWidgets.Add(Widget.Create<Combination>());
            yield return null;
            _secondWidgets.Add(Widget.Create<RankingBoard>());
            yield return null;
            _secondWidgets.Add(Widget.Create<Battle>());
            yield return null;

            // 모듈류.
            _secondWidgets.Add(Widget.Create<StatusDetail>());
            yield return null;
            _secondWidgets.Add(Widget.Create<Inventory>());
            yield return null;
            _secondWidgets.Add(Widget.Create<Mail>());
            yield return null;
            _secondWidgets.Add(Widget.Create<Quest>());
            yield return null;
            _secondWidgets.Add(Widget.Create<BottomMenu>());
            yield return null;
            _secondWidgets.Add(Widget.Create<Dialog>());
            yield return null;

            // 팝업류.
            //_secondWidgets.Add(Widget.Create<PopupBlur>());
            //yield return null;
            _secondWidgets.Add(Widget.Create<BattleResult>());
            yield return null;
            _secondWidgets.Add(Widget.Create<SimpleItemCountPopup>());
            yield return null;
            _secondWidgets.Add(Widget.Create<ItemCountAndPricePopup>());
            yield return null;
            _secondWidgets.Add(Widget.Create<CombinationResultPopup>());
            yield return null;
            _secondWidgets.Add(Widget.Create<StageTitle>());
            yield return null;
            _secondWidgets.Add(Widget.Create<Alert>());
            yield return null;
            _secondWidgets.Add(Widget.Create<Confirm>());
            yield return null;
            _secondWidgets.Add(Widget.Create<InputBox>());
            yield return null;

            Widget last = null;
            foreach (var value in _secondWidgets)
            {
                if (value is null)
                {
                    Debug.LogWarning($"value is null. last is {last.name}");
                    continue;
                }
                
                value.Initialize();
                last = value;
            }

            Notification.RegisterWidgetTypeForUX<Mail>();
        }
    }
}
