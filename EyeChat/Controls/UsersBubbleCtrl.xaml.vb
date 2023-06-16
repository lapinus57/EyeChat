Imports System.ComponentModel
Imports EyeChat.MainWindow
Imports Newtonsoft.Json

Public Class UsersBubbleCtrl
    Inherits UserControl
    Implements INotifyPropertyChanged

    Public Shared ReadOnly SizeProperty As DependencyProperty = DependencyProperty.Register("SizeDisplayUsers", GetType(Double), GetType(UsersBubbleCtrl))
    Public Shared ReadOnly NameProperty As DependencyProperty = DependencyProperty.Register("Name", GetType(String), GetType(UsersBubbleCtrl))

    Public Property Name As String
        Get
            Return CStr(GetValue(NameProperty))
        End Get
        Set(value As String)
            SetValue(NameProperty, value)
        End Set
    End Property
    Public Property Size As Double
        Get
            Return CDbl(GetValue(SizeProperty))
        End Get
        Set(value As Double)
            SetValue(SizeProperty, value)
        End Set
    End Property

    Private _RoomNameDisplayUsers As Visibility
    Public Property RoomNameDisplayUsers As Visibility
        Get
            Return _RoomNameDisplayUsers
        End Get
        Set(value As Visibility)
            _RoomNameDisplayUsers = value
            OnPropertyChanged("RoomNameDisplayUsers")
        End Set
    End Property

    Private _NameRoomDisplayUsers As Visibility
    Public Property NameRoomDisplayUsers As Visibility
        Get
            Return _NameRoomDisplayUsers
        End Get
        Set(value As Visibility)
            _NameRoomDisplayUsers = value
            OnPropertyChanged("NameRoomDisplayUsers")
        End Set
    End Property

    Private _NameDisplayUsers As Visibility
    Public Property NameDisplayUsers As Visibility
        Get
            Return _NameDisplayUsers
        End Get
        Set(value As Visibility)
            _NameDisplayUsers = value
            OnPropertyChanged("NameDisplayUsers")
        End Set
    End Property

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().
        RoomNameDisplayUsers = My.Settings.RoomNameDisplayUsers
        NameRoomDisplayUsers = My.Settings.NameRoomDisplayUsers
        NameDisplayUsers = My.Settings.NameDisplayUsers
        Size = My.Settings.AppSizeDisplay
    End Sub

    Protected Sub OnPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub Border_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
        Dim selectedUser As User = DirectCast(DataContext, User)
        Dim selectedName As String = selectedUser.Name
        SelectUser(selectedName)
        ' Faites quelque chose avec le Name sélectionné
        ' ...

    End Sub
End Class

