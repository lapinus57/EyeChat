Imports System.IO
Imports log4net

Public Class FileWatcherSV
    Private Shared ReadOnly logger As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Private watcher As FileSystemWatcher
    Private iniFilePath As String
    Private mainWindow As MainWindow

    Public Sub New(iniFilePath As String, mainWindow As MainWindow)
        Try
            ' Vérifier si le fichier INI existe
            If Not File.Exists(iniFilePath) Then
                logger.Error("Le fichier INI n'existe pas : " & iniFilePath)
                Return
            End If
        Catch ex As Exception
            logger.Error("Erreur lors de la vérification de l'existence du fichier INI : " & ex.Message)
        End Try

        Me.iniFilePath = iniFilePath
        Me.mainWindow = mainWindow

        Try
            ' Initialisation du FileSystemWatcher pour surveiller les modifications du fichier INI
            watcher = New FileSystemWatcher()
            watcher.Path = Path.GetDirectoryName(iniFilePath)
            watcher.Filter = Path.GetFileName(iniFilePath)
            watcher.NotifyFilter = NotifyFilters.LastWrite
            AddHandler watcher.Changed, AddressOf OnIniFileChanged
            watcher.EnableRaisingEvents = True

            ' Lire initialement la valeur NumPat
            ReadNumPatValue()
        Catch ex As Exception
            logger.Error("Erreur lors de l'initialisation du FileSystemWatcher : " & ex.Message)
        End Try

    End Sub

    ' Méthode déclenchée lorsque le fichier INI est modifié
    Private Sub OnIniFileChanged(source As Object, e As FileSystemEventArgs)
        ' Lire la valeur NumPat à chaque modification du fichier
        ReadNumPatValue()
    End Sub

    ' Méthode pour lire la valeur NumPat du fichier INI
    Private Sub ReadNumPatValue()
        Try
            ' Vérifiez si le fichier existe
            If Not File.Exists(iniFilePath) Then
                logger.Error("Le fichier INI n'existe pas : " & iniFilePath)
                Return
            End If

            ' Ouvrir le fichier avec un accès partagé
            Using fs As New FileStream(iniFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                Using sr As New StreamReader(fs)
                    Dim line As String
                    While InlineAssignHelper(line, sr.ReadLine()) IsNot Nothing
                        ' Chercher la ligne contenant NumPat
                        If line.StartsWith("NumPat=") Then
                            Dim numPatValueStr As String = line.Split("="c)(1)
                            Dim numPatValue As Integer
                            ' Valider que la valeur est un nombre
                            If Integer.TryParse(numPatValueStr, numPatValue) Then
                                ' Mettre à jour la variable NumPatValue dans MainWindow
                                mainWindow.NumPatValue = numPatValue
                                logger.Info("La valeur NumPat a été mise à jour : " & numPatValue)
                            Else
                                logger.Error("La valeur NumPat dans le fichier INI n'est pas un nombre valide.")
                            End If
                            Exit While
                        End If
                    End While
                End Using
            End Using
        Catch ex As FileNotFoundException
            logger.Error("Fichier INI non trouvé : " & ex.Message)
        Catch ex As IOException
            logger.Error("Erreur d'entrée/sortie lors de la lecture du fichier INI : " & ex.Message)
        Catch ex As Exception
            logger.Error("Erreur lors de la lecture du fichier INI : " & ex.Message)
        End Try
    End Sub

    ' Méthode pour assigner une valeur à une variable et retourner cette valeur
    Private Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function
End Class
