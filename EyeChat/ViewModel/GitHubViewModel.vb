Imports MahApps.Metro.Controls.Dialogs
Imports MahApps.Metro
Imports MahApps.Metro.IconPacks
Imports System.Reflection

Public Class GitHubViewModel

    Private dialogCoordinator As IDialogCoordinator
    Public Property Title As String
    Public Property AssemblyVersion As String
    Public Property FileVersion As String
    Public Property Description As String
    Public Property Company As String
    Public Property Copyright As String
    Public Property Trademark As String

    Public Sub New(dialogCoordinator As IDialogCoordinator)
        Me.dialogCoordinator = dialogCoordinator
        GetAssemblyInfos()
    End Sub

    Public Async Function DisplayMessageAsync(ByVal title As String, ByVal message As String) As Task
        Await dialogCoordinator.ShowMessageAsync(Me, title, message)
    End Function
    Public Property AppSizeDisplay As Integer
        Get
            Return My.Settings.AppSizeDisplay
        End Get
        Set(ByVal value As Integer)
            My.Settings.AppSizeDisplay = value
            My.Settings.Save()
            NotifyPropertyChanged("AppSizeDisplay")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(v As String)
        Throw New NotImplementedException()
    End Sub

    Public Sub GetAssemblyInfos()
        Dim assembly As Assembly = Assembly.GetExecutingAssembly()
        Dim assemblyName As AssemblyName = assembly.GetName()

        ' Récupérer les informations de version
        Dim assemblyVersionAttribut As Version = Assembly.GetName().Version
        AssemblyVersion = assemblyVersionAttribut.ToString()

        ' Obtenir la version du fichier
        FileVersion = FileVersionInfo.GetVersionInfo(Assembly.Location).FileVersion

        ' Récupérer le titre de l'application
        Title = assemblyName.Name

        Dim CopyrightAttribute As AssemblyCopyrightAttribute = DirectCast(Assembly.GetCustomAttribute(GetType(AssemblyCopyrightAttribute)), AssemblyCopyrightAttribute)
        Copyright = CopyrightAttribute.Copyright

        ' Récupérer la description de l'application
        Dim descriptionAttribute As AssemblyDescriptionAttribute = DirectCast(Assembly.GetCustomAttribute(GetType(AssemblyDescriptionAttribute)), AssemblyDescriptionAttribute)
        Description = descriptionAttribute.Description

        ' Récupérer les informations du développeur
        Dim companyAttribute As AssemblyCompanyAttribute = DirectCast(Assembly.GetCustomAttribute(GetType(AssemblyCompanyAttribute)), AssemblyCompanyAttribute)
        Company = companyAttribute.Company

        Trademark = DirectCast(Assembly.GetCustomAttribute(GetType(AssemblyTrademarkAttribute)), AssemblyTrademarkAttribute).Trademark
    End Sub

End Class
