Imports System.Collections.ObjectModel
Imports System.ComponentModel

Public Class PatientListControl
    Implements INotifyPropertyChanged
    Public Property PatientList As New ObservableCollection(Of Patient)()
    Private _selectedPatient As Patient

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub OnPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub



    Public Property SelectedPatient As Patient
        Get
            Return _selectedPatient
        End Get
        Set(value As Patient)
            _selectedPatient = value
            OnPropertyChanged("selectedPatient")
        End Set
    End Property

    Public Sub New()
        InitializeComponent()
        PatientList.Add(New Patient() With {
            .Title = "Mr",
            .LastName = "Doe",
            .FirstName = "John",
            .Exams = "Exams",
            .Annotation = "Annotation",
            .Position = "Position",
            .Hold_Time = DateTime.Now,
            .Pick_up_Time = DateTime.Now,
            .Time_Order = TimeSpan.Zero
        })

        DataContext = Me
    End Sub

    Public Sub AddPatient(title As String, lastName As String, firstName As String, exams As String, annotation As String, position As String, holdTime As DateTime, pickUpTime As DateTime, timeOrder As TimeSpan, examinator As String, operatorName As String)
        Dim newPatient As New Patient() With {
        .Id = DateTime.Now.ToString("HHmmssfff"),
        .Title = title,
        .LastName = lastName,
        .FirstName = firstName,
        .Exams = exams,
        .Annotation = annotation,
        .Position = position,
        .Hold_Time = holdTime,
        .Pick_up_Time = pickUpTime,
        .Time_Order = timeOrder,
        .Examinator = examinator,
        .OperatorName = operatorName
    }
        PatientList.Add(newPatient)
    End Sub

    Private Sub MenuItem_Click(sender As Object, e As RoutedEventArgs)
        ' Logique à exécuter lorsque l'utilisateur clique sur un MenuItem du menu contextuel
        ' Vous pouvez accéder aux données du patient sélectionné en utilisant le modèle de liaison de données
        ' par exemple : Dim selectedPatient As Patient = DirectCast(DirectCast(sender, MenuItem).DataContext, Patient)
        ' selectedPatient contiendra alors les données du patient sélectionn
        Dim clickedPatient As Patient = SelectedPatient
    End Sub
End Class