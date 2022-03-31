using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using StudentList.ViewModels;

namespace StudentList.Views;

public partial class FileView : UserControl
{
    public FileView()
    {
        InitializeComponent();
        this.FindControl<Button>("Save").Click += async delegate
        {
            var taskPath = new SaveFileDialog().ShowAsync((Window) Parent);

            var path = await taskPath;
            var context = Parent.DataContext as MainWindowViewModel;
            if (path is not null) context.WriteToXMLFile(path);
            context.OpenMainView();
        };
        this.FindControl<Button>("Load").Click += async delegate
        {
            var taskPath = new OpenFileDialog().ShowAsync((Window) Parent);
            var path = await taskPath;
            var context = Parent.DataContext as MainWindowViewModel;
            if (path is not null) context.ReadFromXMLFile(string.Join("/", path));
            context.OpenMainView();
        };
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}