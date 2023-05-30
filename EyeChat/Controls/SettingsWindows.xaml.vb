Imports ControlzEx.Theming
Imports MahApps.Metro.Controls
Imports System.Drawing
Imports EyeChat.SettingsViewModel

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

    Private Sub AppThemeChanged(sender As Object, e As SelectionChangedEventArgs)

        SetTheme()

    End Sub
    Private Sub AppColorChanged(sender As Object, e As SelectionChangedEventArgs)

        SetTheme()

    End Sub



    Private Sub SettingsWindows_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        InitializeComponent()
        Dim settings As New SettingsViewModel()
        Me.DataContext = settings

    End Sub
End Class




