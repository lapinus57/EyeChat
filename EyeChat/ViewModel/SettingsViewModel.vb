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
Imports EyeChat.MainWindow
Imports EyeChat.User



Public Class SettingsViewModel
        Implements INotifyPropertyChanged

        Public Property ColorItems As New ObservableCollection(Of ColorItemViewModel)()
        Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
        Public Property DebugLevels As New ObservableCollection(Of String)() From {"DEBUG", "INFO", "WARN", "ERROR"}


        Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged

    Public Sub New()


    End Sub


    Public Property AppNameDisplay As String
        Get
            Try
                logger.Debug($"Lecture de la propriété  AppNameDisplay : {My.Settings.AppNameDisplay}")
                Return My.Settings.AppNameDisplay

            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété  AppNameDisplay : {ex.Message}")
                Return "EyeChat"
            End Try

        End Get
        Set(ByVal value As String)
            Try
                My.Settings.AppNameDisplay = value
                My.Settings.Save()
                NotifyPropertyChanged("AppNameDisplay")
                logger.Info($"La propriété  AppNameDisplay a été modifiée : {value}")
            Catch ex As Exception
                ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
                logger.Error($"Erreur lors de la modification de la propriété AppNameDisplay : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property ComputerName As String
        Get
            Try
                logger.Debug($"Lecture de la propriété  ComputerName : {My.Settings.ComputerName}")
                Return My.Settings.ComputerName

            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété ComputerName : {ex.Message}")
                Return "Bender"
            End Try

        End Get
        Set(ByVal value As String)
            Try
                My.Settings.ComputerName = value
                My.Settings.Save()
                NotifyPropertyChanged("ComputerName")
                logger.Info($"La propriété  ComputerName a été modifiée : {value}")
            Catch ex As Exception
                ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
                logger.Error($"Erreur lors de la modification de la propriété ComputerName : {ex.Message}")
            End Try
        End Set
    End Property

    Public Property WindowsName As String
        Get
            Try
                logger.Debug($"Lecture de la propriété  ComputerName : {My.Settings.WindowsName}")
                Return My.Settings.WindowsName

            Catch ex As Exception
                logger.Error($"Erreur lors de la lecture de la propriété WindowsName : {ex.Message}")
                Return "Bender"
            End Try

        End Get
        Set(ByVal value As String)
            Try
                My.Settings.WindowsName = value
                My.Settings.Save()
                NotifyPropertyChanged("WindowsName")
                logger.Info($"La propriété WindowsName a été modifiée : {value}")
            Catch ex As Exception
                ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
                logger.Error($"Erreur lors de la modification de la propriété WindowsName : {ex.Message}")
            End Try
        End Set
    End Property
    Public Property UserName As String
            Get
                Try
                    logger.Debug($"Lecture de la propriété UserName : {My.Settings.UserName}")
                    Return My.Settings.UserName

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
                    logger.Debug($"Lecture de la propriété AppTheme : {My.Settings.AppTheme}")
                    Return My.Settings.AppTheme

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
                    logger.Debug($"Lecture de la propriété AppSizeDisplay : {My.Settings.AppSizeDisplay}")
                    Return My.Settings.AppSizeDisplay

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
                NotifyPropertyChanged("ArrowSize")
                SelectUser(SelectedUser)
                Users.Clear()


                Dim loadedUsers = LoadUsersFromJson()
                For Each user In loadedUsers
                    Users.Add(user)
                Next
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
                    logger.Debug($"Lecture de la propriété AppColorString : {My.Settings.AppColorString}")
                    Return My.Settings.AppColorString

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
                SelectUser(SelectedUser)
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
                    logger.Debug($"Lecture de la propriété AppColor : {My.Settings.AppColor}")
                    Return My.Settings.AppColor

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
                SelectUser(SelectedUser)

                logger.Info($"La propriété AppColor a été modifiée : {value}")
            Catch ex As Exception
                    ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
                    logger.Error($"Erreur lors de la modification de la propriété AppColor : {ex.Message}")
                End Try
            End Set
        End Property

        Public Sub NotifyPropertyChanged(ByVal propertyName As String)
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
                Try
                    logger.Debug($"Lecture de la propriété SelectedDebugLevel : {My.Settings.DebugLevel}")
                    Return My.Settings.DebugLevel
                Catch ex As Exception
                    logger.Error($"Erreur lors de la lecture de la propriété SelectedDebugLevel : {ex.Message}")
                    Return "DEBUG"
                End Try

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

        Public Property CtrlF9 As String
            Get
                Try
                    logger.Debug($"Lecture de la propriété CtrlF9 : {My.Settings.CtrlF9}")
                    Return My.Settings.CtrlF9
                Catch ex As Exception
                    logger.Error($"Erreur lors de la lecture de la propriété CtrlF9 : {ex.Message}")
                    Return String.Empty
                End Try
            End Get
            Set(ByVal value As String)
                Try
                    My.Settings.CtrlF9 = value
                    My.Settings.Save()
                    NotifyPropertyChanged("CtrlF9")
                    logger.Info($"La propriété CtrlF9 a été modifiée : {value}")
                Catch ex As Exception
                    logger.Error($"Erreur lors de la modification de la propriété CtrlF9 : {ex.Message}")
                End Try
            End Set
        End Property

        Public Property CtrlF10 As String
            Get
                Try
                    logger.Debug($"Lecture de la propriété CtrlF10 : {My.Settings.CtrlF10}")
                    Return My.Settings.CtrlF10
                Catch ex As Exception
                    logger.Error($"Erreur lors de la lecture de la propriété CtrlF10 : {ex.Message}")
                    Return String.Empty
                End Try
            End Get
            Set(ByVal value As String)
                Try
                    My.Settings.CtrlF10 = value
                    My.Settings.Save()
                    NotifyPropertyChanged("CtrlF10")
                    logger.Info($"La propriété CtrlF10 a été modifiée : {value}")
                Catch ex As Exception
                    logger.Error($"Erreur lors de la modification de la propriété CtrlF10 : {ex.Message}")
                End Try
            End Set
        End Property
        Public Property CtrlF11 As String
            Get
                Try
                    logger.Debug($"Lecture de la propriété CtrlF11 : {My.Settings.CtrlF11}")
                    Return My.Settings.CtrlF11

                Catch ex As Exception
                    logger.Error($"Erreur lors de la lecture de la propriété CtrlF11 : {ex.Message}")
                    Return String.Empty
                End Try
            End Get
            Set(ByVal value As String)
                Try
                    My.Settings.CtrlF11 = value
                    My.Settings.Save()
                    NotifyPropertyChanged("CtrlF11")
                    logger.Info($"La propriété CtrlF11 a été modifiée : {value}")
                Catch ex As Exception
                    logger.Error($"Erreur lors de la modification de la propriété CtrlF11 : {ex.Message}")
                End Try
            End Set
        End Property

        Public Property CtrlF12 As String
            Get
                Try
                    logger.Debug($"Lecture de la propriété CtrlF12 : {My.Settings.CtrlF12}")
                    Return My.Settings.CtrlF12
                Catch ex As Exception
                    logger.Error($"Erreur lors de la lecture de la propriété CtrlF12 : {ex.Message}")
                    Return String.Empty
                End Try
            End Get
            Set(ByVal value As String)
                Try
                    My.Settings.CtrlF12 = value
                    My.Settings.Save()
                    NotifyPropertyChanged("CtrlF12")
                    logger.Info($"La propriété CtrlF12 a été modifiée : {value}")
                Catch ex As Exception
                    logger.Error($"Erreur lors de la modification de la propriété CtrlF12 : {ex.Message}")
                End Try
            End Set
        End Property
        Public Property AltF9 As String
            Get
                Try
                    logger.Debug($"Lecture de la propriété AltF9 : {My.Settings.AltF9}")
                    Return My.Settings.AltF9
                Catch ex As Exception
                    logger.Error($"Erreur lors de la lecture de la propriété AltF9 : {ex.Message}")
                    Return String.Empty
                End Try
            End Get
            Set(ByVal value As String)
                Try
                    My.Settings.AltF9 = value
                    My.Settings.Save()
                    NotifyPropertyChanged("AltF9")
                    logger.Info($"La propriété AltF9 a été modifiée : {value}")
                Catch ex As Exception
                    logger.Error($"Erreur lors de la modification de la propriété AltF9 : {ex.Message}")
                End Try
            End Set
        End Property

        Public Property AltF10 As String
            Get
                Try
                    logger.Debug($"Lecture de la propriété AltF10 : {My.Settings.AltF10}")
                    Return My.Settings.AltF10
                Catch ex As Exception
                    logger.Error($"Erreur lors de la lecture de la propriété AltF10 : {ex.Message}")
                    Return String.Empty
                End Try
            End Get
            Set(ByVal value As String)
                Try
                    My.Settings.AltF10 = value
                    My.Settings.Save()
                    NotifyPropertyChanged("AltF10")
                    logger.Info($"La propriété AltF10 a été modifiée : {value}")
                Catch ex As Exception
                    logger.Error($"Erreur lors de la modification de la propriété AltF10 : {ex.Message}")
                End Try
            End Set
        End Property

        Public Property AltF11 As String
            Get
                Try
                    logger.Debug($"Lecture de la propriété AltF11 : {My.Settings.AltF11}")
                    Return My.Settings.AltF11
                Catch ex As Exception
                    logger.Error($"Erreur lors de la lecture de la propriété AltF11 : {ex.Message}")
                    Return String.Empty
                End Try
            End Get
            Set(ByVal value As String)
                Try
                    My.Settings.AltF11 = value
                    My.Settings.Save()
                    NotifyPropertyChanged("AltF11")
                    logger.Info($"La propriété AltF11 a été modifiée : {value}")
                Catch ex As Exception
                    logger.Error($"Erreur lors de la modification de la propriété AltF11 : {ex.Message}")
                End Try
            End Set
        End Property

        Public Property AltF12 As String
            Get
                Try
                    logger.Debug($"Lecture de la propriété AltF12 : {My.Settings.AltF12}")
                    Return My.Settings.AltF12
                Catch ex As Exception
                    logger.Error($"Erreur lors de la lecture de la propriété AltF12 : {ex.Message}")
                    Return String.Empty
                End Try
            End Get
            Set(ByVal value As String)
                Try
                    My.Settings.AltF12 = value
                    My.Settings.Save()
                    NotifyPropertyChanged("AltF12")
                    logger.Info($"La propriété AltF12 a été modifiée : {value}")
                Catch ex As Exception
                    logger.Error($"Erreur lors de la modification de la propriété AltF12 : {ex.Message}")
                End Try
            End Set
        End Property

        Public ReadOnly Property ArrowSize As Double
            Get
                Return AppSizeDisplay * 0.8 ' Ajustez le coefficient selon vos préférences
            End Get
        End Property

        Public Property RoomDisplay As Boolean
            Get
                Return My.Settings.RoomDisplay
            End Get
            Set(value As Boolean)
                My.Settings.RoomDisplay = value
                If value = True Then
                    My.Settings.RoomDisplayStr = "Visible"
                Else
                    My.Settings.RoomDisplayStr = "Collapsed"
                End If
                My.Settings.Save()
                NotifyPropertyChanged("RoomDisplay")
                NotifyPropertyChanged("RoomDisplayStr")
            End Set
        End Property

        Public Property RoomDisplayStr As String
            Get
                Return My.Settings.RoomDisplayStr

            End Get
            Set(value As String)
                My.Settings.RoomDisplayStr = value
                NotifyPropertyChanged("RoomDisplayStr")
            End Set
        End Property



    End Class


