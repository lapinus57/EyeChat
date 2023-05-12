
Imports System.Reflection

Public Class GithubWindows

    Public Property Title As String
    Public Property AssemblyVersion As String
    Public Property FileVersion As String
    Public Property Description As String
    Public Property Company As String
    Public Property Copyright As String
    Public Property Trademark As String

    Public Sub GetAssemblyInfos()
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
    End Sub

    Private Sub GithubWindows_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        GetAssemblyInfos()
        DataContext = Me
    End Sub

    Private Sub SendReport_Click(sender As Object, e As RoutedEventArgs)

    End Sub
End Class
