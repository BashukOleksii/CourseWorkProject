using System.Windows.Input;

namespace InventorySystem_MAUI.View.Components;

public partial class PaginationControl : ContentView
{
    public static readonly BindableProperty PageNumberProperty = BindableProperty.Create(nameof(PageNumber), typeof(int), typeof(PaginationControl));
    public static readonly BindableProperty CanNextProperty = BindableProperty.Create(nameof(CanNext), typeof(bool), typeof(PaginationControl));
    public static readonly BindableProperty PrevCommandProperty = BindableProperty.Create(nameof(PrevCommand), typeof(ICommand), typeof(PaginationControl));
    public static readonly BindableProperty NextCommandProperty = BindableProperty.Create(nameof(NextCommand), typeof(ICommand), typeof(PaginationControl));

    public int PageNumber { get => (int)GetValue(PageNumberProperty); set => SetValue(PageNumberProperty, value); }
    public bool CanNext { get => (bool)GetValue(CanNextProperty); set => SetValue(CanNextProperty, value); }
    public ICommand PrevCommand { get => (ICommand)GetValue(PrevCommandProperty); set => SetValue(PrevCommandProperty, value); }
    public ICommand NextCommand { get => (ICommand)GetValue(NextCommandProperty); set => SetValue(NextCommandProperty, value); }

    public PaginationControl() => InitializeComponent();
}