using BrilliantSee.Models.Enums;
using BrilliantSee.ViewModels;
using CommunityToolkit.Maui.Core.Platform;

namespace BrilliantSee.Views;

public partial class SearchPage : ContentPage
{
    private readonly SearchViewModel _vm;

    /// <summary>
    /// ��ť�ı���Ӧ�����
    /// </summary>
    private Dictionary<string, SourceCategory> Categories;

    private Button[] Buttons;

    private int CurrentButtonIndex = 0;
    private SwipeDirection _direction { get; set; }
    private double _offset { get; set; } = 0;

    public SearchPage(SearchViewModel vm)
    {
        _vm = vm;
        this.BindingContext = _vm;
        InitializeComponent();
        Categories = new Dictionary<string, SourceCategory>()
        {
            { "ȫ��", SourceCategory.All },
            { "С˵", SourceCategory.Novel },
            { "����", SourceCategory.Comic },
            { "����", SourceCategory.Video }
        };
        Buttons = new Button[] { all, novels, comics, videos };
        this.Loaded += SearchPage_Loaded;
    }

    /// <summary>
    /// ҳ�����ʱ������ȡ����
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void SearchPage_Loaded(object? sender, EventArgs e)
    {
        this.input.Focus();
        await Task.Delay(250);
        this.input.Focus();
        //this.audio.IsVisible = await _vm._db.GetAudioStatus();
    }

    /// <summary>
    /// ���������¼���ʵ�ַ��ض�����ť����ʾ�������Լ����״������ظ���
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        this.floatButton.IsVisible = e.FirstVisibleItemIndex == 0 ? false : true;
        if (e.LastVisibleItemIndex == _vm.CurrentObjsCount - 1 && _vm.IsGettingResult == false && _vm.CurrentObjsCount != 0)
        {
            await _vm.GetMoreAsync();
        }
    }

    /// <summary>
    /// ���ض���
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BacktoTop(object sender, EventArgs e)
    {
        this.comicList.ScrollTo(0, position: ScrollToPosition.Start);
    }

    /// <summary>
    /// ��ť���Ч��
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="type"></param>
    private async Task ButtonTapped(object sender, Type type)
    {
        View obj = (View)sender;
        await obj!.ScaleTo(1.15, 100);
        await obj!.ScaleTo(1, 100);
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        _ = ButtonTapped(sender, sender.GetType());
    }

    private void floatButton_Pressed(object sender, EventArgs e)
    {
        _ = ButtonTapped(sender, sender.GetType());
    }

    /// <summary>
    /// �л���𣬸���UI,ˢ���б�
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Clicked(object sender, EventArgs e)
    {
        var button = sender! as Button;
        var selectedCategory = Categories[button!.Text];

        if (selectedCategory == _vm.CurrentCategory) return;
        _vm.ChangeCurrentCategory(selectedCategory);
        _ = ButtonTapped(sender, typeof(Button));
        CurrentButtonIndex = Array.IndexOf(Buttons, button);

        this.comicList.ItemsSource = _vm.GetObjsOnDisplay();
    }

    private void SwipeView_SwipeChanging(object sender, SwipeChangingEventArgs e)
    {
        _direction = e.SwipeDirection;
        _offset = e.Offset;
    }

    private void SwipeView_SwipeEnded(object sender, SwipeEndedEventArgs e)
    {
        var value = _direction == SwipeDirection.Left ? 1 : -1;
        if (Math.Abs(_offset) > 24)
        {
            swipeView.Close();
            var index = CurrentButtonIndex + value;
            if (index < 0 || index > 3)
            {
                return;
            }
            CurrentButtonIndex = index;
            Button_Clicked(Buttons[CurrentButtonIndex], e);
        }
    }

    /// <summary>
    /// ���������¼
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void CleanRecord(object sender, EventArgs e)
    {
        //��ʾ�Ƿ����
        bool answer = await DisplayAlert("���������¼", "�Ƿ�ɾ��ȫ��������ʷ?", "ȫ��ɾ��", "ȡ��");
        if (answer)
        {
            _vm.IsShowRecord = false;
            await _vm.ClearSearchRecordAsync();
        }
    }

    /// <summary>
    /// �������
    /// </summary>
    private void HideKeyboard()
    {
#if ANDROID
        if (input.IsSoftKeyboardShowing())
        {
            _ = input.HideKeyboardAsync(CancellationToken.None);
        }
#endif
    }

    private void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        HideKeyboard();
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        Button button = (Button)sender!;
        input.Text = button.Text;
        input.Unfocus();
        HideKeyboard();
    }

    private void input_Focused(object sender, FocusEventArgs e)
    {
        _vm.IsShowSearchResult = false;
        _vm.IsShowSearchMessage = true;
    }

    private void input_Completed(object sender, EventArgs e)
    {
        input.Unfocus();
    }

    protected override bool OnBackButtonPressed()
    {
        if (_vm.IsShowSearchResult)
        {
            input.Text = "";
            _vm.IsShowSearchResult = false;
            _vm.IsShowSearchMessage = true;
            return true;
        }
        return base.OnBackButtonPressed();
    }
}