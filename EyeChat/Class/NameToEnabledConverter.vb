Imports System.Globalization
Imports System.Windows
Imports System.Windows.Data

Public Class NameToEnabledConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim name As String = value.ToString()
        Dim visibility As Visibility = Visibility.Visible

        ' Vérifier si le nom correspond à "A Tous" ou "Secrétariat"
        If name = "A Tous" OrElse name = "Secrétariat" Then
            visibility = Visibility.Collapsed
        End If

        Return visibility
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class