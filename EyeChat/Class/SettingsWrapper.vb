Imports System.ComponentModel

Public Class SettingsWrapper
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged

    Public Property UserName As String
        Get
            Return My.Settings.UserName
        End Get
        Set(ByVal value As String)
            My.Settings.UserName = value
            My.Settings.Save()
            NotifyPropertyChanged("UserName")
        End Set
    End Property

    Public Property AppTheme As String
        Get
            Return My.Settings.AppTheme
        End Get
        Set(ByVal value As String)
            My.Settings.AppTheme = value
            My.Settings.Save()
            NotifyPropertyChanged("AppTheme")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class
