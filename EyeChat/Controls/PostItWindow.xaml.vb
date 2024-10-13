Imports System.ComponentModel

Public Class PostItWindow
    Private isProgrammaticClose As Boolean = False ' Indique si la fermeture est faite par le programme

    ' Méthode pour fermer la fenêtre programmatiquement
    Public Sub CloseProgrammatically()
        isProgrammaticClose = True
        Me.Close()
    End Sub
    Public Sub New(content As String)
        InitializeComponent()

        ' Assigner la chaîne passée au TextBlock
        PostItContent.Text = content

        ' Désactiver les boutons de maximisation et de minimisation
        Me.ShowMaxRestoreButton = False
        Me.ShowMinButton = False
        ' Garder la fenêtre au premier plan
        Me.Topmost = True
    End Sub
    ' Méthode pour mettre à jour le contenu du post-it
    Public Sub UpdateContent(content As String)
        PostItContent.Text = content
    End Sub
    Private Sub PostItWindow_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Me.MouseDown
        If e.ChangedButton = MouseButton.Left Then
            Me.DragMove()
        End If
    End Sub

    ' Événement déclenché lors de la fermeture de la fenêtre
    Private Sub PostItWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        ' Vérifier si la fermeture est manuelle ou programmée
        If Not isProgrammaticClose Then
            If MsgBox("Voulez-vous vraiment fermer cette note ?", MsgBoxStyle.YesNo, "Fermeture de la note") = MsgBoxResult.No Then
                e.Cancel = True ' Annuler la fermeture si l'utilisateur sélectionne "Non"
            Else
                'SendManager.SendMessage("MSG01Benoit|A Tous|" & My.Settings.WindowsName & "|postit fermer|avataaars.png")
                MainWindow.postItWindow = Nothing
                MainWindow.isPostItOpened = False
            End If
        Else
            ' Si la fermeture est programmée, ne pas afficher le MsgBox
            'SendManager.SendMessage("MSG01Benoit|A Tous|" & My.Settings.WindowsName & "|postit cacher|avataaars.png")
        End If
    End Sub
End Class
