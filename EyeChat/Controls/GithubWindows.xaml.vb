
Imports System.Reflection

Public Class GithubWindows

    Public Sub GetAssemblyInfos()
        Dim assembly As Assembly = Assembly.GetExecutingAssembly()
        Dim assemblyName As AssemblyName = assembly.GetName()

        ' Récupérer les informations de version
        Dim assemblyVersionAttribut As Version = assembly.GetName().Version
        Dim assemblyVersion As String = assemblyVersionAttribut.ToString()

        ' Obtenir la version du fichier
        Dim FileVersion As String = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion

        ' Récupérer le titre de l'application
        Dim title As String = assemblyName.Name


        Dim CopyrightAttribute As AssemblyCopyrightAttribute = DirectCast(assembly.GetCustomAttribute(GetType(AssemblyCopyrightAttribute)), AssemblyCopyrightAttribute)
        Dim Copyright As String = CopyrightAttribute.Copyright

        ' Récupérer la description de l'application
        Dim descriptionAttribute As AssemblyDescriptionAttribute = DirectCast(assembly.GetCustomAttribute(GetType(AssemblyDescriptionAttribute)), AssemblyDescriptionAttribute)
        Dim description As String = descriptionAttribute.Description

        ' Récupérer les informations du développeur
        Dim companyAttribute As AssemblyCompanyAttribute = DirectCast(assembly.GetCustomAttribute(GetType(AssemblyCompanyAttribute)), AssemblyCompanyAttribute)
        Dim company As String = companyAttribute.Company

        Dim Trademark As String = DirectCast(assembly.GetCustomAttribute(GetType(AssemblyTrademarkAttribute)), AssemblyTrademarkAttribute).Trademark

        ' Générer le texte à propos avec les informations de l'assembly
        Dim aboutText As String = $"{title}{Environment.NewLine}" &
                                  $"Version : {assemblyVersion}{Environment.NewLine}" &
                                  $"Fichier : {FileVersion}{Environment.NewLine}" &
                                  $"{description}{Environment.NewLine}" &
                                  $"Développé par {company}{Environment.NewLine}" &
                                  $"{Copyright}{Environment.NewLine}" &
                                  $"{Trademark}"

        AboutTextBlock.Text = aboutText

    End Sub

    Private Sub GithubWindows_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        GetAssemblyInfos()
    End Sub
End Class
