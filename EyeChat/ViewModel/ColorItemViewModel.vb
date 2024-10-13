Imports System.Collections.ObjectModel

Public Class ColorItemViewModel
    Public Property Color As Color
    Public Property Name As String

    Private ReadOnly ColorItems As ObservableCollection(Of ColorItemViewModel)

    Public Sub New(color As Color, name As String, colorItems As ObservableCollection(Of ColorItemViewModel))
        Me.Color = color
        Me.Name = name
        Me.ColorItems = colorItems
    End Sub

    Public Property ColorItem As ColorItemViewModel
        Get
            ' Convertir la valeur de AppColorString en ColorItemViewModel
            Return ColorItems.FirstOrDefault(Function(c) c.Name = MainWindow._userSettingsMain.AppColorString)
        End Get
        Set(ByVal value As ColorItemViewModel)
            MainWindow._userSettingsMain.AppColorString = value?.Name ' Assurez-vous que value n'est pas null avant d'accéder à sa propriété Name
            MainWindow._userSettingsMain.Save()
            NotifyPropertyChanged("AppColorString")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(v As String)
        Throw New NotImplementedException()
    End Sub
End Class