using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Avalonia.Media;
using ReactiveUI;
using StudentList.Models;

namespace StudentList.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase content;

    public MainWindowViewModel()
    {
        Items = new ObservableCollection<Student>();
        AverageGrades = new ObservableCollection<float?> {0, 0, 0};
        AverageGradesBrushes = new ObservableCollection<IBrush>
        {
            new SolidColorBrush(Brushes.White.Color), new SolidColorBrush(Brushes.White.Color),
            new SolidColorBrush(Brushes.White.Color)
        };
        Items.CollectionChanged += MyItemsSource_CollectionChanged;
        Content = new MainViewModel();
    }

    private ObservableCollection<Student> Items { get; set; }
    private ObservableCollection<float?> AverageGrades { get; }
    private ObservableCollection<IBrush> AverageGradesBrushes { get; }

    public ViewModelBase Content
    {
        get => content;
        private set => this.RaiseAndSetIfChanged(ref content, value);
    }

    public void AddNewStudent()
    {
        Items.Insert(0, new Student("Новый студент"));
    }

    public void RemoveCheckedStudents()
    {
        var neededStudents = Items.Where(x => !x.isChecked).ToList();
        Items.Clear();
        foreach (var neededStudent in neededStudents) Items.Add(neededStudent);
    }

    public void WriteToXMLFile(string filePath)
    {
        var xs = new XmlSerializer(typeof(ObservableCollection<Student>));

        using (var wr = new StreamWriter(filePath))
        {
            xs.Serialize(wr, Items);
        }
    }

    public void ReadFromXMLFile(string filePath)
    {
        var xs = new XmlSerializer(typeof(ObservableCollection<Student>));
        using (var sr = new StreamReader(filePath))
        {
            Items.Clear();
            try
            {
                Items = (ObservableCollection<Student>) xs.Deserialize(sr);
                foreach (var s in Items)
                {
                    var gradeList = new List<StudentMark> {s.ControlMarks[3], s.ControlMarks[4], s.ControlMarks[5]};
                    s.ControlMarks.Clear();

                    foreach (var mark in gradeList)
                        s.ControlMarks.Add(mark);

                    s.CalculateAverage();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }

    public void OpenFileView()
    {
        Content = new FileViewModel();
    }

    public void OpenMainView()
    {
        Content = new MainViewModel();
    }

    public void CalculateAveragesOfStudents()
    {
        for (var i = 0; i < 3; i++) AverageGrades[i] = 0;
        foreach (var s in Items)
            for (var i = 0; i < 3; i++)
                AverageGrades[i] += s.ControlMarks[i].Mark;
        for (var i = 0; i < 3; i++)
        {
            AverageGrades[i] /= Items.Count;
            Student.SetBrushColor(AverageGrades[i], AverageGradesBrushes[i]);
        }
    }

    private void MyItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
            foreach (Student item in e.NewItems)
                item.PropertyChanged += MyType_PropertyChanged;

        if (e.OldItems != null)
            foreach (Student item in e.OldItems)
                item.PropertyChanged -= MyType_PropertyChanged;
    }

    private void MyType_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        CalculateAveragesOfStudents();
    }
}