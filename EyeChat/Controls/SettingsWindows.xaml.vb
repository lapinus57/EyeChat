Imports ControlzEx.Theming
Imports MahApps.Metro.Controls

Public Class SettingsWindows

    Private _settings As SettingsWrapper
    Public ReadOnly Property Settings As SettingsWrapper
        Get
            If _settings Is Nothing Then
                _settings = New SettingsWrapper()
            End If
            Return _settings
        End Get
    End Property

    Public Sub New()

    End Sub


    Private Sub AppThemeChanged(sender As Object, e As SelectionChangedEventArgs)
        ' Construire le nom du thème en utilisant les variables VariableAppTheme et VariableAppTheme
        Dim themeName As String = If(My.Settings.AppTheme = "Clair", "Light", "Dark") & "." & My.Settings.AppColor
        ThemeManager.Current.ChangeTheme(Application.Current, themeName)
    End Sub

    Private Sub SettingsWindows_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        InitializeComponent()
        Dim settings As New SettingsWrapper()
        Me.DataContext = settings

    End Sub
End Class




