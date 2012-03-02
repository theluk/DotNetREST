Imports System.Web
Public Class Service
    Implements IHttpHandler

    Sub New()
    End Sub

    Sub New(ByVal routeContext As System.Web.Routing.RequestContext)
        _values = routeContext.RouteData.Values
        _isRouted = True
    End Sub

#Region "IHttpHandler Members"

    Private _isRouted As Boolean = False
    Private _values As IDictionary(Of String, Object)
    Private _ser As New System.Web.Script.Serialization.JavaScriptSerializer

    ReadOnly Property values As IDictionary(Of String, Object)
        Get
            Return _values
        End Get
    End Property

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property


    Overridable ReadOnly Property Serializer As System.Web.Script.Serialization.JavaScriptSerializer
        Get
            Return _ser
        End Get
    End Property

    Overridable ReadOnly Property RESTHandlers As IDictionary(Of String, Type)
        Get
            Return Configuration.RESTFullService.Current.RESTHandlers.GetDictionary
        End Get
    End Property

    Overridable ReadOnly Property DefaultOptions As IDictionary(Of String, Object)
        Get
            Return Configuration.RESTFullService.Current.RequestDefaultOptions.GetDictionary
        End Get
    End Property

    Protected Overridable Sub SetupOptions(ByVal dict As IDictionary(Of String, Object))
        For Each key As String In DefaultOptions.Keys
            If Not dict.ContainsKey(key) Then dict(key) = DefaultOptions(key)
        Next
    End Sub

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest


        If Not _isRouted Then
            _values = New Dictionary(Of String, Object)
        End If

        For Each item As String In context.Request.QueryString.Keys
            If Not values.ContainsKey(item) Then
                _values(item) = context.Request.QueryString(item)
            Else
                _values("q_" & item) = context.Request.QueryString(item)
            End If
        Next

        SetupOptions(values)

        Dim type = RESTHandlers(values("type"))

        Dim handler = CType(Activator.CreateInstance(type), ModelRESTHandler)
        handler._httpContext = context
        handler._Options = values

        Dim result As ModelRESTUpdateResult = Nothing

        Select Case context.Request.HttpMethod
            Case "POST"
                result = handler.Insert(Serializer.Deserialize(context.Request.Form("model"), handler.TargetType))
            Case "PUT"
                result = handler.Update(Serializer.Deserialize(context.Request.Form("model"), handler.TargetType))
            Case "DELETE"
                result = handler.Delete(values("id"))
            Case "GET"
                result = If(values("id") Is Nothing, handler.Get(), handler.Get(values("id")))
        End Select

        ProcessResult(context, result)

    End Sub

    Overridable Sub ProcessResult(ByVal context As HttpContext, ByVal result As ModelRESTUpdateResult)
        Dim r As HttpResponse = context.Response

        If Not result.success Then
            r.StatusCode = 500
            r.ContentType = "text/plain"
            r.Write(result.message)
            r.End()
            Return
        End If

        Dim responseSerializer As SerializerBase = Nothing
        Dim s = New SerializerBase() {
            New JsonSerializer With {._values = values},
            New JsonpSerializer With {._values = values},
            New XmlSerializer With {._values = values}
        }

        responseSerializer = (From i In s Where (From accept In i.Accepts Where context.Request.AcceptTypes.Contains(accept)).Any
                              Select i).FirstOrDefault

        If result.item IsNot Nothing Then

            r.StatusCode = 200
            r.ContentType = responseSerializer.ReturnsType
            r.Write(responseSerializer.GetContent(result.item))
            r.End()
            Return

        End If

        If result.items IsNot Nothing Then

            r.StatusCode = 200
            r.ContentType = responseSerializer.ReturnsType
            r.Write(responseSerializer.GetContent(result.items))
            r.End()
            Return

        End If


        r.StatusCode = 200
        r.End()
        Return

    End Sub

#End Region

End Class
