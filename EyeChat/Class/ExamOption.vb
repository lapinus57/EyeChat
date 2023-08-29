Imports System.ComponentModel

Public Class ExamOption
    Implements INotifyPropertyChanged

    Private _name As String
    Public Property Name As String
        Get
            Return _name
        End Get
        Set(value As String)
            If _name <> value Then
                _name = value
                NotifyPropertyChanged("Name")
            End If
        End Set
    End Property

    Private _color As String
    Public Property Color As String
        Get
            Return _color
        End Get
        Set(value As String)
            If _color <> value Then
                _color = value
                NotifyPropertyChanged("Color")
            End If
        End Set
    End Property

    Private _index As String
    Public Property index As String
        Get
            Return _index
        End Get
        Set(value As String)
            If _index <> value Then
                _index = value
                NotifyPropertyChanged("Index")
            End If
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub NotifyPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class