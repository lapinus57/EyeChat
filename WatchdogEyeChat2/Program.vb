Imports System.Diagnostics
Imports System.Threading

Module Program
    Sub Main()
        Dim appName As String = "EyeChat" ' Nom du processus sans extension .exe
        Dim appPath As String = "C:\MULLER\EyeChat\EyeChat.exe" ' Chemin de l'application

        Console.WriteLine("Watchdog démarré. Surveillance de " & appName)

        While True
            Dim isRunning As Boolean = Process.GetProcessesByName(appName).Length > 0

            If Not isRunning Then
                Console.WriteLine("Application non détectée, relancement...")
                Try
                    Process.Start(appPath)
                    Console.WriteLine("Application relancée avec succès.")
                Catch ex As Exception
                    Console.WriteLine("Erreur lors du relancement : " & ex.Message)
                End Try
            End If
            ' Temporisation entre les vérifications
            Thread.Sleep(5000) ' Vérifie toutes les 5 secondes
        End While
    End Sub
End Module
