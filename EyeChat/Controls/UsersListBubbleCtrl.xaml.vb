Imports System.Collections.ObjectModel

Public Class UsersListBubbleCtrl
    Inherits UserControl
    Public Shared ReadOnly UsersProperty As DependencyProperty = DependencyProperty.Register("Users", GetType(ObservableCollection(Of User)), GetType(UsersListBubbleCtrl))

    Public Property Users As ObservableCollection(Of User)
        Get
            Return CType(GetValue(UsersProperty), ObservableCollection(Of User))
        End Get
        Set(value As ObservableCollection(Of User))
            SetValue(UsersProperty, value)
        End Set
    End Property



    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().

    End Sub



End Class
