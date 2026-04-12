using System.Windows.Input;

namespace InventorySystem_MAUI.View.Components;

public partial class AddButton : ContentView
{
	public static readonly BindableProperty AddCommandProperty = BindableProperty.Create(nameof(AddCommand), typeof(ICommand), typeof(AddButton));

	public ICommand AddCommand
    {
        get => (ICommand)GetValue(AddCommandProperty);
        set => SetValue(AddCommandProperty, value);
    }

    public AddButton() => InitializeComponent();
	
}