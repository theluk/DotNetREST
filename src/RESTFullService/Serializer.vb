Imports System.Xml.Serialization

Public Class SerializerBase

    Overridable ReadOnly Property Accepts As String()
        Get
            Return Nothing
        End Get
    End Property

    Overridable ReadOnly Property ReturnsType As String
        Get
            Return Nothing
        End Get
    End Property

    Overridable Function GetContent(ByVal data As Object) As String
        Return Nothing
    End Function

    Friend _values As IDictionary(Of String, Object)
    Public ReadOnly Property Options As IDictionary(Of String, Object)
        Get
            Return _values
        End Get
    End Property

End Class

Public Class JsonSerializer
    Inherits SerializerBase

    Protected Friend Shared ser As New System.Web.Script.Serialization.JavaScriptSerializer

    Overrides ReadOnly Property Accepts As String()
        Get
            Return New String() {"application/json", "text/json"}
        End Get
    End Property


    Public Overrides Function GetContent(ByVal data As Object) As String
        Return ser.Serialize(data)
    End Function

    Overrides ReadOnly Property ReturnsType As String
        Get
            Return "application/json"
        End Get
    End Property

End Class

Public Class JsonpSerializer
    Inherits JsonSerializer

    Public Overrides ReadOnly Property Accepts As String()
        Get
            Return New String() {"text/javascript", "application/javascript", "application/ecmascript", "application/x-ecmascript"}
        End Get
    End Property

    Public Overrides ReadOnly Property ReturnsType As String
        Get
            Return "text/javascript"
        End Get
    End Property

    Public Overrides Function GetContent(ByVal data As Object) As String
        Return Me.Options("callback").ToString & "(" & ser.Serialize(data) & ");"
    End Function

End Class

Public Class XmlSerializer
    Inherits SerializerBase

    Public Overrides ReadOnly Property Accepts As String()
        Get
            Return New String() {"application/xml", "text/xml"}
        End Get
    End Property

    Public Overrides ReadOnly Property ReturnsType As String
        Get
            Return "application/xml"
        End Get
    End Property

    Public Overrides Function GetContent(ByVal data As Object) As String
        Dim xml As New System.Xml.Serialization.XmlSerializer(data.GetType)
        Dim result As String = ""
        Using st As New IO.StringWriter
            xml.Serialize(st, data)
            result = st.ToString
        End Using
        Return result
    End Function

End Class
