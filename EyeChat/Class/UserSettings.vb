Imports System.Collections.ObjectModel
Imports System.IO
Imports Newtonsoft.Json
Public Class UserSettings

    <JsonProperty("UserName")>
    Public Property UserName As String

    <JsonProperty("UserAvatar")>
    Public Property UserAvatar As String

    <JsonProperty("AppTheme")>
    Public Property AppTheme As String

    <JsonProperty("AppColor")>
    Public Property AppColor As String

    <JsonProperty("AppColorHex")>
    Public Property AppColorHex As String

    <JsonProperty("AppColorString")>
    Public Property AppColorString As String

    <JsonProperty("AppSizeDisplay")>
    Public Property AppSizeDisplay As Double

    <JsonProperty("DebugLevel")>
    Public Property DebugLevel As String

    <JsonProperty("CtrlF9")>
    Public Property CtrlF9 As String

    <JsonProperty("CtrlF9Enabled")>
    Public Property CtrlF9Enabled As Boolean

    <JsonProperty("CtrlF10")>
    Public Property CtrlF10 As String

    <JsonProperty("CtrlF10Enabled")>
    Public Property CtrlF10Enabled As Boolean

    <JsonProperty("CtrlF11")>
    Public Property CtrlF11 As String

    <JsonProperty("CtrlF11Enabled")>
    Public Property CtrlF11Enabled As Boolean

    <JsonProperty("CtrlF12")>
    Public Property CtrlF12 As String

    <JsonProperty("CtrlF12Enabled")>
    Public Property CtrlF12Enabled As Boolean

    <JsonProperty("ShiftF9")>
    Public Property ShiftF9 As String

    <JsonProperty("ShiftF9Enabled")>
    Public Property ShiftF9Enabled As Boolean

    <JsonProperty("ShiftF10")>
    Public Property ShiftF10 As String

    <JsonProperty("ShiftF10Enabled")>
    Public Property ShiftF10Enabled As Boolean

    <JsonProperty("ShiftF11")>
    Public Property ShiftF11 As String

    <JsonProperty("ShiftF11Enabled")>
    Public Property ShiftF11Enabled As Boolean

    <JsonProperty("ShiftF12")>
    Public Property ShiftF12 As String

    <JsonProperty("ShiftF12Enabled")>
    Public Property ShiftF12Enabled As Boolean


    <JsonProperty("F5Enabled")>
    Public Property F5Enabled As Boolean

    <JsonProperty("F5Page1")>
    Public Property F5Page1 As String

    <JsonProperty("F5Text1")>
    Public Property F5Text1 As String

    <JsonProperty("F5Page2")>
    Public Property F5Page2 As String

    <JsonProperty("F5Text2")>
    Public Property F5Text2 As String

    <JsonProperty("F5Page3")>
    Public Property F5Page3 As String

    <JsonProperty("F5Text3")>
    Public Property F5Text3 As String

    <JsonProperty("F5Page4")>
    Public Property F5Page4 As String

    <JsonProperty("F5Text4")>
    Public Property F5Text4 As String

    <JsonProperty("F5Page5")>
    Public Property F5Page5 As String

    <JsonProperty("F5Text5")>
    Public Property F5Text5 As String

    <JsonProperty("F6Enabled")>
    Public Property F6Enabled As Boolean

    <JsonProperty("F6Page1")>
    Public Property F6Page1 As String

    <JsonProperty("F6Text1")>
    Public Property F6Text1 As String

    <JsonProperty("F6Page2")>
    Public Property F6Page2 As String

    <JsonProperty("F6Text2")>
    Public Property F6Text2 As String

    <JsonProperty("F6Page3")>
    Public Property F6Page3 As String

    <JsonProperty("F6Text3")>
    Public Property F6Text3 As String

    <JsonProperty("F6Page4")>
    Public Property F6Page4 As String

    <JsonProperty("F6Text4")>
    Public Property F6Text4 As String

    <JsonProperty("F6Page5")>
    Public Property F6Page5 As String

    <JsonProperty("F6Text5")>
    Public Property F6Text5 As String

    <JsonProperty("F7Enabled")>
    Public Property F7Enabled As Boolean

    <JsonProperty("F7Page1")>
    Public Property F7Page1 As String

    <JsonProperty("F7Text1")>
    Public Property F7Text1 As String

    <JsonProperty("F7Page2")>
    Public Property F7Page2 As String

    <JsonProperty("F7Text2")>
    Public Property F7Text2 As String

    <JsonProperty("F7Page3")>
    Public Property F7Page3 As String

    <JsonProperty("F7Text3")>
    Public Property F7Text3 As String

    <JsonProperty("F7Page4")>
    Public Property F7Page4 As String

    <JsonProperty("F7Text4")>
    Public Property F7Text4 As String

    <JsonProperty("F7Page5")>
    Public Property F7Page5 As String

    <JsonProperty("F7Text5")>
    Public Property F7Text5 As String

    <JsonProperty("F8Enabled")>
    Public Property F8Enabled As Boolean

    <JsonProperty("F8Page1")>
    Public Property F8Page1 As String

    <JsonProperty("F8Text1")>
    Public Property F8Text1 As String

    <JsonProperty("F8Page2")>
    Public Property F8Page2 As String

    <JsonProperty("F8Text2")>
    Public Property F8Text2 As String

    <JsonProperty("F8Page3")>
    Public Property F8Page3 As String

    <JsonProperty("F8Text3")>
    Public Property F8Text3 As String

    <JsonProperty("F8Page4")>
    Public Property F8Page4 As String

    <JsonProperty("F8Text4")>
    Public Property F8Text4 As String

    <JsonProperty("F8Page5")>
    Public Property F8Page5 As String

    <JsonProperty("F8Text5")>
    Public Property F8Text5 As String


    Public Property RoomDisplay As String



    Public Property OrthoMode As Boolean



End Class
