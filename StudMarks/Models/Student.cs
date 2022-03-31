using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using Avalonia.Media;

namespace StudentList.Models;

[Serializable]
public class Student : INotifyPropertyChanged
{
    private float? average;

    [XmlIgnore] private SolidColorBrush averageBrush;

    private ObservableCollection<StudentMark> controlMarks;


    public Student(string name)
    {
        Name = name;
        ControlMarks = new ObservableCollection<StudentMark>();
        ControlMarks.CollectionChanged += MyItemsSource_CollectionChanged;
        ControlMarks.Clear();
        ControlMarks.Add(new StudentMark(0));
        ControlMarks.Add(new StudentMark(0));
        ControlMarks.Add(new StudentMark(0));
        isChecked = false;
        CalculateAverage();
    }

    public Student()
    {
        Name = "NULL";
        ControlMarks = new ObservableCollection<StudentMark>();
        ControlMarks.CollectionChanged += MyItemsSource_CollectionChanged;
        ControlMarks.Clear();
        ControlMarks.Add(new StudentMark(0));
        ControlMarks.Add(new StudentMark(0));
        ControlMarks.Add(new StudentMark(0));
        isChecked = false;
        CalculateAverage();
    }

    public string Name { set; get; }

    public ObservableCollection<StudentMark> ControlMarks
    {
        get => controlMarks;
        set
        {
            controlMarks = value;
            RaisePropertyChangedEvent("ControlMarks");
        }
    }

    [XmlIgnore]
    public SolidColorBrush AverageBrush
    {
        get => averageBrush;
        private set
        {
            averageBrush = value;
            RaisePropertyChangedEvent("AverageBrush");
        }
    }

    [XmlIgnore] public bool isChecked { get; set; }

    [XmlIgnore]
    public float? Average
    {
        get => average;
        private set
        {
            if (SetBrushColor(value, AverageBrush))
                average = value;

            RaisePropertyChangedEvent("Average");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void CalculateAverage()
    {
        if (ControlMarks.Any(mark => mark.Mark is null))
        {
            Average = null;
        }
        else
        {
            float sum = 0;
            foreach (var mark in ControlMarks) sum += (float) mark.Mark;
            Average = sum / 3;
        }
    }

    public static bool SetBrushColor(float? mark, IBrush? brush = null)
    {
        if (mark is not null)
        {
            if (mark < 1.5) brush ??= new SolidColorBrush(Brushes.Yellow.Color);
            if (mark < 1) brush ??= new SolidColorBrush(Brushes.Red.Color);
            if (mark >= 1.5) brush ??= new SolidColorBrush(Brushes.LightGreen.Color);
        }
        else
        {
            brush ??= new SolidColorBrush(Brushes.White.Color);
            return false;
        }

        return true;
    }

    private void MyItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
            foreach (StudentMark item in e.NewItems)
                item.PropertyChanged += MyType_PropertyChanged;

        if (e.OldItems != null)
            foreach (StudentMark item in e.OldItems)
                item.PropertyChanged -= MyType_PropertyChanged;
    }

    private void MyType_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        CalculateAverage();
    }

    protected void RaisePropertyChangedEvent(string propertyName)
    {
        if (PropertyChanged != null)
        {
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, e);
        }
    }
}