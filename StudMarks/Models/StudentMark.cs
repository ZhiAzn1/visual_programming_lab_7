using System.ComponentModel;
using System.Xml.Serialization;
using Avalonia.Media;

namespace StudentList.Models;

public class StudentMark : INotifyPropertyChanged
{
    private float? mark;

    public StudentMark(float mark)
    {
        Mark = mark;
    }

    public StudentMark()
    {
        Mark = 0;
    }

    [XmlIgnore] public IBrush Brush { get; private set; }

    public float? Mark
    {
        set
        {
            switch (value)
            {
                case 0:
                    Brush = Brushes.Red;
                    mark = value;
                    break;
                case 1:
                    Brush = Brushes.Yellow;
                    mark = value;
                    break;
                case 2:
                    Brush = Brushes.LightGreen;
                    mark = value;
                    break;
                default:
                    Brush = Brushes.White;
                    mark = null;
                    break;
            }

            RaisePropertyChangedEvent("Mark");
        }
        get => mark;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void RaisePropertyChangedEvent(string propertyName)
    {
        if (PropertyChanged != null)
        {
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, e);
        }
    }
}