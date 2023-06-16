Imports ControlzEx.Theming
Imports MahApps.Metro.Controls
Imports System.Drawing
Imports EyeChat
Imports EyeChat.SettingsViewModel
Imports EyeChat.MainWindow
Imports EyeChat.User

Public Class SettingsWindows

    Private _settings As SettingsViewModel



    Public ReadOnly Property Settings As SettingsViewModel
        Get
            If _settings Is Nothing Then
                _settings = New SettingsViewModel()
            End If
            Return _settings
        End Get
    End Property



    Private Sub AppColorChanged(sender As Object, e As SelectionChangedEventArgs)
        SetTheme()
    End Sub



    Private Sub AppThemeChanged(sender As Object, e As SelectionChangedEventArgs)

        SetTheme()

    End Sub



    Private Sub SettingsWindows_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        InitializeComponent()
        Dim settings As New SettingsViewModel()
        Me.DataContext = settings

    End Sub



    Private Sub ToggleSwitch_Toggled(sender As Object, e As RoutedEventArgs)

        My.Application.MainWindow.UpdateLayout()

    End Sub

    Private Sub cbodisplay_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbodisplay.SelectionChanged
        If cbodisplay.SelectedValue = "A" Then
            My.Settings.NameDisplayUsers = Visibility.Visible
            My.Settings.RoomNameDisplayUsers = Visibility.Collapsed
            My.Settings.NameRoomDisplayUsers = Visibility.Collapsed
        ElseIf cbodisplay.SelectedValue = "B" Then
            My.Settings.NameDisplayUsers = Visibility.Collapsed
            My.Settings.RoomNameDisplayUsers = Visibility.Visible
            My.Settings.NameRoomDisplayUsers = Visibility.Collapsed
        ElseIf cbodisplay.SelectedValue = "C" Then
            My.Settings.NameDisplayUsers = Visibility.Collapsed
            My.Settings.RoomNameDisplayUsers = Visibility.Collapsed
            My.Settings.NameRoomDisplayUsers = Visibility.Visible
        End If
        My.Settings.Save()
        Users.Clear()
        Dim loadedUsers = LoadUsersFromJson()
        For Each user In loadedUsers
            Users.Add(user)
        Next
    End Sub
End Class




