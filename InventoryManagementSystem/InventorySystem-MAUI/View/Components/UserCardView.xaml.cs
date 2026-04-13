using System.Windows.Input;

namespace InventorySystem_MAUI.View.Components;

public partial class UserCardView : ContentView
{
    public static readonly BindableProperty EditCommandProperty =
        BindableProperty.Create(nameof(EditCommand), typeof(ICommand), typeof(UserCardView));

    public static readonly BindableProperty DeleteCommandProperty =
        BindableProperty.Create(nameof(DeleteCommand), typeof(ICommand), typeof(UserCardView));

    public static readonly BindableProperty PhotoURIProperty =
       BindableProperty.Create(nameof(PhotoURI), typeof(string), typeof(UserCardView));

    public ICommand EditCommand
    {
        get => (ICommand)GetValue(EditCommandProperty);
        set => SetValue(EditCommandProperty, value);
    }

    public ICommand DeleteCommand
    {
        get => (ICommand)GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }

    public string PhotoURI
    {
        get => (string)GetValue(PhotoURIProperty);
        set => SetValue(PhotoURIProperty, value);
    }
    public UserCardView()
    {
        InitializeComponent();
    }
}