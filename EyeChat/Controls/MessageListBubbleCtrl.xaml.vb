Imports System.Collections.ObjectModel

Public Class MessageListBubbleCtrl
    Inherits UserControl

    Public Shared ReadOnly MessagesProperty As DependencyProperty = DependencyProperty.Register("Messages", GetType(ObservableCollection(Of Message)), GetType(MessageListBubbleCtrl))

    Public Property Messages As ObservableCollection(Of Message)
        Get
            Return CType(GetValue(MessagesProperty), ObservableCollection(Of Message))
        End Get
        Set(value As ObservableCollection(Of Message))
            SetValue(MessagesProperty, value)
        End Set
    End Property

    Public Sub New()
        InitializeComponent()
    End Sub
    Public Sub ScrollToEnd()
        ScrollViewer.ScrollToEnd()
    End Sub
End Class
