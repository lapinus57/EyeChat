Imports MahApps.Metro.Controls.Dialogs
Imports MahApps.Metro
Imports MahApps.Metro.IconPacks
Imports System.Reflection
Imports log4net
Imports Newtonsoft.Json.Linq
Imports log4net.Core

Public Class GitHubViewModel

    Private dialogCoordinator As IDialogCoordinator
    Public Property Title As String
    Public Property AssemblyVersion As String
    Public Property FileVersion As String
    Public Property Description As String
    Public Property Company As String
    Public Property Copyright As String
    Public Property Trademark As String

    Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

    Public Sub New(dialogCoordinator As IDialogCoordinator)
        Me.dialogCoordinator = dialogCoordinator
        GetAssemblyInfos()
    End Sub

    Public Async Function DisplayMessageAsync(ByVal title As String, ByVal message As String) As Task
        Await dialogCoordinator.ShowMessageAsync(Me, title, message)
    End Function
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
                NotifyPropertyChanged("ArrowSize")
                logger.Info($"La propriété AppSizeDisplay a été modifiée : {value}")
            Catch ex As Exception
                ' Gérer l'exception ici (par exemple, enregistrer l'erreur dans les journaux)
                logger.Error($"Erreur lors de la modification de la propriété AppSizeDisplay : {ex.Message}")
            End Try
        End Set
    End Property

    Public ReadOnly Property ArrowSize As Double
        Get
            Return AppSizeDisplay * 0.8 ' Ajustez le coefficient selon vos préférences
        End Get
    End Property

    Private Sub NotifyPropertyChanged(v As String)
        Throw New NotImplementedException()
    End Sub

    Public Sub GetAssemblyInfos()
        Try
            Dim assembly As Assembly = Assembly.GetExecutingAssembly()
            Dim assemblyName As AssemblyName = assembly.GetName()

            ' Récupérer les informations de version
            Dim assemblyVersionAttribut As Version = assembly.GetName().Version
            AssemblyVersion = assemblyVersionAttribut.ToString()

            ' Obtenir la version du fichier
            FileVersion = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion

            ' Récupérer le titre de l'application
            Title = assemblyName.Name

            Dim CopyrightAttribute As AssemblyCopyrightAttribute = DirectCast(assembly.GetCustomAttribute(GetType(AssemblyCopyrightAttribute)), AssemblyCopyrightAttribute)
            Copyright = CopyrightAttribute.Copyright

            ' Récupérer la description de l'application
            Dim descriptionAttribute As AssemblyDescriptionAttribute = DirectCast(assembly.GetCustomAttribute(GetType(AssemblyDescriptionAttribute)), AssemblyDescriptionAttribute)
            Description = descriptionAttribute.Description

            ' Récupérer les informations du développeur
            Dim companyAttribute As AssemblyCompanyAttribute = DirectCast(assembly.GetCustomAttribute(GetType(AssemblyCompanyAttribute)), AssemblyCompanyAttribute)
            Company = companyAttribute.Company

            Trademark = DirectCast(assembly.GetCustomAttribute(GetType(AssemblyTrademarkAttribute)), AssemblyTrademarkAttribute).Trademark
            logger.Debug("Lecture de la propriété GetAssemblyInfos")
        Catch ex As Exception
            logger.Error($"Erreur lors de la lecture de la propriété GetAssemblyInfos : {ex.Message}")
        End Try

    End Sub

End Class
