''' <summary>
''' Assorted helper methods
''' </summary>
Public Class Helpers
	''' <summary>
	''' Removes invalid characters from a path
	''' </summary>
	''' <param name="path">Path to check</param>
	''' <returns>Path without invalid chars</returns>
	Public Shared Function ReplaceInvalidChars(ByVal path As String) As String
		If path = "" Then
			Return ""
		End If

		For Each character As Char In IO.Path.GetInvalidFileNameChars()
			path = path.Replace(character, "")
		Next
		For Each character In IO.Path.GetInvalidPathChars()
			path = path.Replace(character, "")
		Next
		Return path
	End Function
End Class
