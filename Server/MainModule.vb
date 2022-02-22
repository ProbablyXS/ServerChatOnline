Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Module MainModule

    Dim _server As TcpListener
    Dim clients As New List(Of TcpClient)
    Dim clientName As New List(Of String)
    Dim clientID As Integer

    Sub Main()

        Try
            Dim hostName As String = Dns.GetHostName()
            Dim ip As String
            ip = Dns.GetHostByName(hostName).AddressList(0).ToString()
            Dim port As Integer = 5656

            _server = New TcpListener(IPAddress.Parse(ip), port)
            _server.Start()

            Threading.ThreadPool.QueueUserWorkItem(AddressOf NewClient)

        Catch ex As Exception
        End Try

        While True

            Console.Write("Insert your command here: ")
            Dim TXTCommand As String = Console.ReadLine()

            If TXTCommand = "list" Then

                If clients.Count = 0 Then

                    Console.WriteLine("No client connected ")

                Else

                    Try
                        For i As Integer = 0 To clients.Count
                            Console.WriteLine(clients(i).Client.RemoteEndPoint.ToString & Space(1) & clientName(i) & Space(1) & "ClientID: " & i)
                        Next

                    Catch ex As Exception

                    End Try

                    Console.WriteLine("Total Clients connected: " & clients.Count().ToString.Trim)

                End If





            ElseIf TXTCommand = "kick" Then

                If clients.Count = 0 Then

                    Console.WriteLine("No client connected ")

                Else

                    Console.Write("Insert your number here: ")
                    Dim numb As Integer = Console.ReadLine()
                    clientID = numb
                    clients(numb).Client.Close()

                End If


            ElseIf TXTCommand = "kick all" Then

                If clients.Count = 0 Then

                    Console.WriteLine("No client connected ")

                Else

                    Do Until clients.Count = 0

                        clients(0).Client.Close()
                    Loop

                    Console.WriteLine("Everyone was kicked")

                End If



            ElseIf TXTCommand = "help" Then
                Console.WriteLine("Command available: list,kick,kick all,help")
            End If

        End While

    End Sub

    Private Sub NewClient(state As Object)

        Dim client As TcpClient = _server.AcceptTcpClient()
        Threading.ThreadPool.QueueUserWorkItem(AddressOf NewClient)

        Try

            While True

                Dim ns As NetworkStream = client.GetStream()
                Dim toReceive(100000) As Byte
                Dim length As Integer = ns.Read(toReceive, 0, toReceive.Length)
                Dim text As String = Encoding.UTF8.GetString(toReceive, 0, length)

                If clients.Contains(client) = False Then
                    clients.Add(client)
                    clientName.Add(text)
                    Dim text2 As String = text & " a rejoint le salon"

                    'GET IP ADDRESS
                    clientIP = CType(client.Client.RemoteEndPoint, IPEndPoint).Address.ToString()
                    'GET IP ADDRESS

                    Dim text3 As String = text & " a rejoint le salon avec l'adresse IP: " & ClientIP

                    For Each c As TcpClient In clients
                        ns = c.GetStream()
                        ns.Write(Encoding.UTF8.GetBytes(text2), Nothing, text2.Length)
                    Next

                    Console.WriteLine(text3)

                Else

                    Dim ReceiveMessage As String = text.Split(":")(1).Trim()

                    If ReceiveMessage.Length > 0 And ReceiveMessage.Length <301 Then
                        Console.WriteLine(text & vbCrLf)

                        For Each c As TcpClient In clients
                            ns = c.GetStream()
                            ns.Write(Encoding.UTF8.GetBytes(text), 0, text.Length)
                        Next
                    End If

                End If

            End While

        Catch ex As Exception

            If clients.Contains(client) Then
                Console.WriteLine("Le client " & clientName(clientID) & " s'est déconnecté")
                clients.Remove(client)
                clientName.RemoveAt(clientID)
            End If

        End Try

    End Sub

End Module
