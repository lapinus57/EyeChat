Imports System.ComponentModel
Imports EyeChat.MainWindow
Imports EyeChat.User

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
        Size = My.Settings.AppSizeDisplay
    End Sub

#Disable Warning BC40005 ' Le membre masque une méthode substituable dans le type de base
    Protected Sub OnPropertyChanged(propertyName As String)
#Enable Warning BC40005 ' Le membre masque une méthode substituable dans le type de base
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub Border_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
        Dim selectedUser As User = DirectCast(DataContext, User)
        Dim selectedName As String = selectedUser.Name
        SelectUser(selectedName)

    End Sub

    Private Sub MenuItemMoveUp_Click(sender As Object, e As RoutedEventArgs)
        ' Trouver l'élément parent ListBoxItem du UserControl
        Dim listBoxItem As ListBoxItem = FindParentListBoxItem(Me)

        ' Récupérer l'utilisateur correspondant à l'élément de la liste
        Dim user As User = TryCast(listBoxItem.DataContext, User)

        If user IsNot Nothing Then
            ' Obtenir l'index actuel de l'utilisateur dans la liste
            Dim currentIndex As Integer = Users.IndexOf(user)

            ' Vérifier si l'utilisateur n'est pas déjà en haut de la liste et n'est pas "A Tous"
            If currentIndex > 2 Then
                ' Utiliser la méthode Move de la ObservableCollection pour déplacer l'utilisateur vers le haut
                Users.Move(currentIndex, currentIndex - 1)
            End If
        End If
        SaveUsersToJson(Users)
    End Sub

    Private Sub MenuItemMoveDown_Click(sender As Object, e As RoutedEventArgs)
        ' Trouver l'élément parent ListBoxItem du UserControl
        Dim listBoxItem As ListBoxItem = FindParentListBoxItem(Me)

        ' Récupérer l'utilisateur correspondant à l'élément de la liste
        Dim user As User = TryCast(listBoxItem.DataContext, User)

        If user IsNot Nothing Then
            ' Obtenir l'index actuel de l'utilisateur dans la liste
            Dim currentIndex As Integer = Users.IndexOf(user)

            ' Vérifier si l'utilisateur n'est pas déjà en bas de la liste et n'est pas "Secrétariat"
            If currentIndex < Users.Count - 1 AndAlso Not user.Name.Equals("Secrétariat") AndAlso Not user.Name.Equals("A Tous") Then
                ' Utiliser la méthode Move de la ObservableCollection pour déplacer l'utilisateur vers le bas
                Users.Move(currentIndex, currentIndex + 1)
            End If
        End If
        SaveUsersToJson(Users)
    End Sub

    Private Function FindParentListBoxItem(element As DependencyObject) As ListBoxItem
        ' Parcourir la hiérarchie des éléments parents jusqu'à ce qu'un ListBoxItem soit trouvé
        Dim parent = VisualTreeHelper.GetParent(element)

        While parent IsNot Nothing AndAlso Not TypeOf parent Is ListBoxItem
            parent = VisualTreeHelper.GetParent(parent)
        End While

        Return DirectCast(parent, ListBoxItem)
    End Function
End Class

