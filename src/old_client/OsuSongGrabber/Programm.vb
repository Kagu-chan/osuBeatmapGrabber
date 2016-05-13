Option Compare Binary
Option Explicit On
Option Infer On
Option Strict On
Imports System.Drawing
Imports System.IO
Imports System.IO.Path
Imports System.Security.AccessControl
Imports System.Text.RegularExpressions

Imports TagLib

''' <summary>
''' Main Module for the software
''' </summary>
Module Programm

    ''' <summary>
    ''' The registry key to find out osu! start command. This command is used to extract the osu path
    ''' </summary>
    Private _registryKey As String = "HKEY_CLASSES_ROOT\osu!\shell\open\command"

    ''' <summary>
    ''' If the key above does not work (osu bug?) try this one
    ''' </summary>
    Private _fallbackRegistryKey As String = "HKEY_CLASSES_ROOT\osu\DefaultIcon"

    ''' <summary>
    ''' The target directory where the songs will be saved
    ''' </summary>
    Private _copyToFolger As String = String.Empty

    ''' <summary>
    ''' The album name which will tagged to all songs
    ''' </summary>
    Private _albumName As String = "Osu!"

    ''' <summary>
    ''' The Path where osu is saved
    ''' </summary>
    Private _osuStoragePath As String = String.Empty

    ''' <summary>
    ''' If true, no album data will get saved to song files
    ''' </summary>
    ''' <remarks></remarks>
    Private _avoidAlbumName As Boolean = False

    ''' <summary>
    ''' Users preferences. <seealso cref="LoadUserPreferences">Before use please load user preferences</seealso>.
    ''' </summary>
    ''' <remarks></remarks>
    Public UserPreferences As New Dictionary(Of String, String)

    ''' <summary>
    ''' List of all invalid characters
    ''' </summary>
    ''' <remarks></remarks>
    Public InvalidCharacters As List(Of Char)

    Public Reg As New Regex("(tv|short|cut)([\s-\.]*)(ver|size|edit).*", System.Text.RegularExpressions.RegexOptions.IgnoreCase)

    ''' <summary>
    ''' osu! songs path depending on user preferences or default path
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property SongsPath As String
        Get
            Dim sPath As String = Path(UserPreferences("BeatmapDirectory"))
            Return If(Directory.Exists(sPath), sPath, UserEnterPath("songs"))
        End Get
    End Property

    ''' <summary>
    ''' Loads all user preferences
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LoadUserPreferences()
        Dim fileName As String = Path(GetConfigName())

        If Not IO.File.Exists(fileName) Then
            Console.WriteLine("Please Enter the path of your osu! configuration path")
            fileName = Console.ReadLine()

            If Not IO.File.Exists(fileName) Then
                LoadUserPreferences()
                Return
            End If
        End If

        Dim lines As IEnumerable(Of String) = IO.File.ReadAllLines(fileName)
        Dim findPattern As New Regex("^@?(\w+)\s?=\s?(.+)$", RegexOptions.IgnoreCase)

        For Each line As String In lines
            Dim match As Match = findPattern.Match(line)
            If match.Success Then
                UserPreferences(match.Groups(1).Value) = match.Groups(2).Value.Replace(Chr(13), vbNullString)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Find out the name of your personal osu! config file
    ''' </summary>
    ''' <returns>osu!.USERNAME.cfg</returns>
    Private Function GetConfigName() As String
        Dim userName As String = Environment.UserName
        Return String.Format("osu!.{0}.cfg", userName)
    End Function

    ''' <summary>
    ''' Returns the path where osu! is stored
    ''' </summary>
    ''' <returns>osu! storage path</returns>
    Private Function GetOsuStoragePath() As String
        Return GetOsuStoragePath(_registryKey, True)
    End Function

    Private Function GetOsuStoragePath(key As String, chain As Boolean) As String
        Dim osuStartCommand As String = CStr(My.Computer.Registry.GetValue(key, "", Nothing))
        If String.IsNullOrEmpty(osuStartCommand) Then Return If(chain, GetOsuStoragePath(_fallbackRegistryKey, False), UserEnterPath("storage"))

        Dim path As String = osuStartCommand.Substring(1, osuStartCommand.IndexOf("osu!.exe") - 1)

        Return If(Directory.Exists(path), path, If(chain, GetOsuStoragePath(_fallbackRegistryKey, False), UserEnterPath("storage")))
    End Function

    Private Function UserEnterPath(key As String) As String
        Console.WriteLine(String.Format("Cannot determine osu {0} path - please enter it below", key))
        Dim newPath As String = Console.ReadLine()

        Return If(Directory.Exists(newPath), newPath, UserEnterPath(key))
    End Function

    ''' <summary>
    ''' Combines the osu storage path with a path partial
    ''' </summary>
    ''' <param name="[partial]">The partial</param>
    ''' <returns>The full path</returns>
    Private Function Path([partial] As String) As String
        Return Combine(_osuStoragePath, [partial])
    End Function

    Sub Main()
        Console.WriteLine("Cleanup directory...")
        Threading.Thread.Sleep(3000)

        Dim fList As IEnumerable(Of String) = Directory.GetFiles(".")
        For Each file As String In fList
            If file.EndsWith(".upd") Then
                Dim oFile As String = file.Substring(0, file.IndexOf(".upd"))
                Try
                    IO.File.Delete(oFile)
                    IO.File.Move(file, oFile)
                Catch ex As Exception
                End Try
            End If
        Next

        _osuStoragePath = GetOsuStoragePath()
        LoadUserPreferences()

        Console.WriteLine("Please enter the path where beatmaps should be stored - leave blank for default ""OsuSongs"" in your music library")
        _copyToFolger = Console.ReadLine
        If String.IsNullOrEmpty(_copyToFolger) Then _copyToFolger = Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "OsuSongs")

        Console.WriteLine("If you want to save album information to files? If not, enter 'No'")
        _avoidAlbumName = Console.ReadLine.ToLower.Equals("no")
        If _avoidAlbumName Then _albumName = Nothing

        InvalidCharacters = New List(Of Char)(IO.Path.GetInvalidFileNameChars)
        InvalidCharacters.AddRange({"("c, ")"c, "."c, "~"c, "["c, "]"c})

        If Not Directory.Exists(_copyToFolger) Then Directory.CreateDirectory(_copyToFolger)

        Dim dInfo As New DirectoryInfo(_copyToFolger)
        Dim dSecurity As DirectorySecurity = dInfo.GetAccessControl()

        dSecurity.AddAccessRule(New FileSystemAccessRule(Environment.UserDomainName() & "\" & Environment.UserName(), FileSystemRights.Modify, AccessControlType.Allow))
        dInfo.SetAccessControl(dSecurity)

        Console.WriteLine()
        Console.WriteLine("This release is only to fix some issues. Currently we're rewriting the complete tool and updater.")
        Threading.Thread.Sleep(3000)

        Console.WriteLine()
        Console.WriteLine("Create beatmap list...")
        Threading.Thread.Sleep(2000)

        Dim beatmapList As New List(Of String)
        CreateFileList(beatmapList)

        Console.WriteLine()
        Console.WriteLine("Filter files...")
        Threading.Thread.Sleep(2000)

        Dim pairs As Dictionary(Of String, String()) = FilterList(beatmapList)

        Console.WriteLine()
        Console.WriteLine("Grab Files...")
        Threading.Thread.Sleep(2000)

        Dim files As UInteger = GrabSongs(pairs)

        Console.WriteLine()
        Console.WriteLine("Finnished grabbing...")
        Console.WriteLine(String.Format("Found {0} Beatmaps, filtered down to {1} entries - grabbed {2} files!", beatmapList.Count, pairs.Keys.Count, files))
        Console.WriteLine()
        Console.WriteLine("Hit enter to exit...")
        Console.ReadLine()
    End Sub

    Private Sub CreateFileList(ByRef fList As List(Of String), Optional startDirectory As String = "")
        Console.Title = String.Format("Search Beatmaps... {0} beatmaps so far", fList.Count)

        If String.IsNullOrEmpty(startDirectory) Then startDirectory = SongsPath()

        Dim currentFName As String
        Dim foundedFile As String
        Dim foundedFolder As String
        Dim directories() As String = {}
        Dim directoryNumber As Integer

        'Attach backslash if not present
        If Not (startDirectory.EndsWith("\")) Then startDirectory += "\"

        'read all contents of a directory
        Try
            currentFName = Dir(startDirectory & "*.*")
        Catch e As Exception
            Return
        End Try

        While Len(currentFName) > 0
            foundedFile = startDirectory & currentFName 'filename is now the current file

            currentFName = Dir()
            ' we only want .osu files
            If Not (foundedFile.EndsWith(".osu")) Then Continue While

            fList.Add(foundedFile)

            Console.WriteLine("Found: " & foundedFile)
        End While

        'read subdirectories
        directoryNumber = 0
        currentFName = Dir(startDirectory, vbDirectory)
        While Len(currentFName) > 0
            If currentFName <> "." AndAlso currentFName <> ".." Then
                directoryNumber = directoryNumber + 1
                ReDim Preserve directories(directoryNumber)
                directories(directoryNumber - 1) = currentFName
            End If
            currentFName = Dir()
        End While

        For i = 0 To directoryNumber - 1
            foundedFolder = startDirectory & directories(i)
            CreateFileList(fList, foundedFolder)
        Next
    End Sub

    Private Function FilterList(beatmapList As List(Of String)) As Dictionary(Of String, String())
        Console.Title = "Filter Beatmaps..."

        Dim songsList As New Dictionary(Of String, String())

        For Each file As String In beatmapList
            Dim data As IEnumerable(Of String) = GetRelevantData(file)
            If data.Count() < 1 Then
                Continue For
            End If

            Dim path As String = Combine(GetDirectoryName(file), data(0))
            Dim imgPath As String = If(String.IsNullOrEmpty(data(3)), String.Empty, Combine(GetDirectoryName(file), data(3)))

            Dim fullName As String = String.Format(
                "{0} - {1}",
                data(1).Trim(),
                data(2).Trim()
            )
            If songsList.ContainsKey(fullName.ToLower) Then Continue For ' alredy found
            If fullName.ToLower.EndsWith("[short ver]") AndAlso songsList.ContainsKey(fullName.ToLower.Replace(" [short ver]", "")) Then Continue For ' its a short ver but contains a non short ver
            If Not fullName.ToLower.EndsWith("[short ver]") AndAlso songsList.ContainsKey(fullName.ToLower & " [short ver]") Then songsList.Remove(fullName.ToLower & " [short ver]") ' its a non short ver - we remove the short one

            songsList.Add(fullName.ToLower, {data(0), data(1), data(2), path, imgPath})

            Console.WriteLine("Filtered: " & fullName)
            Console.Title = String.Format("Filter Beatmaps... filtered from {0} to {1} beatmaps now", beatmapList.Count, songsList.Keys.Count)
        Next

        Return songsList
    End Function

    Private Function GetRelevantData(ByVal file As String) As IEnumerable(Of String)
        Dim content As String
        Try
            content = IO.File.ReadAllText(file)
        Catch ex As PathTooLongException
            Return New List(Of String)()
        End Try

        Dim settings As String() = {"AudioFilename", "Artist", "Title", String.Empty}
        Dim i As Integer = 0

        For Each setting As String In settings
            If String.IsNullOrEmpty(setting) Then Continue For
            Dim expression As New Regex(String.Format("^{0}\s?:\s?(.+)$", setting), RegexOptions.Multiline Or RegexOptions.IgnoreCase)
            Dim result As Match = expression.Match(content)
            settings(i) = result.Groups(1).Value.Replace(Chr(13), vbNullString)

            i += 1
        Next

        ' remove invalid characters!
        settings(1) = String.Join("", settings(1).Split(InvalidCharacters.ToArray()))
        settings(2) = String.Join("", settings(2).Split(InvalidCharacters.ToArray()))

        Dim wStr As String = settings(2)
        settings(2) = Reg.Replace(settings(2), String.Empty).Trim()

        If Not wStr.Equals(settings(2)) Then
            settings(2) &= " [Short Ver]"
        End If

        Dim image As Match = (New Regex("^0,0,""([^""]+)"".*", RegexOptions.Multiline Or RegexOptions.IgnoreCase)).Match(content)
        settings(3) = If(image.Success, image.Groups(1).Value, String.Empty)

        Return settings
    End Function

    Private Function GrabSongs(ByVal list As Dictionary(Of String, String())) As UInteger
        Dim file As TagLib.File = Nothing

        Console.Title = "Copy Songs..."
        Dim tmpPath As String = Combine(My.Computer.FileSystem.SpecialDirectories.Temp, Guid.NewGuid.ToString & ".jpg")

        For Each foundFile As String In My.Computer.FileSystem.GetFiles(_copyToFolger)
            My.Computer.FileSystem.DeleteFile(foundFile)
        Next

        Dim titleNumber As UInteger = 0

        For Each pair As KeyValuePair(Of String, String()) In list
            Dim interpret As String = GetInvalidFileNameChars().Aggregate(pair.Value(1), Function(current, c) current.Replace(c.ToString(), String.Empty)).Trim()
            Dim title As String = GetInvalidFileNameChars().Aggregate(pair.Value(2), Function(current, c) current.Replace(c.ToString(), String.Empty)).Trim()
            Dim originalFile As String = pair.Value(3)
            Dim img As IPicture = Nothing

            If Not String.IsNullOrEmpty(pair.Value(4)) AndAlso IO.File.Exists(pair.Value(4)) Then
                Dim cropped As Image = Nothing
                Dim image As Bitmap = Nothing
                Try
                    image = New Bitmap(pair.Value(4))
                    If image.Width = image.Height Then cropped = image.Clone(New Rectangle(0, 0, image.Width, image.Height), Drawing.Imaging.PixelFormat.DontCare)
                    If image.Width < image.Height Then
                        cropped = image.Clone(New Rectangle(0, CInt((image.Height - image.Width) / 2), image.Width, image.Width), Drawing.Imaging.PixelFormat.DontCare)
                    End If
                    If image.Width > image.Height Then
                        cropped = image.Clone(New Rectangle(CInt((image.Width - image.Height) / 2), 0, image.Height, image.Height), Drawing.Imaging.PixelFormat.DontCare)
                    End If
                    cropped.Save(tmpPath)

                    img = New Picture(tmpPath)
                Catch ex As Exception
                    img = Nothing
                Finally
                    If Not IsNothing(cropped) Then cropped.Dispose()
                    If Not IsNothing(image) Then image.Dispose()
                End Try
            End If

            Dim fileName As String = String.Format("{0}\{1} - {2}.mp3", _copyToFolger, interpret, title)

            Try
                IO.File.Copy(originalFile, fileName, True)
            Catch ex As Exception
                Console.WriteLine("Skip " & fileName)
                Console.WriteLine(ex.Message)
                Console.WriteLine()
                Threading.Thread.Sleep(1000)
                Continue For
            End Try

            titleNumber += 1UI

            Try
                file = TagLib.File.Create(fileName)

                With file.Tag
                    ' first clear all data
                    '@see <http://stackoverflow.com/questions/17292142/taglib-sharp-not-editing-artist>

                    .Album = Nothing
                    .AlbumArtists = Nothing
                    .Performers = Nothing
                    .Title = Nothing
                    .Track = Nothing
                    .Pictures = Nothing

                    ' fill in data
                    If Not _avoidAlbumName Then
                        .Album = _albumName
                        .AlbumArtists = {_albumName}
                    End If

                    .Performers = {interpret}
                    .Title = title
                    .Track = titleNumber
                    .Pictures = If(IsNothing(img), {}, {img})
                End With

                file.Save()
            Catch ex As Exception
                Console.WriteLine("Issue with " & fileName)
                Console.WriteLine(ex.Message)
                Console.WriteLine()
                Threading.Thread.Sleep(1000)
            Finally
                If Not IsNothing(file) Then file.Dispose()
            End Try

            Console.WriteLine("Grabbed: " + fileName)
            Console.Title = String.Format("Copy Songs... {0} from {1} songs so far", titleNumber, list.Count)
        Next

        Try
            IO.File.Delete(tmpPath)
        Catch ex As Exception
            ' NOP - its a tmp file!
        End Try

        Return titleNumber
    End Function

End Module
