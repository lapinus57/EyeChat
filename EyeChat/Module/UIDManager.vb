Imports log4net

Public Class UIDManager
    Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

    Public Shared Function GenerateUniqueId() As String

        ' Génération d'un identifiant unique basé sur le nom d'utilisateur de Windows et le nom du PC
        logger.Debug("Génération de l'identifiant unique")
        Try
            Dim windowsUser As String = Environment.UserName
            Dim computerName As String = Environment.MachineName
            logger.Info($"UniqueId est : {windowsUser}_{computerName}")
            Return $"{windowsUser}_{computerName}"
        Catch ex As Exception
            logger.Error($"Erreur lors de la génération de l'identifiant unique : {ex.Message}")
            Return String.Empty
        End Try
    End Function
    Public Shared Function GetUniqueIdHashCode() As Integer
        ' Génération d'un integer unique basé sur le UniqueId
        logger.Debug("Génération de l'identifiant unique en hashcode")
        Try
            logger.Info($"HashCode de UniqueId est : {My.Settings.UniqueId.GetHashCode()}")
            Return My.Settings.UniqueId.GetHashCode()
        Catch ex As Exception
            logger.Error($"Erreur lors de la génération du hashcode de l'identifiant unique : {ex.Message}")
            Return 0
        End Try
    End Function
End Class
