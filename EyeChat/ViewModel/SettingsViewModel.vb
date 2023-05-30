Imports System.ComponentModel
Imports ControlzEx.Theming
Imports System.Drawing
Imports System.Collections.ObjectModel
Imports System.Globalization
Imports System.Reflection
Imports Microsoft.VisualBasic.ApplicationServices
Imports log4net
Imports Newtonsoft.Json.Linq
Imports log4net.Core

Public Class SettingsViewModel
    Implements INotifyPropertyChanged

    Public Property ColorItems As New ObservableCollection(Of ColorItemViewModel)()
    Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Public Property DebugLevels As New ObservableCollection(Of String)() From {"DEBUG", "INFO", "WARN", "ERROR"}


    Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged

    Public Sub New()


    End Sub

    Public Property UserName As String
        Get
            Try
                Return My.Settings.UserName
                logger.Debug($"Lecture de la propriété UserName : {My.Settings.UserName}")
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété UserName : {ex.Message}")
                Return "EyeChat"
            End Try

        End Get
        Set(ByVal value As String)
            Try
                My.Settings.UserName = value
                My.Settings.Save()
                NotifyPropertyChanged("UserName")
                logger.Info($"La propriété UserName a été modifiée : {value}")
            Catch ex As Exception
                ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
                logger.Error($"Erreur lors de la modification de la propriété UserName : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property AppTheme As String
        Get
            Try
                Return My.Settings.AppTheme
                logger.Debug($"Lecture de la propriété AppTheme : {My.Settings.AppTheme}")
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété AppTheme : {ex.Message}")
                Return "White"
            End Try

        End Get
        Set(ByVal value As String)
            Try
                My.Settings.AppTheme = value
                My.Settings.Save()
                NotifyPropertyChanged("AppTheme")
                logger.Info($"La propriété AppTheme a été modifiée : {value}")
            Catch ex As Exception
                ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
                logger.Error($"Erreur lors de la modification de la propriété AppTheme : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property AppSizeDisplay As Integer
        Get
            Try
                Return My.Settings.AppSizeDisplay
                logger.Debug($"Lecture de la propriété AppSizeDisplay : {My.Settings.AppSizeDisplay}")
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété AppSizeDisplay : {ex.Message}")
                Return "14"
            End Try

        End Get
        Set(ByVal value As Integer)
            Try
                My.Settings.AppSizeDisplay = value
                My.Settings.Save()
                NotifyPropertyChanged("AppSizeDisplay")
                logger.Info($"La propriété AppSizeDisplay a été modifiée : {value}")
            Catch ex As Exception
                ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
                logger.Error($"Erreur lors de la modification de la propriété AppSizeDisplay : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property AppColorString As String
        Get
            Try
                Return My.Settings.AppColorString
                logger.Debug($"Lecture de la propriété AppColorString : {My.Settings.AppColorString}")
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété AppColorString : {ex.Message}")
                Return "Blue"
            End Try

        End Get
        Set(ByVal value As String)

            Try
                Dim converter As New System.Windows.Media.ColorConverter()
                My.Settings.AppColorString = value
                My.Settings.AppColor = System.Drawing.Color.FromName(value)
                My.Settings.Save()
                NotifyPropertyChanged("AppColorString")
                logger.Info($"La propriété AppColorString a été modifiée : {value}")
            Catch ex As Exception
                ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
                logger.Error($"Erreur lors de la modification de la propriété AppColorString : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property AppColor As Color
        Get
            Try
                Return My.Settings.AppColor
                logger.Debug($"Lecture de la propriété AppColor : {My.Settings.AppColor}")
            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété AppColor : {ex.Message}")
                Return Color.Blue
            End Try

        End Get
        Set(ByVal value As Color)
            Try
                My.Settings.AppColor = value
                My.Settings.Save()
                NotifyPropertyChanged("AppColor")
                logger.Info($"La propriété AppColor a été modifiée : {value}")
            Catch ex As Exception
                ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
                logger.Error($"Erreur lors de la modification de la propriété AppColor : {ex.Message}")
            End Try
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Shared Sub SetTheme()
        Try
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
            My.Settings.AppColorString,                      ' Nom de la couleur d'accent stockée dans les Settings
            mediaColor,                                      ' Couleur d'accent stockée dans les Settings
            New SolidColorBrush(mediaColor),                 ' Pinceau pour la couleur d'accent
            True,                                            ' Utiliser les couleurs système
            False                                            ' Appliquer uniquement aux contrôles MahApps.Metro
        )

            ' Changer le thème de l'application en utilisant le nouvel objet Theme
            ThemeManager.Current.ChangeTheme(System.Windows.Application.Current, newTheme)
            logger.Info($"La propriété ChangeTheme a été modifiée : {themeName} {My.Settings.AppColorString}")
        Catch ex As Exception
            ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
            logger.Error($"Erreur lors de la modification de la propriété ChangeTheme : {ex.Message}")
        End Try

    End Sub

    Public Property SelectedDebugLevel As String
        Get
            Return My.Settings.DebugLevel
        End Get
        Set(ByVal value As String)
            My.Settings.DebugLevel = value
            My.Settings.Save()

            ' Vérifier le niveau de débogage sélectionné
            Select Case value
                Case "DEBUG"
                    logger.Logger.Repository.Threshold = log4net.Core.Level.Debug
                Case "INFO"
                    logger.Logger.Repository.Threshold = log4net.Core.Level.Info
                Case "WARN"
                    logger.Logger.Repository.Threshold = log4net.Core.Level.Warn
                Case "ERROR"
                    logger.Logger.Repository.Threshold = log4net.Core.Level.Error
            End Select

            logger.Info($"Le niveau de débogage a été modifié : {value}")
            NotifyPropertyChanged("SelectedDebugLevel")
        End Set
    End Property

End Class
