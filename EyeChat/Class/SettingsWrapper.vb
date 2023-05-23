Imports System.ComponentModel
Imports ControlzEx.Theming
Imports System.Runtime
Imports System.Drawing
Imports MahApps.Metro
Imports System.Windows.Media

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

    Public Shared Sub SetTheme()
        'Récupération de la couleur de base et traduction
        Dim themeName As String = If(My.Settings.AppTheme = "Clair", "Light", "Dark")

        Dim mediaColor As System.Windows.Media.Color = System.Windows.Media.Color.FromArgb(
        My.Settings.AppColor.A,
        My.Settings.AppColor.R,
        My.Settings.AppColor.G,
        My.Settings.AppColor.B)

        Dim brush As New System.Windows.Media.SolidColorBrush(mediaColor)

        ' Créer un nouvel objet Theme en utilisant les valeurs des paramètres stockées dans les Settings par défaut
        Dim newTheme As New Theme(
        "AppTheme",                                      ' Nom du thème
        "AppTheme",                                      ' Nom affiché du thème
        themeName,                                       ' Nom du thème de base 
        My.Settings.AppColor.ToString(),                 ' Nom de la couleur d'accent stockée dans les Settings
        mediaColor,                                      ' Couleur d'accent stockée dans les Settings
        New SolidColorBrush(mediaColor),                 ' Pinceau pour la couleur d'accent
        True,                                            ' Utiliser les couleurs système
        False                                            ' Appliquer uniquement aux contrôles MahApps.Metro
    )

        ' Changer le thème de l'application en utilisant le nouvel objet Theme
        ThemeManager.Current.ChangeTheme(System.Windows.Application.Current, newTheme)
    End Sub
End Class
