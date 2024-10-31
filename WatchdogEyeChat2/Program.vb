Imports System.Diagnostics
Imports System.Threading

Module Program
    Sub Main()
        Dim appName As String = "EyeChat" ' Nom du processus sans extension .exe
        Dim appPath As String = "C:\MULLER\EyeChat\EyeChat.exe" ' Chemin de l'application

        Console.WriteLine("Watchdog d�marr�. Surveillance de " & appName)

        While True
            Dim isRunning As Boolean = Process.GetProcessesByName(appName).Length > 0

            If Not isRunning Then
                Console.WriteLine("Application non d�tect�e, relancement...")
                Try
                    Process.Start(appPath)
                    Console.WriteLine("Application relanc�e avec succ�s.")
                Catch ex As Exception
                    Console.WriteLine("Erreur lors du relancement : " & ex.Message)
                End Try
            End If
            ' Temporisation entre les v�rifications
            Thread.Sleep(5000) ' V�rifie toutes les 5 secondes
        End While
    End Sub
End Module
