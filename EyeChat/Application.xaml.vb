Imports log4net.Config
Class Application

    ' Les événements de niveau application, par exemple Startup, Exit et DispatcherUnhandledException
    ' peuvent être gérés dans ce fichier.
    Public Sub New()
        ' ...

        ' Activer la configuration de log4net à partir du fichier de configuration
        XmlConfigurator.Configure(New System.IO.FileInfo("log4net.config"))
    End Sub
End Class
